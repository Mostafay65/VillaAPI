using System.ComponentModel.DataAnnotations;

namespace VillaMVC.Models.DTO;

public class VillaNumberDto
{
    [Display(Name = "Villa Number")]
    public int VillaNo  { get; set; }
    [Display(Name = "Special Details")]
    public string SpecialDetails { get; set; }
    public string Image { get; set; } 
    public int  VillaID { get; set; }
    public VillaDTO? Villa { get; set; }
}