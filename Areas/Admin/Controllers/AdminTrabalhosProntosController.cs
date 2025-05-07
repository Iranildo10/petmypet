using System;
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
    public class AdminTrabalhosProntosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotyfService _notyf;

        public AdminTrabalhosProntosController(AppDbContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // LISTAR: GET /TrabalhosProntos
        public async Task<IActionResult> Index()
        {
            var trabalhos = await _context.TrabalhosProntos.ToListAsync();
            
            return View(trabalhos);
        }

        // CREATE: GET /TrabalhosProntos/Create
        public IActionResult Create()
        {
            var trabalhoPronto = new TrabalhoPronto
            {
                ImagemUrl = "/images/semimagem.jpg", // Caminho da imagem padrão
                Ativo = true
            };

            return View(trabalhoPronto);
        }


        // CREATE: POST /TrabalhosProntos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrabalhoPronto trabalhoPronto, IFormFile? imageInput = null)
        {

            bool teste = trabalhoPronto.Ativo;

            if (ModelState.IsValid)
            {
                // Se não houver imagem, define o caminho para a imagem padrão
                if (imageInput != null && imageInput.Length > 0)
                {
                    // Processa a imagem e converte para Base64
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageInput.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        string base64String = Convert.ToBase64String(imageBytes);
                        trabalhoPronto.ImagemUrl = "data:image/png;base64," + base64String; // Ou o tipo correto
                    }
                }
                else
                {
                    // Caso não haja imagem, define a imagem padrão
                    trabalhoPronto.ImagemUrl = "/images/semimagem.jpg";
                   
                }

                _context.Add(trabalhoPronto);
                await _context.SaveChangesAsync();
                _notyf.Success("Cadastrado com sucesso");
                return RedirectToAction(nameof(Index));
            }

            
            return View(trabalhoPronto);
        }


        // EDITAR: GET /TrabalhosProntos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trabalhoPronto = await _context.TrabalhosProntos.FindAsync(id);

            if (trabalhoPronto == null)
            {
                return NotFound();
            }

            return View(trabalhoPronto);
        }

        // EDITAR: POST /TrabalhosProntos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrabalhoPronto trabalhoPronto, IFormFile? imageInput = null)
        {
            if (id != trabalhoPronto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
               
                // Verifica se foi selecionada uma nova imagem
                if (imageInput != null && imageInput.Length > 0)
                {
                    // Se houver uma nova imagem, processa e converte para Base64
                    using (var memoryStream = new MemoryStream()) // Corrigido para MemoryStream
                    {
                        await imageInput.CopyToAsync(memoryStream); // Corrigido para CopyToAsync
                        byte[] imageBytes = memoryStream.ToArray(); // Corrigido para ToArray
                        string base64String = Convert.ToBase64String(imageBytes); // Corrigido para Convert.ToBase64String
                        trabalhoPronto.ImagemUrl = "data:image/png;base64," + base64String; // Ou o tipo correto
                    }
                }

                // Atualiza o trabalho no banco de dados
                _context.Update(trabalhoPronto);
                await _context.SaveChangesAsync();
                _notyf.Success("Editado com sucesso");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _notyf.Information("Verifique os campos");
            }

            // Retorna a view com o modelo
            return View(trabalhoPronto);
        }


        // EXCLUIR: GET /TrabalhosProntos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trabalhoPronto = await _context.TrabalhosProntos.FirstOrDefaultAsync(m => m.Id == id);

            if (trabalhoPronto == null)
            {
                return NotFound();
            }

            return View(trabalhoPronto);
        }

        // EXCLUIR: POST /TrabalhosProntos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trabalhoPronto = await _context.TrabalhosProntos.FindAsync(id);

            if (trabalhoPronto != null)
            {
                _context.TrabalhosProntos.Remove(trabalhoPronto);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Trabalho excluído com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TrabalhoProntoExists(int id)
        {
            return _context.TrabalhosProntos.Any(e => e.Id == id);
        }
    }
}
