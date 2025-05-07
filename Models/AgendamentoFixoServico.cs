using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace petmypet.Models
{
    public class AgendamentoFixoServico
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AgendamentoFixo")]
        public int AgendamentoFixoId { get; set; }
        public AgendamentoFixo AgendamentoFixo { get; set; }

        [ForeignKey("Servico")]
        public int ServicoId { get; set; }
        public Servico Servico { get; set; }
    }
}
