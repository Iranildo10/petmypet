using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using petmypet.Context;
using petmypet.Models;
using petmypet.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

[Authorize("Admin")]
public class AgendamentosController : Controller
{
    private readonly AppDbContext _context;

    public AgendamentosController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(DateTime? data)
    {
        var dataSelecionada = data ?? DateTime.Today;

        // Semana (segunda a domingo)
        var inicioSemana = dataSelecionada.AddDays(-(int)dataSelecionada.DayOfWeek + (int)DayOfWeek.Monday);
        var fimSemana = inicioSemana.AddDays(6);

        // ----------------------
        // AGENDAMENTOS DO DIA
        // ----------------------
        var agendamentos = _context.Agendamentos
            .Include(a => a.Pet)
                .ThenInclude(c => c.Cliente)
            .Include(a => a.AgendamentoServicos).ThenInclude(s => s.Servico)
            .Where(a => a.Data.Date == dataSelecionada.Date)
            .ToList();

        var diaSemana = dataSelecionada.DayOfWeek;

        var agendamentosFixos = _context.AgendamentosFixos
            .Include(f => f.Pet)
                .ThenInclude(c => c.Cliente)
            .Include(f => f.Servicos).ThenInclude(s => s.Servico)
            .Where(f => f.DiaSemana == diaSemana)
            .Select(f => new
            {
                f.Id,
                f.Horario,
                f.Pet,
                f.Observacao,
                f.Servicos,
                Tipo = "Fixo",
                f.ClienteTraz,
                f.TaxiDog
            }).ToList();

        var petsExibidos = new HashSet<int>();

        var agendamentosUnificados = new List<AgendamentoViewModel>();

        foreach (var agendamento in agendamentos)
        {

            var cliente = agendamento.Pet.Cliente;
            var endereco = !string.IsNullOrWhiteSpace(cliente.Endereco1) ? cliente.Endereco1 : cliente.Endereco2;

            if (!petsExibidos.Contains(agendamento.PetId))
            {
                petsExibidos.Add(agendamento.PetId);
                agendamentosUnificados.Add(new AgendamentoViewModel
                {
                    Id = agendamento.Id,
                    Horario = agendamento.Horario,
                    Pet = agendamento.Pet,
                    Observacao = agendamento.Observacao,
                    Servicos = agendamento.AgendamentoServicos.Select(s => s.Servico),
                    Tipo = "Avulso",
                    EnderecoCliente = endereco,
                    ClienteTraz = agendamento.ClienteTraz,
                    TaxiDog = agendamento.TaxiDog
                });
            }
        }

        foreach (var fixo in agendamentosFixos)
        {

            var cliente = fixo.Pet.Cliente;
            var endereco = !string.IsNullOrWhiteSpace(cliente.Endereco1) ? cliente.Endereco1 : cliente.Endereco2;

            if (!petsExibidos.Contains(fixo.Pet.Id))
            {
                petsExibidos.Add(fixo.Pet.Id);
                agendamentosUnificados.Add(new AgendamentoViewModel
                {
                    Id = fixo.Id,
                    Horario = fixo.Horario, // Deve ser DateTime!
                    Pet = fixo.Pet,
                    Observacao = fixo.Observacao,
                    Servicos = fixo.Servicos.Select(s => s.Servico),
                    Tipo = "Fixo",
                    EnderecoCliente = endereco,
                    ClienteTraz = fixo.ClienteTraz,
                    TaxiDog = fixo.TaxiDog
                });
            }
        }


        // --------------------------
        // OVERVIEW SEMANAL
        // --------------------------
        var resumoSemana = new List<object>();

        for (var dt = inicioSemana; dt <= fimSemana; dt = dt.AddDays(1))
        {
            var normais = _context.Agendamentos.Count(a => a.Data.Date == dt.Date);
            var fixos = _context.AgendamentosFixos.Count(f => f.DiaSemana == dt.DayOfWeek);

            resumoSemana.Add(new
            {
                Data = dt,
                TotalAgendamentos = normais + fixos
            });
        }

        ViewBag.DataSelecionada = dataSelecionada.ToString("yyyy-MM-dd");
        ViewBag.SemanaResumo = resumoSemana;
        ViewBag.SemanaInicio = inicioSemana;
        ViewBag.SemanaFim = fimSemana;

        return View(agendamentosUnificados);
    }

