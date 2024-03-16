using VillaAPI.Models.DTO;

namespace VillaMVC.Models.DTO;

public class LoginResponseDTO
{
    public UserDTO User { get; set; }
    public TokenDTO Token { get; set; } 
    public string Role { get; set; }

}