using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using petmypet.Context;
using petmypet.Models;

namespace petmypet.Controllers
{
    public class AgendamentosFixosController : Controller
    {
        private readonly AppDbContext _context;

        public AgendamentosFixosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string busca, string statusFiltro)
        {
            var agendamentos = await _context.AgendamentosFixos
                .Include(a => a.Pet)
                    .ThenInclude(p => p.Cliente)
                .Include(a => a.Servicos)
                    .ThenInclude(s => s.Servico)
                .ToListAsync();

            var hoje = DateTime.Today;

            // Filtro por nome do pet ou tutor
            if (!string.IsNullOrWhiteSpace(busca))
            {
                busca = busca.ToLower();
                agendamentos = agendamentos
                    .Where(a =>
                        (!string.IsNullOrEmpty(a.Pet?.Nome) && a.Pet.Nome.ToLower().Contains(busca)) ||
                        (!string.IsNullOrEmpty(a.Pet?.Cliente?.Nome) && a.Pet.Cliente.Nome.ToLower().Contains(busca))
                    ).ToList();
            }

            // Filtro por status de pagamento
            if (!string.IsNullOrWhiteSpace(statusFiltro) && statusFiltro != "Todos")
            {
                agendamentos = agendamentos.Where(a =>
                {
                    if (!a.DataProximoVencimento.HasValue)
                        return statusFiltro == "Sem Informação";

                    var data = a.DataProximoVencimento.Value.Date;

                    return (statusFiltro == "Em Dia" && data > hoje)
                        || (statusFiltro == "Vencido" && data < hoje)
                        || (statusFiltro == "Vence Hoje" && data == hoje);
                }).ToList();
            }

            var agendamentosVencidos = agendamentos.Where(a => a.DataProximoVencimento.HasValue && a.DataProximoVencimento.Value.Date < hoje).ToList();

            decimal valorRecebido = 0;
            decimal valorTotalMes = 0;

            foreach (var agendamento in agendamentos)
            {
                decimal valorAgendamento = agendamento.Servicos?.Sum(s => s.Servico?.Valor ?? 0) ?? 0;
                valorTotalMes += valorAgendamento;

                if (agendamento.DataUltimoPagamento.HasValue &&
                    agendamento.DataUltimoPagamento.Value.Month == hoje.Month &&
                    agendamento.DataUltimoPagamento.Value.Year == hoje.Year)
                {
                    valorRecebido += valorAgendamento;
                }
            }

            ViewBag.QtdAgendamentos = agendamentos.Count;
            ViewBag.QtdVencidos = agendamentosVencidos.Count;
            ViewBag.ValorRecebido = valorRecebido;
            ViewBag.ValorFaltante = Math.Max(0, valorTotalMes - valorRecebido);

            return View(agendamentos);
        }





        // GET: AgendamentosFixos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var agendamento = await _context.AgendamentosFixos
                .Include(a => a.Pet)
                .Include(a => a.Servicos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (agendamento == null) return NotFound();

            return View(agendamento);
        }

        // GET: AgendamentosFixos/Create
        public IActionResult Create()
        {
            ViewData["PetId"] = new SelectList(_context.Pets, "Id", "Nome");
            return View();
        }

        // POST: AgendamentosFixos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AgendamentoFixo agendamento)
        {

            var teste = "Teste";

            try
            {
                _context.Add(agendamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar agendamento: {ex.Message}");
                ViewData["PetId"] = new SelectList(_context.Pets, "Id", "Nome", agendamento.PetId);
                return View(agendamento);
            }
        }

        // GET: AgendamentosFixos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var agendamento = await _context.AgendamentosFixos
                .Include(a => a.Pet)
                .Include(a => a.Servicos)
                    .ThenInclude(s => s.Servico)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agendamento == null) return NotFound();

            ViewData["PetId"] = new SelectList(_context.Pets, "Id", "Nome", agendamento.PetId);
            ViewData["Servicos"] = await _context.Servicos
                .Where(s => s.Ativo)
                .ToListAsync();

