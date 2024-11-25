using Microsoft.AspNetCore.Identity;

namespace petmypet.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public bool Ativo { get; set; }
    }
}
