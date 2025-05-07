using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace petmypet.Models
{
    public class AgendamentoFixo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DayOfWeek DiaSemana { get; set; }

        [Required]
        public DateTime Horario { get; set; }

        [ForeignKey("Pet")]
        public int PetId { get; set; }

        public Pet Pet { get; set; }

        [Required]
        public bool TaxiDog { get; set; }

        [Required]
        public bool ClienteTraz { get; set; }

        public int? DiaVencimentoPacote { get; set; }

        public bool? PacoteEmDia { get; set; }

        public DateTime? DataUltimoPagamento { get; set; }
        public DateTime? DataProximoVencimento { get; set; }


        public string? Observacao { get; set; }

        public ICollection<AgendamentoFixoServico> Servicos { get; set; }
    }

}
