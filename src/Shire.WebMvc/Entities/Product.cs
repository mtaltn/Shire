namespace Shire.WebMvc.Entities;

public class Product : EntityBase
{
    public string Description { get; set; }
    public string CategoryId { get; set; }
    public decimal Price { get; set; }
}
