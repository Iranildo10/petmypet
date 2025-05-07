using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace petmypet.ViewModels
{
    public class EditarAgendamentoFixoViewModel
    {
        public int Id { get; set; }

        [Required]
        public DayOfWeek DiaSemana { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime Horario { get; set; }

        [Required]
        public int PetId { get; set; }

        public string? Observacao { get; set; }

        [Required]
        public bool TaxiDog { get; set; }

        [Required]
        public bool ClienteTraz { get; set; }

        public List<int> ServicosSelecionados { get; set; } = new();

        // Dados auxiliares para preencher os dropdowns e checkboxes
        public List<Pet> PetsDisponiveis { get; set; } = new();
        public List<Servico> ServicosDisponiveis { get; set; } = new();
    }
}
