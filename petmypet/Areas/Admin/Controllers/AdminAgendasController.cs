using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using petmypet.Context;
using petmypet.Models;

namespace petmypet.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")]
    public class AdminAgendasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotyfService _notyf;

        public AdminAgendasController(AppDbContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        public async Task<IActionResult> Index()
        {
            var agendas = await _context.Agendas
                .Include(a => a.HorariosAgenda) // Inclui os horários associados
                .ToListAsync();

            return View(agendas);
        }

        // GET: Admin/AdminAgendas/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminAgendas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Agenda agenda)
        {

            ModelState.Remove("HorariosAgenda");

            if (ModelState.IsValid)
            {
                // Adiciona a agenda ao banco de dados
                _context.Agendas.Add(agenda);
                await _context.SaveChangesAsync();

                // Gera os horários baseados nos dados fornecidos
                List<HorarioAgenda> horarios = new List<HorarioAgenda>();

                // Horário inicial e final da agenda
                TimeSpan horarioAtual = agenda.HorarioInicial;
                TimeSpan horarioFinal = agenda.HorarioFinal;

                // Intervalo, se houver
                TimeSpan inicioIntervalo = agenda.InicioIntervalo;
                TimeSpan fimIntervalo = agenda.FimIntervalo;

                // Incrementa horários com base na duração
                while (horarioAtual < horarioFinal)
                {
                    // Verifica se o horário atual está fora do intervalo
                    if (horarioAtual < inicioIntervalo || horarioAtual >= fimIntervalo)
                    {
                        // Adiciona o horário na lista
                        horarios.Add(new HorarioAgenda
                        {
                            Horario = horarioAtual,
                            AgendaId = agenda.Id
                        });
                    }

                    // Incrementa o horário com base na duração
                    horarioAtual = horarioAtual.Add(TimeSpan.FromMinutes(agenda.DuracaoHorario));
                }

                // Adiciona os horários no banco de dados
                _context.HorariosAgendas.AddRange(horarios);
                await _context.SaveChangesAsync();
                _notyf.Success("Agenda criada com sucesso");
                return RedirectToAction(nameof(Index)); // Redireciona para a listagem de agendas
            }

            _notyf.Warning("Verifique os campos");
            return View(agenda); // Se inválido, retorna o formulário com os erros
        }

        // GET: Edit (Exibe o formulário para edição)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Agendas == null)
                return NotFound();

            var agenda = await _context.Agendas.FindAsync(id);
            if (agenda == null)
                return NotFound();

            return View(agenda);
        }

        // POST: Edit (Recebe os dados atualizados e salva no banco)
        // Edit no AgendaController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Seg,Ter,Qua,Qui,Sex,Sab,Dom,HorarioInicial,HorarioFinal,DuracaoHorario,InicioIntervalo,FimIntervalo,Ativo")] Agenda agenda)
        {
            if (id != agenda.Id)
                return NotFound();

            // Remove o estado inválido para HorariosAgenda
            ModelState.Remove("HorariosAgenda");

            if (ModelState.IsValid)
            {
                try
                {
                    // Remove os horários existentes para esta agenda
                    var horariosExistentes = _context.HorariosAgendas
                        .Where(h => h.AgendaId == agenda.Id);

                    _context.HorariosAgendas.RemoveRange(horariosExistentes);

                    // Salva as alterações no banco
                    await _context.SaveChangesAsync();

                    // Recalcula os horários com base na lógica do método Create
                    TimeSpan horarioAtual = agenda.HorarioInicial;

                    while (horarioAtual < agenda.HorarioFinal)
                    {
                        // Verifica se está dentro do intervalo de trabalho
                        if (horarioAtual < agenda.InicioIntervalo || horarioAtual >= agenda.FimIntervalo)
                        {
                            // Adiciona novo horário
                            _context.HorariosAgendas.Add(new HorarioAgenda
                            {
                                AgendaId = agenda.Id,
                                Horario = horarioAtual
                            });
                        }

                        // Incrementa pelo tempo de duração
                        horarioAtual = horarioAtual.Add(TimeSpan.FromMinutes(agenda.DuracaoHorario));
                    }

                    // Atualiza a agenda no banco
                    _context.Update(agenda);
                    await _context.SaveChangesAsync();

                    _notyf.Success("Agenda editada com sucesso");

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgendaExists(agenda.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            _notyf.Warning("Verifique os campos");
            return View(agenda);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Busca a agenda pelo ID
            var agenda = await _context.Agendas
                .Include(a => a.HorariosAgenda) // Inclui os horários relacionados
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agenda == null)
            {
                return NotFound();
            }

            try
            {
                // Remove os horários relacionados
                _context.HorariosAgendas.RemoveRange(agenda.HorariosAgenda);

                // Remove a agenda
                _context.Agendas.Remove(agenda);

                // Salva as alterações no banco
                await _context.SaveChangesAsync();
                _notyf.Success("Agenda excluída com sucesso");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Tratar possíveis exceções, se necessário, e exibir uma mensagem ao usuário
                ModelState.AddModelError("", $"Erro ao excluir a agenda: {ex.Message}");
                return View("Error");
            }
        }


        private bool AgendaExists(int id)
        {
            return _context.Agendas.Any(e => e.Id == id);
        }

    }
}
