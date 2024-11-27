using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Servico
{
    [Key] // Define como chave primária
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Geração automática de ID
    public int Id { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória.")] // Campo obrigatório
    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")] // Tamanho máximo
    public string Descricao { get; set; }

    [Display(Name = "Imagem do Serviço")]
    public string ImagemUrl { get; set; } // Caminho ou URL para a imagem

    [Display(Name = "Tempo de espera do serviço")]
    public int Duracao { get; set; } // Duração em minutos

    [Required(ErrorMessage = "O valor é obrigatório.")] // Campo obrigatório
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")] // Validação para valores positivos
    [Column(TypeName = "decimal(18,2)")] // Especifica o tipo no banco de dados
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "O status de ativo é obrigatório.")] // Campo obrigatório
    public bool Ativo { get; set; } // Indica se o serviço está ativo
}
