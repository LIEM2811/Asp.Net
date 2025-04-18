using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using lengocsiliem_2122110324.Dto;
using lengocsiliem_2122110324.Model;

public class OrderDetail
{
    [Key]
    public int OrderDetailId { get; set; }

    [JsonIgnore]
    public Order? Order { get; set; }
    public int OrderId { get; set; }

    [JsonIgnore]
    public Product? Product { get; set; }
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Discount { get; set; }  // Ví dụ: 0.1 là giảm 10%

    public decimal Total => Quantity * UnitPrice * (1 - Discount);


}
