using System.Text.Json.Serialization;

namespace lengocsiliem_2122110324.Model
{
    public class Cart
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public int? UserId { get; set; }
        [JsonIgnore]
        public virtual User? User { get; set; }
        public virtual ICollection<CartItem> Items { get; set; }
    }
}
