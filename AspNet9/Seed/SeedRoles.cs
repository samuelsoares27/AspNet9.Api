using Microsoft.AspNetCore.Identity;

namespace AspNet9.Seed
{

    public static class RoleSeeder
    {
        /// <summary>
        /// Método para inicializar as roles no banco de dados, se elas não existirem.
        /// </summary>
        /// <param name="roleManager">Instância do RoleManager.</param>
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // Lista de roles que você deseja criar
            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                // Verifica se a role já existe no banco de dados
                if (!await roleManager.RoleExistsAsync(role))
                {
                    // Cria a role
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
