using Microsoft.EntityFrameworkCore;
using PlanningPoker.Api.Models;

namespace PlanningPoker.Api.Data
{
    public class PlanningPokerContext : DbContext
    {
        public PlanningPokerContext(DbContextOptions<PlanningPokerContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<Player> Players { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Player>().HasKey(m => m.Id);

            base.OnModelCreating(builder);
        }

        public void InitializeDatabase()
        {
            Database.Migrate();

            Rooms.Add(new Room() { Reveal = false });

            SaveChanges();
        }
    }
}
