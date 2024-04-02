using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API_TRADUCTOR.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly Services.AuthenticationService _authenticationService;

        private readonly ApiTraductorDB _db;

        public LoginController(IConfiguration configuration, Services.AuthenticationService authenticationService, ApiTraductorDB db)
        {
            _configuration = configuration;
            _authenticationService = authenticationService;
            _db = db;
        }

        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            // Suponiendo que tienes una propiedad de navegación "Role" en tu clase User
            var loggedUser = _db.Users
                .FirstOrDefault(a => (a.Email == email) && a.Password == password);
            // Aquí iría tu lógica de autenticación para verificar las credenciales y obtener el rol del usuario
            if (loggedUser == null)
                return Unauthorized();

            // Si las credenciales son válidas, generamos el token
            var token = _authenticationService
                .GenerateJSONWebToken(loggedUser.Email,loggedUser.Id, TimeSpan.FromHours(2)); // Token válido por

            // Guardar el token en la cookie
            Response.Cookies.Append("jwtToken", token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddHours(2), // La cookie expira junto con el token inicial
                                                    // Aquí puedes configurar otras opciones de cookie si lo deseas
            });

            // Devolver el token
            return Ok(new { token });
        }

        [HttpPost("refresh")]
        public IActionResult RefreshToken()
        {
            var token = Request.Cookies["jwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                // No hay token en la cookie, probablemente el usuario no está autenticado
                return Unauthorized();
            }

            // Aquí puedes implementar la lógica para refrescar el token si es necesario
            // Por simplicidad, aquí simplemente devolvemos el mismo token
            var refreshedToken = _authenticationService.RefreshJSONWebToken(token, TimeSpan.FromDays(5)); // Refresco del token cada 5 días
            Response.Cookies.Append("jwtToken", refreshedToken, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.Add(TimeSpan.FromDays(5)), // Renueva la cookie con el nuevo token
                // Aquí puedes configurar otras opciones de cookie si lo deseas
            });

            return Ok(new { token = refreshedToken });
        }

        [HttpPost("ValidarToken")]
        public IActionResult ValidarToken(string token)
        {
            var jwtTokenCookie = Request.Cookies["jwtToken"];

            if (jwtTokenCookie.IsNullOrEmpty())
            {
                return Unauthorized();
            }
            else if (jwtTokenCookie != token)
            {
                return Ok(jwtTokenCookie);
            }

            return Ok(jwtTokenCookie);
        }


    }
}
