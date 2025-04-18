namespace lengocsiliem_2122110324.Request
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; } // Add this property
        public List<int> Roles { get; set; } // Add this property
    }

}
