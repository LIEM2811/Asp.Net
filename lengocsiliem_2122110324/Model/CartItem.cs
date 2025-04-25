using System.Text.Json.Serialization;

namespace lengocsiliem_2122110324.Model
{
   public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; }
    [JsonIgnore]
    public virtual Cart? Cart { get; set; }
    public int ProductId { get; set; }
    [JsonIgnore]
    public virtual Product? Product { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; } // đơn giá
    public decimal? Discount { get; set; }
    public decimal? SpecialPrice { get; set; }
}
}
