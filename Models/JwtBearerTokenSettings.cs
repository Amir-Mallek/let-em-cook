namespace let_em_cook.Models;

public class JwtBearerTokenSettings
{
    public string SecretKey { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int ExpireTimeInSeconds { get; set; }
}