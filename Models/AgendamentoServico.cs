using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class AgendamentoServico
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Agendamento")]
    public int AgendamentoId { get; set; }

    public Agendamento Agendamento { get; set; }

    [ForeignKey("Servico")]
    public int ServicoId { get; set; }

    public Servico Servico { get; set; }
}
