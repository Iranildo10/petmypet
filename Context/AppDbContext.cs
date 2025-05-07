using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using petmypet.Models;

namespace petmypet.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Pet> Pets { get; set; }
        public DbSet<TrabalhoPronto> TrabalhosProntos { get; set; }
        
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<AgendamentoServico> AgendamentoServicos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<AgendamentoFixo> AgendamentosFixos { get; set; }
        public DbSet<AgendamentoFixoServico> AgendamentoFixoServicos { get; set; }
        public DbSet<AgendamentoFixoExcecao> AgendamentoFixoExcecoes { get; set; }
        public DbSet<PagamentoPacote> PagamentosPacotes { get; set; }

    }
}
