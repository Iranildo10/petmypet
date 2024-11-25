using System.ComponentModel.DataAnnotations;

namespace petmypet.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

    }
}
