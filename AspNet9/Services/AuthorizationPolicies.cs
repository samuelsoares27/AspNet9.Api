using Microsoft.AspNetCore.Authorization;

namespace AspNet9.WebApi.Services
{
    public static class AuthorizationPolicies
    {
        public static void AddPolicies(this AuthorizationOptions options)
        {
            options.AddPolicy("AdminWithInsertClaim", policy =>
            {
                policy.RequireRole("Admin"); 
                policy.RequireClaim("Inserir", "true"); 
            });

            options.AddPolicy("AdminWithMultipleClaims", policy =>
            {
                policy.RequireRole("Admin"); 
                policy.RequireClaim("Inserir", "true"); 
                policy.RequireClaim("Editar", "true"); 
                policy.RequireClaim("Excluir", "true"); 
                                                       
            });

            // Adicione outras políticas conforme necessário
        }
    }

}
