using Microsoft.EntityFrameworkCore;

namespace WebApp.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        public DbSet<Skill> Skills { get; set; }
        
        public ApplicationContext(DbContextOptions<ApplicationContext> options):base(options) {  }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasMany(p => p.Skills)
                .WithMany()
                .UsingEntity(j => j.ToTable("PersonSkills"));
        }
    }
}
