using AspNet9.Data;
using AspNet9.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _context;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, DataContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // Endpoint para listar todos os usuários
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        // Endpoint para criar um usuário
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
        {
            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Ok(new { Message = "User created successfully!" });
            }

            return BadRequest(result.Errors);
        }

        // Endpoint para atualizar informações de um usuário
        [HttpPut("update/{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found!" });
            }

            user.Email = model.Email;
            user.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { Message = "User updated successfully!" });
            }

            return BadRequest(result.Errors);
        }

        // Endpoint para excluir um usuário
        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found!" });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { Message = "User deleted successfully!" });
            }

            return BadRequest(result.Errors);
        }

        // Endpoint para listar todas as roles
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }

        // Endpoint para adicionar uma role a um usuário
        [HttpPost("add-role")]
        public async Task<IActionResult> AddRoleToUser([FromBody] AddRoleToUserDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found!" });
            }

            var role = await _roleManager.FindByNameAsync(model.RoleName);
            if (role == null)
            {
                return NotFound(new { Message = "Role not found!" });
            }

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Role added to user!" });
            }

            return BadRequest(result.Errors);
        }

        // Endpoint para remover uma role de um usuário
        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] AddRoleToUserDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found!" });
            }

            var role = await _roleManager.FindByNameAsync(model.RoleName);
            if (role == null)
            {
                return NotFound(new { Message = "Role not found!" });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Role removed from user!" });
            }

            return BadRequest(result.Errors);
        }

        // Obter Claims do Usuário
        [HttpGet("claims/{email}")]
        public async Task<IActionResult> GetClaims(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");

            var claims = await _userManager.GetClaimsAsync(user);
            return Ok(claims);
        }

        // Adicionar Claim ao Usuário
        [HttpPost("add-claim")]
        [Authorize(Roles = "Admin")]  // Permite apenas admins adicionarem claims
        public async Task<IActionResult> AddClaim([FromBody] AddClaimDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound("User not found");

            var claim = new Claim(dto.ClaimType, dto.ClaimValue);
            var result = await _userManager.AddClaimAsync(user, claim);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Claim added successfully.");
        }

        // Remover Claim do Usuário
        [HttpDelete("remove-claim")]
        [Authorize(Roles = "Admin")]  // Permite apenas admins removerem claims
        public async Task<IActionResult> RemoveClaim([FromBody] RemoveClaimDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound("User not found");

            var claim = new Claim(dto.ClaimType, dto.ClaimValue);
            var result = await _userManager.RemoveClaimAsync(user, claim);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Claim removed successfully.");
        }

    }

}
