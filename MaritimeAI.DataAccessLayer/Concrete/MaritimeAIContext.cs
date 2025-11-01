using MaritimeAI.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace MaritimeAI.DataAccessLayer.Context
{
    public class MaritimeAIContext : DbContext
    {
        public MaritimeAIContext(DbContextOptions<MaritimeAIContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}