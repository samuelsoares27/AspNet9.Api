using AspNet9.Data;
using AspNet9.Seed;
using AspNet9.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
});

// Banco de Dados
var connectionString = builder.Configuration.GetConnectionString(Environment.MachineName) ??
                       builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddScoped<TokenService>();

// Configura��o JWT
var secretKey = builder.Configuration.GetValue<string>("JwtSettings:SecretKey");
var issuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer");
var audience = builder.Configuration.GetValue<string>("JwtSettings:Audience");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));

// Configura��o do Identity para APIs, incluindo UserManager e RoleManager
builder.Services.AddIdentity<IdentityUser, IdentityRole>() // Agora estamos usando Identity completo
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders(); // Mant�m os tokens padr�o

builder.Services.AddAuthorization(options =>
{
    options.AddPolicies();  // Registra todas as pol�ticas centralizadas
});

builder.Services.AddControllers();
builder.Services.AddOpenApi(); // OpenAPI (Swagger)

var app = builder.Build();

// Executa o seed das roles ap�s a aplica��o ser constru�da
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager);
}

// Pipeline de requisi��es
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.OAuthClientId("your-client-id");
        c.OAuthAppName("Your App");
    });
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Middleware de autentica��o
app.UseAuthorization();  // Middleware de autoriza��o

app.MapControllers();

app.Run();
