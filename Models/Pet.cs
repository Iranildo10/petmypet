using petmypet.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Pet
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; }

    [ForeignKey("Cliente")]
    public int ClienteId { get; set; }

    public Cliente? Cliente { get; set; }

    public string? Observacao { get; set; }

    public IEnumerable<Agendamento>? Agendamentos { get; set; }
    public ICollection<AgendamentoFixo>? AgendamentosFixos { get; set; }
}
