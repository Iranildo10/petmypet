using Microsoft.AspNetCore.Identity;

namespace petmypet.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; }
        public string Endereco { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
    }
}
