using API_TRADUCTOR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<API_TRADUCTOR.Services.AuthenticationService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        var authenticationService = serviceProvider.GetRequiredService<API_TRADUCTOR.Services.AuthenticationService>();

        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = "http://www.api-traductor.somee.com",
            ValidIssuer = "http://www.api-traductor.somee.com",
            ClockSkew = TimeSpan.Zero, // It forces tokens to expire exactly at token expiration time instead of 5 minutes later
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MynameisJamesBond007_MynameisJamesBond007")),

        };
    });

builder.Services.AddCors(options => { options.AddPolicy(name: "AllowAnyOrigin", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }); });

builder.Services.AddDbContext<ApiTraductorDB>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConStr")));


var app = builder.Build();

//app.UseCors(builder => builder
//     .AllowAnyOrigin()
//     .AllowAnyMethod()
//     .AllowAnyHeader()
//     .AllowCredentials());

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
