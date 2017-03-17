

namespace newdot.Models
{
    public class User : BaseModel
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
    }
}