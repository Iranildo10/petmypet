using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace petmypet.Models
{
    [Table("HorariosAgendas")]
    public class HorarioAgenda
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Horário é obrigatório.")]
        [Column(TypeName = "time")]
        public TimeSpan Horario { get; set; }

        [Required(ErrorMessage = "O campo Agenda é obrigatório.")]
        [ForeignKey("Agenda")]
        public int AgendaId { get; set; }

        public Agenda Agenda { get; set; }
    }
}


