using lengocsiliem_2122110324.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Order
{
    [Key]
    public int OrderId { get; set; }
    public string Email { get; set; }
    public DateTime OrderDate { get; set; }
    public string Payment { get; set; }
    public decimal TotalAmount { get; set; }
    public string OrderStatus { get; set; }
    public string? Note { get; set; }
    public List<OrderDetail> OrderItems { get; set; }


    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; } // Sửa lỗi trùng thuộc tính CreatedAt

}