            return View(agendamento);
        }


        // POST: AgendamentosFixos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AgendamentoFixo agendamento, int[] servicosSelecionados)
        {
            if (id != agendamento.Id) return NotFound();

            try
            {
                var agendamentoExistente = await _context.AgendamentosFixos
                    .Include(a => a.Servicos)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (agendamentoExistente == null) return NotFound();

                // Atualiza propriedades principais
                agendamentoExistente.DiaSemana = agendamento.DiaSemana;
                agendamentoExistente.Horario = agendamento.Horario;
                agendamentoExistente.PetId = agendamento.PetId;
                agendamentoExistente.TaxiDog = agendamento.TaxiDog;
                agendamentoExistente.ClienteTraz = agendamento.ClienteTraz;
                agendamentoExistente.DiaVencimentoPacote = agendamento.DiaVencimentoPacote;

                agendamentoExistente.DiaVencimentoPacote = agendamento.DiaVencimentoPacote;

                // Aqui entra a lógica para DataProximoVencimento:
                var hoje = DateTime.Today;
                var ultimoPagamento = agendamentoExistente.DataUltimoPagamento;
                var novoDiaVencimento = agendamento.DiaVencimentoPacote ?? hoje.Day;

                bool mesAtualFoiPago = ultimoPagamento != null &&
                                       ultimoPagamento.Value.Month == hoje.Month &&
                                       ultimoPagamento.Value.Year == hoje.Year;

                if (!mesAtualFoiPago)
                {
                    agendamentoExistente.DataProximoVencimento = new DateTime(
                        hoje.Year,
                        hoje.Month,
                        Math.Min(novoDiaVencimento, DateTime.DaysInMonth(hoje.Year, hoje.Month))
                    );
                }
                else
                {
                    var proximoMes = hoje.AddMonths(1);
                    agendamentoExistente.DataProximoVencimento = new DateTime(
                        proximoMes.Year,
                        proximoMes.Month,
                        Math.Min(novoDiaVencimento, DateTime.DaysInMonth(proximoMes.Year, proximoMes.Month))
                    );
                }

                // Atualiza serviços
                _context.AgendamentoFixoServicos.RemoveRange(agendamentoExistente.Servicos);

                agendamentoExistente.Servicos = servicosSelecionados
                    .Select(sid => new AgendamentoFixoServico
                    {
                        AgendamentoFixoId = agendamento.Id,
                        ServicoId = sid
                    }).ToList();

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao editar agendamento fixo: " + ex.Message);
            }

            ViewData["PetId"] = new SelectList(_context.Pets, "Id", "Nome", agendamento.PetId);
            ViewData["Servicos"] = await _context.Servicos.ToListAsync();
            return View(agendamento);
        }


        // GET: AgendamentosFixos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var agendamento = await _context.AgendamentosFixos
                .Include(a => a.Pet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (agendamento == null) return NotFound();

            return View(agendamento);
        }

        // POST: AgendamentosFixos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var agendamento = await _context.AgendamentosFixos.FindAsync(id);
                if (agendamento != null)
                {
                    _context.AgendamentosFixos.Remove(agendamento);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao excluir agendamento: {ex.Message}");
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: AgendamentosFixos/ConfirmarPagamento/5
        public async Task<IActionResult> ConfirmarPagamento(int? id)
        {
            if (id == null) return NotFound();

            var agendamento = await _context.AgendamentosFixos
                .Include(a => a.Pet)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agendamento == null) return NotFound();

            return View(agendamento);
        }

        // POST: AgendamentosFixos/ConfirmarPagamento/5
        // POST: AgendamentosFixos/ConfirmarPagamento/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarPagamentoPost(int id)
        {
            var agendamento = await _context.AgendamentosFixos.FindAsync(id);

            if (agendamento == null) return NotFound();

            agendamento.PacoteEmDia = true;
            agendamento.DataUltimoPagamento = DateTime.Today;

            if (agendamento.DataProximoVencimento.HasValue)
            {
                // Já tem vencimento, adiciona 1 mês
                agendamento.DataProximoVencimento = agendamento.DataProximoVencimento.Value.AddMonths(1);
            }
            else
            {
                // Se não tiver vencimento anterior, define como 1 mês a partir de hoje
                agendamento.DataProximoVencimento = DateTime.Today.AddMonths(1);
            }

            // Registrar o pagamento no histórico
            var pagamento = new PagamentoPacote
            {
                AgendamentoFixoId = agendamento.Id,
                DataPagamento = DateTime.Today,
                ReferenteAoMes = DateTime.Today, // Pode ajustar aqui se quiser marcar o mês anterior
                ValorPago = null // Se quiser permitir informar, depois a gente altera
            };

            _context.PagamentosPacotes.Add(pagamento);
            _context.AgendamentosFixos.Update(agendamento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool AgendamentoExists(int id)
        {
            return _context.AgendamentosFixos.Any(e => e.Id == id);
        }
    }
}
