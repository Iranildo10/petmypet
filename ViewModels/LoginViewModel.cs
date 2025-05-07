using System.ComponentModel.DataAnnotations;

namespace petmypet.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O número de telefone é obrigatório.")]
        [Phone(ErrorMessage = "Por favor, insira um número de telefone válido.")]
        [Display(Name = "Número de Telefone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        //[StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
