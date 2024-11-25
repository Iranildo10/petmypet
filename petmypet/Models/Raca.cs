using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace petmypet.Models
{
    [Table("Racas")]
    public class Raca
    {
        [Key]
        public int Id { get; set; } // ID da raça

        [Required(ErrorMessage = "O nome da raça é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da raça não pode exceder 100 caracteres.")]
        public string Nome { get; set; } // Nome da raça (ex: Labrador, Siamês, etc.)

        // Chave estrangeira para TipoAnimal
        [Required]
        [ForeignKey("TipoAnimal")]
        public int TipoAnimalId { get; set; } // ID do tipo de animal (cachorro, gato, etc.)

        public TipoAnimal TipoAnimal { get; set; } // Propriedade de navegação para TipoAnimal

        [Required]
        public bool Ativo { get; set; } // Se a raça está ativa ou não
    }
}
