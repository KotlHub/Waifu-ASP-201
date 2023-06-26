using ASP_201.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace ASP_201.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Entity.Waifu> WaifuHusbanduTable { get; set; }
        public DbSet<Entity.User> Users { get; set; }
        public DbSet<Entity.EmailConfirmToken> EmailConfirmTokens { get; set; }

        public DataContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
