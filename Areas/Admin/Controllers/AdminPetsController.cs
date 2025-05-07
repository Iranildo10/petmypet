using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using petmypet.Context;

namespace petmypet.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize("Admin")]
    public class AdminPetsController : Controller
    {
        private readonly AppDbContext _context;

        public AdminPetsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pets = _context.Pets.Include(p => p.Cliente);
            return View(await pets.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pet = await _context.Pets
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pet == null) return NotFound();

            return View(pet);
        }

        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,ClienteId,Observacao")] Pet pet)
        {

            if (ModelState.IsValid)
            {
                _context.Add(pet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", pet.ClienteId);
            return View(pet);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var pet = await _context.Pets.FindAsync(id);
            if (pet == null) return NotFound();

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", pet.ClienteId);
            return View(pet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,ClienteId,Observacao")] Pet pet)
        {
            if (id != pet.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Pets.Any(e => e.Id == pet.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", pet.ClienteId);
            return View(pet);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var pet = await _context.Pets
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pet == null) return NotFound();

            return View(pet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await _context.Pets
                .Include(p => p.Agendamentos)
                    .ThenInclude(a => a.AgendamentoServicos)
                .Include(p => p.AgendamentosFixos)
                    .ThenInclude(af => af.Servicos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet == null)
                return NotFound();

            // Deletar Agendamentos e seus serviços
            foreach (var agendamento in pet.Agendamentos)
            {
                _context.AgendamentoServicos.RemoveRange(agendamento.AgendamentoServicos);
                _context.Agendamentos.Remove(agendamento);
            }

            // Deletar Agendamentos Fixos e seus serviços
            foreach (var agendamentoFixo in pet.AgendamentosFixos)
            {
                _context.AgendamentoFixoServicos.RemoveRange(agendamentoFixo.Servicos);
                _context.AgendamentosFixos.Remove(agendamentoFixo);
            }

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
