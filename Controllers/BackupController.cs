using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using petmypet.Context;
using petmypet.Models;
using System.Diagnostics;

namespace petmypet.Controllers
{
    public class BackupController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BackupController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var backups = _context.BackupHistories
                .OrderByDescending(b => b.BackupDate)
                .ToList();

            return View(backups);
        }

        [HttpPost]
        public IActionResult CreateBackup(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return BadRequest("Caminho da pasta inválido.");

            var fileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
            var fullPath = Path.Combine(folderPath, fileName);

            var mySqlDumpPath = @"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe";
            var connectionString = _context.Database.GetDbConnection().ConnectionString;

            var builder = new MySqlConnectionStringBuilder(connectionString);
            var arguments = $"-u{builder.UserID} -p{builder.Password} {builder.Database}";

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = mySqlDumpPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                using (var process = Process.Start(psi))
                using (var streamReader = process.StandardOutput)
                using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(streamReader.ReadToEnd());
                }

                var fileInfo = new FileInfo(fullPath);

                var history = new BackupHistory
                {
                    FolderPath = folderPath,
                    FileName = fileName,
                    FileSize = fileInfo.Length,
                    BackupDate = DateTime.Now
                };

                _context.BackupHistories.Add(history);
                _context.SaveChanges();

                TempData["Success"] = "Backup criado com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Erro ao criar backup: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        


    }
}
