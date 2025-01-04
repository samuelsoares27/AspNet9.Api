namespace AspNet9.Model.Dto
{
    public class AddClaimDTO
    {
        public string Email { get; set; } = string.Empty;
        public string ClaimType { get; set; } = string.Empty;
        public string ClaimValue { get; set; } = string.Empty;
    }
}
