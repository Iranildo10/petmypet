using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace petmypet.Models
{
    [Table("Agendas")]
    public class Agenda
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        public bool Seg { get; set; }

        public bool Ter { get; set; }

        public bool Qua { get; set; }

        public bool Qui { get; set; }

        public bool Sex { get; set; }

        public bool Sab { get; set; }

        public bool Dom { get; set; }

        [Required(ErrorMessage = "O campo horário inicial é obrigatório.")]
        [Column(TypeName = "time")]
        public TimeSpan HorarioInicial { get; set; }

        [Required(ErrorMessage = "O campo horário final é obrigatório.")]
        [Column(TypeName = "time")]
        public TimeSpan HorarioFinal { get; set; }

        [Required(ErrorMessage = "A duração é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A duração deve ser um número maior que zero.")]
        public int DuracaoHorario { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan InicioIntervalo { get; set; }


        [Column(TypeName = "time")]
        public TimeSpan FimIntervalo { get; set; }

        public bool Ativo { get; set; }

        // Propriedade de navegação
        public ICollection<HorarioAgenda> HorariosAgenda { get; set; }
    }
}


