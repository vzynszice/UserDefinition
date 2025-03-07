
namespace DAL.Models
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }
        public UserModel User { get; set; }
    }
}