    // GET: Agendamentos/Create
    public IActionResult Create()
    {
        var petsComCliente = _context.Pets
            .Include(p => p.Cliente) // garante que traz o Cliente relacionado
            .Select(p => new
            {
                Id = p.Id,
                NomeCompleto = p.Nome + " - " + p.Cliente.Nome
            })
            .ToList();

        ViewBag.Pets = new SelectList(petsComCliente, "Id", "NomeCompleto");
        ViewBag.Servicos = _context.Servicos.ToList();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Agendamento agendamento, int[] servicoIds)
    {

        try
        {
            if (agendamento.HorarioFixoSemanal)
            {

                var hoje = DateTime.Today;
                var diaVencimento = agendamento.DiaVencimentoPacote ?? hoje.Day;
                var proximoVencimento = new DateTime(hoje.Year, hoje.Month, Math.Min(diaVencimento, DateTime.DaysInMonth(hoje.Year, hoje.Month)));

                // Criar agendamento fixo
                var fixo = new AgendamentoFixo
                {
                    PetId = agendamento.PetId,
                    DiaSemana = agendamento.Data.DayOfWeek,
                    Horario = agendamento.Horario,
                    TaxiDog = agendamento.TaxiDog,
                    ClienteTraz = agendamento.ClienteTraz,
                    PacoteEmDia = false,
                    DiaVencimentoPacote = agendamento.DiaVencimentoPacote,
                    DataUltimoPagamento = null,
                    DataProximoVencimento = proximoVencimento,
                    Observacao = agendamento.Observacao
                };

                _context.AgendamentosFixos.Add(fixo);
                _context.SaveChanges();

                foreach (var servicoId in servicoIds)
                {
                    _context.AgendamentoFixoServicos.Add(new AgendamentoFixoServico
                    {
                        AgendamentoFixoId = fixo.Id,
                        ServicoId = servicoId
                    });
                }

                _context.SaveChanges();
            }
            else
            {
                // Agendamento normal
                _context.Agendamentos.Add(agendamento);
                _context.SaveChanges();

                foreach (var servicoId in servicoIds)
                {
                    _context.AgendamentoServicos.Add(new AgendamentoServico
                    {
                        AgendamentoId = agendamento.Id,
                        ServicoId = servicoId
                    });
                }

                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));

        }
        catch (Exception)
        {
            var petsComCliente = _context.Pets
            .Include(p => p.Cliente) // garante que traz o Cliente relacionado
            .Select(p => new
            {
                Id = p.Id,
                NomeCompleto = p.Nome + " - " + p.Cliente.Nome
            })
            .ToList();

            ViewBag.Pets = new SelectList(petsComCliente, "Id", "NomeCompleto");
            ViewBag.Servicos = _context.Servicos.ToList();
            return View(agendamento);
            throw;
        }

        
    }

    public IActionResult Edit(int id)
    {
        var agendamento = _context.Agendamentos
            .Include(a => a.AgendamentoServicos)
            .FirstOrDefault(a => a.Id == id);

        if (agendamento == null)
            return NotFound();

        var isFixo = _context.AgendamentosFixos.Any(f =>
            f.PetId == agendamento.PetId &&
            f.DiaSemana == agendamento.Data.DayOfWeek &&
            f.Horario == agendamento.Horario);

        var model = new EditarAgendamentoViewModel
        {
            Id = agendamento.Id,
            PetId = agendamento.PetId,
            Data = agendamento.Data,
            Horario = agendamento.Horario,
            Observacao = agendamento.Observacao,
            TaxiDog = agendamento.TaxiDog,
            ClienteTraz = agendamento.ClienteTraz,
            ServicoIds = agendamento.AgendamentoServicos.Select(s => s.ServicoId).ToList(),
            HorarioFixoSemanal = isFixo
        };

        ViewBag.IsFixo = isFixo;
        var petsComCliente = _context.Pets
            .Include(p => p.Cliente) // garante que traz o Cliente relacionado
            .Select(p => new
            {
                Id = p.Id,
                NomeCompleto = p.Nome + " - " + p.Cliente.Nome
            })
            .ToList();

        ViewBag.Pets = new SelectList(petsComCliente, "Id", "NomeCompleto");
        ViewBag.Servicos = _context.Servicos.ToList();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(EditarAgendamentoViewModel model, int[] servicoIds)
    {
        try
        {
            var agendamento = _context.Agendamentos
                .Include(a => a.AgendamentoServicos)
                .FirstOrDefault(a => a.Id == model.Id);

            if (agendamento == null)
                return NotFound();

            // Verifica se esse agendamento já era fixo
            var fixoAnterior = _context.AgendamentosFixos.FirstOrDefault(f =>
                f.PetId == agendamento.PetId &&
                f.DiaSemana == agendamento.Data.DayOfWeek &&
                f.Horario == agendamento.Horario);

            // Se agora NÃO é fixo, mas antes era: remover o fixo antigo
            if (!model.HorarioFixoSemanal && fixoAnterior != null)
            {
                var servicosFixo = _context.AgendamentoFixoServicos
                    .Where(s => s.AgendamentoFixoId == fixoAnterior.Id).ToList();

                _context.AgendamentoFixoServicos.RemoveRange(servicosFixo);
                _context.AgendamentosFixos.Remove(fixoAnterior);
                _context.SaveChanges();
            }

            // Se agora É fixo
            if (model.HorarioFixoSemanal)
            {
                var fixoExistente = _context.AgendamentosFixos.FirstOrDefault(f =>
                    f.PetId == model.PetId &&
                    f.DiaSemana == model.Data.DayOfWeek &&
                    f.Horario == model.Horario);

                if (fixoExistente != null)
                {
                    fixoExistente.TaxiDog = model.TaxiDog;
                    fixoExistente.ClienteTraz = model.ClienteTraz;
                    fixoExistente.Observacao = model.Observacao;

                    var servicosAntigos = _context.AgendamentoFixoServicos
                        .Where(s => s.AgendamentoFixoId == fixoExistente.Id).ToList();
                    _context.AgendamentoFixoServicos.RemoveRange(servicosAntigos);

                    foreach (var servicoId in servicoIds)
                    {
                        _context.AgendamentoFixoServicos.Add(new AgendamentoFixoServico
                        {
                            AgendamentoFixoId = fixoExistente.Id,
                            ServicoId = servicoId
                        });
                    }

                    _context.SaveChanges();
                }
                else
                {
                    var novoFixo = new AgendamentoFixo
                    {
                        PetId = model.PetId,
                        DiaSemana = model.Data.DayOfWeek,
                        Horario = model.Horario,
                        TaxiDog = model.TaxiDog,
                        ClienteTraz = model.ClienteTraz,
                        Observacao = model.Observacao
                    };

                    _context.AgendamentosFixos.Add(novoFixo);
                    _context.SaveChanges();

                    foreach (var servicoId in servicoIds)
                    {
                        _context.AgendamentoFixoServicos.Add(new AgendamentoFixoServico
                        {
                            AgendamentoFixoId = novoFixo.Id,
                            ServicoId = servicoId
                        });
                    }

                    _context.SaveChanges();
                }
            }

            // Atualiza o agendamento normal
            agendamento.PetId = model.PetId;
            agendamento.Data = model.Data;
            agendamento.Horario = model.Horario;
            agendamento.TaxiDog = model.TaxiDog;
            agendamento.ClienteTraz = model.ClienteTraz;
            agendamento.Observacao = model.Observacao;

            var servicosAntigosNormais = _context.AgendamentoServicos
                .Where(s => s.AgendamentoId == agendamento.Id).ToList();

            _context.AgendamentoServicos.RemoveRange(servicosAntigosNormais);
            _context.SaveChanges();

            foreach (var servicoId in servicoIds)
            {
                _context.AgendamentoServicos.Add(new AgendamentoServico
                {
                    AgendamentoId = agendamento.Id,
                    ServicoId = servicoId
                });
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            var petsComCliente = _context.Pets
                .Include(p => p.Cliente)
                .Select(p => new
                {
                    Id = p.Id,
                    NomeCompleto = p.Nome + " - " + p.Cliente.Nome
                })
                .ToList();

            ViewBag.Pets = new SelectList(petsComCliente, "Id", "NomeCompleto");
            ViewBag.Servicos = _context.Servicos.ToList();

            return View(model);
        }
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var agendamento = await _context.Agendamentos
            .Include(a => a.Pet)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(asg => asg.Servico)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (agendamento == null) return NotFound();

        return View(agendamento);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var agendamento = await _context.Agendamentos
            .Include(a => a.AgendamentoServicos)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (agendamento == null)
        {
            return NotFound();
        }
        
        // Remove os relacionamentos com os serviços primeiro
        _context.AgendamentoServicos.RemoveRange(agendamento.AgendamentoServicos);

        // Remove o agendamento
        _context.Agendamentos.Remove(agendamento);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    
    public IActionResult CadastrarClienteEPetRapido(
    string NomeCliente,
    string Telefone,
    string Endereco,
    string NomePet)
    {
        
        var cliente = new Cliente
        {
            Nome = NomeCliente,
            Telefone1 = Telefone,
            Endereco1 = Endereco
        };
        _context.Clientes.Add(cliente);
        _context.SaveChanges();

        var pet = new Pet
        {
            Nome = NomePet,
            ClienteId = cliente.Id
            
        };
        _context.Pets.Add(pet);
        _context.SaveChanges();

        var petsComCliente = _context.Pets
            .Include(p => p.Cliente) // garante que traz o Cliente relacionado
            .Select(p => new
            {
                Id = p.Id,
                NomeCompleto = p.Nome + " - " + p.Cliente.Nome
            })
            .ToList();

        ViewBag.Pets = new SelectList(petsComCliente, "Id", "NomeCompleto");
        ViewBag.Servicos = _context.Servicos.ToList();

        var novoAgendamento = new Agendamento
        {
            PetId = pet.Id,
            Data = DateTime.Today
        };

        return View("Create", novoAgendamento);
    }



}