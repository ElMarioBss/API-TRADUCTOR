using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_TRADUCTOR.Services
{
    public class AuthenticationService
    {
        private readonly IConfiguration _configuration;

        public AuthenticationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJSONWebToken(string email, int id, TimeSpan expiryTime)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, id.ToString()),
                new Claim(ClaimTypes.Email, email)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.Add(expiryTime),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string RefreshJSONWebToken(string token, TimeSpan expiryTime)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            // Validar y leer los claims del token existente
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false // Desactivamos la validación de la caducidad para poder leer los claims
            };

            SecurityToken validatedToken;
            var principal = jwtTokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);

            // Generar nuevos claims si es necesario
            var claims = new List<Claim>();
            foreach (var claim in principal.Claims)
            {
                if (claim.Type != ClaimTypes.Expiration)
                {
                    claims.Add(claim);
                }
            }

            // Generar nuevo token con los claims actualizados
            var newToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.Add(expiryTime),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(newToken);
        }

        public bool ValidateToken(string token)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true // Activa la validación de la caducidad del token
            };

            try
            {
                jwtTokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                return true; // El token es válido
            }
            catch (Exception)
            {
                return false; // El token no es válido
            }
        }

    }
}
