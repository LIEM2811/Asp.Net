namespace lengocsiliem_2122110324.Model
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
