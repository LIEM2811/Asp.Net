using System.ComponentModel.DataAnnotations;
using lengocsiliem_2122110324.Model;

public class Category
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Product> Products { get; set; } = new List<Product>();
}
