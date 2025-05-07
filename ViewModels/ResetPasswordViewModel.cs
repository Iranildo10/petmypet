using System.ComponentModel.DataAnnotations;

public class ResetPasswordViewModel
{
    public string UserId { get; set; }

    [Required(ErrorMessage = "A nova senha é obrigatória.")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "A confirmação de senha é obrigatória.")]
    [Compare("NewPassword", ErrorMessage = "As senhas não coincidem.")]
    public string ConfirmPassword { get; set; }
}
