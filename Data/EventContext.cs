using ite4160.Models;
using Microsoft.EntityFrameworkCore;

namespace ite4160.Data
{
    public class EventContext : DbContext
    {
        public EventContext(DbContextOptions<EventContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Call> Calls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Call>()
                .HasMany(c => c.Events)
                .WithOne(e => e.Call)
                .IsRequired();

            modelBuilder
                .Entity<Call>()
                .Property(m => m.Receiver)
                .IsRequired(false);

            base.OnModelCreating(modelBuilder);
        }
    }

}