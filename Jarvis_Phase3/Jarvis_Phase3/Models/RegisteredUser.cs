using System.ComponentModel.DataAnnotations;

namespace Jarvis_Phase3.Models
{
    public class RegisteredUser
    {
        // add 2 more properties here
        [Required]
        [Display(Name = "User Name:")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "First Name:")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name:")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",
        ErrorMessage = "This is not a valid email address.")]
        [Display(Name = "Email:")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        [MinLength(6, ErrorMessage = "Password needs to be atleast 6 characters")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password:")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }

        public RegisteredUser() { }
        public RegisteredUser(string email, string subject, string body)
        {
            Email = email;
            Subject = subject;
            Body = body;

        }

    }
}