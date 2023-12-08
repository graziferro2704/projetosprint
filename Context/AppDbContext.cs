using Microsoft.EntityFrameworkCore;
using App.Models;

namespace App.Context

{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Usuario> Usuarios { get; set;}
        public DbSet<Banner> Banners {get; set;}
    }
}