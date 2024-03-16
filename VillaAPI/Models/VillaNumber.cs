using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VillaAPI.Models;

public class VillaNumber
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int VillaNo  { get; set; }
    public string SpecialDetails { get; set; }
    public DateTime CreatedTime { get; set; }
    
    public string Image { get; set; } = "https://placehold.co/600*400";
    
    public int VillaID { get; set; }
    public Villa Villa { get; set; }
}