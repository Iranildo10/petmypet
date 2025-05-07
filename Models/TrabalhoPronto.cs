using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace petmypet.Models
{
    [Table("TrabalhosProntos")]
    public class TrabalhoPronto
    {
        [Key] // Define que este campo é a chave primária.
        public int Id { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")] // Define que o campo é obrigatório.
        [StringLength(500, ErrorMessage = "A descrição pode ter no máximo 500 caracteres.")] // Limita o tamanho do texto.
        public string Descricao { get; set; }

        [Display(Name = "Imagem do Trabalho")]
        public string? ImagemUrl { get; set; } // Permite valores nulos

        [Required] // Define que o campo é obrigatório.
        public bool Ativo { get; set; }
    }
}
