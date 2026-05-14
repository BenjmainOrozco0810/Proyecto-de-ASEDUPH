using ASEDUPH_V2_API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

SqlConnection.ClearAllPools();

// ── Base de datos ─────────────────────────────────────────────────
builder.Services.AddDbContext<AseduphDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ConexionASEDUPH"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
            sqlOptions.CommandTimeout(60);
        }
    ));

// ── Autenticación JWT ─────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key no configurada en appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            )
        };
    });

builder.Services.AddAuthorization();

// ── Controladores ─────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger con soporte para JWT ──────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(); 

var app = builder.Build();

// ── Pipeline HTTP ─────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Primero autenticación
app.UseAuthorization();  // Luego autorización

app.MapControllers();

app.Run();