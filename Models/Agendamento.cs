using petmypet.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Agendamento
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Pet")]
    public int PetId { get; set; }

    public Pet Pet { get; set; }

    [Required]
    public DateTime Data { get; set; }

    [Required]
    public DateTime Horario { get; set; }

    [Required]
    public bool TaxiDog { get; set; }

    [Required]
    public bool ClienteTraz { get; set; }

    public int? DiaVencimentoPacote { get; set; }

    public bool? PacoteEmDia { get; set; }

    public DateTime? DataUltimoPagamento { get; set; }
    public DateTime? DataProximoVencimento { get; set; }

    [Required]
    public bool HorarioFixoSemanal { get; set; }

    public string? Observacao { get; set; }

    public ICollection<AgendamentoServico> AgendamentoServicos { get; set; }
}

