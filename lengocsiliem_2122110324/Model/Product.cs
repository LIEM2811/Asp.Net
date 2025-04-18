using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using lengocsiliem_2122110324.Dto;

namespace lengocsiliem_2122110324.Model
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public Double Price { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}
