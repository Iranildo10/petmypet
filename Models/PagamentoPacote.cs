using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace petmypet.Models
{
    public class PagamentoPacote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AgendamentoFixoId { get; set; }

        [ForeignKey("AgendamentoFixoId")]
        public AgendamentoFixo AgendamentoFixo { get; set; }

        [Required]
        public DateTime DataPagamento { get; set; }

        [Required]
        public DateTime ReferenteAoMes { get; set; }

        public decimal? ValorPago { get; set; }  // (opcional) Se quiser registrar valores
    }
}
