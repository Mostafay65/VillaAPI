namespace VillaAPI.Models.DTO;

public class VillaNumberDto
{
    public int VillaNo  { get; set; }
    public string SpecialDetails { get; set; }
    public string Image { get; set; } 
    public int  VillaID { get; set; }
    public VillaDTO? Villa { get; set; }
}