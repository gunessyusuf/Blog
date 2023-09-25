namespace WebApi.Models
{
    public class JwtOptionsModel
    {
        public string Audience { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public int ExpirationMinutes { get; set; }
        public string SecurityKey { get; set; } = null!;
    }
}
