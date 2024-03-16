namespace VillaAPI.Models.DTO;

public class LoginResponseDTO
{
    public LocalUserDTO User { get; set; }
    public TokenDTO Token { get; set; }   
    public string Role { get; set; }

}