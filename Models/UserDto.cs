using System.ComponentModel.DataAnnotations;
namespace SmileProject.Models
{

    public class UserDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "please enter your name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "please enter your family")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "please enter your phone number")]
        [RegularExpression(@"^09\d{9}$", ErrorMessage = "error please enter the correct form of number")]
        public string Mobile { get; set; }
    }
}
