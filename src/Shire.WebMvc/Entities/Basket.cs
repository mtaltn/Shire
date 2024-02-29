namespace Shire.WebMvc.Entities;

public class Basket
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string UserId { get; set; }
    public decimal Price { get; set; }

    public virtual Product Product { get; set; }
}
