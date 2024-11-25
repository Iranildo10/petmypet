using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace petmypet.Models
{
    [Table("Pets")]
    public class Pet
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do pet é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do pet não pode exceder 100 caracteres.")]
        public string Nome { get; set; }

        [Url(ErrorMessage = "A URL da imagem é inválida.")]
        public string Imagem { get; set; }

        // Chaves estrangeiras para TipoAnimal e Raca
        [ForeignKey("TipoAnimal")]
        public int TipoAnimalId { get; set; }
        public TipoAnimal TipoAnimal { get; set; } // Navegação para TipoAnimal

        [ForeignKey("Raca")]
        public int RacaId { get; set; }
        public Raca Raca { get; set; } // Navegação para Raca

        [Required(ErrorMessage = "O sexo do pet é obrigatório.")]
        [StringLength(10, ErrorMessage = "O sexo do pet não pode exceder 10 caracteres.")]
        public string Sexo { get; set; } // Ex: "Macho", "Fêmea"

        [Required(ErrorMessage = "A informação sobre as vacinas é obrigatória.")]
        public bool VacinasEmDia { get; set; }

        [StringLength(500, ErrorMessage = "A observação não pode exceder 500 caracteres.")]
        public string Observacao { get; set; }

        [Required(ErrorMessage = "O ID do cliente (dono) é obrigatório.")]
        public string ClienteId { get; set; } // Chave estrangeira para ApplicationUser

        public bool Ativo { get; set; }

        // Navegação para o dono do pet (relacionamento com ApplicationUser)
        public ApplicationUser Cliente { get; set; }
    }
}
