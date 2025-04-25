namespace lengocsiliem_2122110324.Dto
{
    public class CartDTO
    {
        public long CartId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<ProductDTO> Products { get; set; }
    }
}
