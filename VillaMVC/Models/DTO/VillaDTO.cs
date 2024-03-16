namespace VillaMVC.Models.DTO;

public class VillaDTO
{
    public int? ID { get; set; }
    public string Name { get; set; }
    public string Details { get; set; }
    public string? ImageUrl { get; set; }
    
    public IFormFile? Image { get; set; }
}