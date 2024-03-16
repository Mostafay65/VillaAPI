namespace VillaAPI.Models;

public class RefreshToken
{
    public int ID { get; set; }
    public string UserId { get; set; }
    public string JwtTokenId { get; set; }
    public string Refresh_Token { get; set; }
    public bool IsValid { get; set; }
    public DateTime ExpireAt { get; set; }
}