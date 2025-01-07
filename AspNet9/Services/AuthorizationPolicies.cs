using Microsoft.AspNetCore.Authorization;

namespace AspNet9.WebApi.Services
{
    public static class AuthorizationPolicies
    {
        public static void AddPolicies(this AuthorizationOptions options)
        {
            // Política que exige a role "Admin" e que a claim "Tempo" contenha o valor "Inserir"
            options.AddPolicy("AdminWithInsertClaim", policy =>
            {
                policy.RequireRole("Admin");
                policy.RequireClaim("Tempo", "Inserir"); // Verifica se a claim "Tempo" contém o valor "Inserir"
            });

            // Política que exige a role "Admin" e que a claim "Tempo" contenha os valores "Inserir", "Editar" e "Excluir"
            options.AddPolicy("AdminWithMultipleClaims", policy =>
            {
                policy.RequireRole("Admin");
                policy.RequireClaim("Tempo", "Inserir");  // Verifica se a claim "Tempo" contém o valor "Inserir"
                policy.RequireClaim("Tempo", "Editar");   // Verifica se a claim "Tempo" contém o valor "Editar"
                policy.RequireClaim("Tempo", "Excluir");  // Verifica se a claim "Tempo" contém o valor "Excluir"
            });

            // Adicione outras políticas conforme necessário
        }
    }

}
