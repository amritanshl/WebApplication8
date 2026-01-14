using System.ComponentModel.DataAnnotations;
using System.Configuration;
namespace WebApplication8.Models
{
   
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Please enter your email")]
        [EmailAddress(ErrorMessage ="Invalid email address")]
        
        public string Email { get; set; }

        [Required(ErrorMessage ="Please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


    }
}
