using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace petmypet.Models
{
    [Table("TiposAnimais")]
    public class TipoAnimal
    {
        [Key]
        public int Id { get; set; } // ID do tipo de animal

        [Required(ErrorMessage = "O nome do tipo de animal é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome do tipo de animal não pode exceder 50 caracteres.")]
        public string Nome { get; set; } // Nome do tipo de animal (ex: cachorro, gato, etc.)

        [Required]
        public bool Ativo { get; set; } // Se o tipo de animal está ativo ou não
    }
}
