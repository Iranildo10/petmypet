using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using petmypet.Context;
using petmypet.Models;
using System.Linq;
using System.Threading.Tasks;

namespace petmypet.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")]
    public class AdminClientesController : Controller
    {
        private readonly AppDbContext _context;

        public AdminClientesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes
                .Include(c => c.Pets)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Telefone1,Telefone2,Endereco1,Endereco2")] Cliente cliente)
        {

           
            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Telefone1,Telefone2,Endereco1,Endereco2")] Cliente cliente)
        {
            if (id != cliente.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Pets)
                    .ThenInclude(p => p.Agendamentos)
                        .ThenInclude(a => a.AgendamentoServicos)
                .Include(c => c.Pets)
                    .ThenInclude(p => p.AgendamentosFixos)
                        .ThenInclude(af => af.Servicos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
                return NotFound();

            foreach (var pet in cliente.Pets)
            {
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

                // Deletar o pet
                _context.Pets.Remove(pet);
            }

            // Finalmente, remover o cliente
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
