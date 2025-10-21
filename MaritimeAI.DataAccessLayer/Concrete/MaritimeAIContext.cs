using MaritimeAI.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace MaritimeAI.DataAccessLayer.Context
{
    public class MaritimeAIContext : DbContext
    {
        public MaritimeAIContext(DbContextOptions<MaritimeAIContext> options) : base(options)
        {
            //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            //optionsBuilder.UseSqlServer("Server=host.docker.internal;Database=MaritimeAI;User Id=dockeruser;Password=Docker123!;TrustServerCertificate=True;");
        }
        public DbSet<User> Users { get; set; }
    }
}