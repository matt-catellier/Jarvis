using System.ComponentModel.DataAnnotations;

namespace Jarvis_Phase3.Models
{
    public class EditableUser
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
        public string ID { get; set; }

        public EditableUser(string iD, string userName, string email) {
            ID = iD;
            UserName = userName;
            Email = email;
        }
        public EditableUser() { }

    }
}