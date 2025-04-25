namespace lengocsiliem_2122110324.Dto
{
    public class ProductDTO
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal SpecialPrice { get; set; }
        public CategoryDTO Category { get; set; }
    }
}
