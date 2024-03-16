using VillaAPI.Models.DTO;
using VillaUtility;

namespace VillaMVC.Models;

public class APIRequest
{
    public  HttpMethod Method{ get; set; }
    public string URL { get; set; }
    public object? Data { get; set; }
    public TokenDTO Token { get; set; }
    public SD.ContentType ContentType { get; set; } = SD.ContentType.Json;
}
