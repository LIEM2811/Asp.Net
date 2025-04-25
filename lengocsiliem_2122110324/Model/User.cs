using System.Text.Json.Serialization;

namespace lengocsiliem_2122110324.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Cart? Cart { get; set; }
        public ICollection<UserRoles> UserRoles { get; set; }


    }
}
