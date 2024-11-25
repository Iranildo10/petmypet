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
        public DbSet<Raca> Racas { get; set; }
        public DbSet<TipoAnimal> TiposAnimais { get; set; }
    }
}
