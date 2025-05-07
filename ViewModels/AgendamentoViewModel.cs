using Microsoft.AspNetCore.Mvc.Rendering;
using petmypet.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class AgendamentoViewModel
{
    public int Id { get; set; }
    public DateTime Horario { get; set; }
    public Pet Pet { get; set; }
    public string Observacao { get; set; }
    public IEnumerable<Servico> Servicos { get; set; }
    public bool? TaxiDog { get; set; }

    public bool? ClienteTraz { get; set; }
    public String? EnderecoCliente { get; set; }

    public string Tipo { get; set; } // "Normal" ou "Fixo"
}




