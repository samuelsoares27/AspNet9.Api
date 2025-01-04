using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspNet9.WebApi.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TokenService(
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string> GenerateJwtTokenAsync(IdentityUser user)
        {
            // Obter roles associadas ao usuário
            var userRoles = await _userManager.GetRolesAsync(user);

            // Inicializar a lista de claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Adicionar as roles como claims
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Adicionar Role Claims de cada role associada ao usuário
            foreach (var role in userRoles)
            {
                // Buscar a role pelo nome
                var roleEntity = await _roleManager.FindByNameAsync(role);
                if (roleEntity != null)
                {
                    // Obter as claims associadas à role
                    var roleClaims = await _roleManager.GetClaimsAsync(roleEntity);
                    claims.AddRange(roleClaims);
                }
            }

            // Configurações do token JWT
            var secretKey = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
                throw new InvalidOperationException("JwtSettings:SecretKey não configurado no appsettings.");

            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                SecurityAlgorithms.HmacSha256
            );

            // Criar o token JWT
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),  // Token válido por 30 minutos
                signingCredentials: credentials
            );

            // Retornar o token como string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
