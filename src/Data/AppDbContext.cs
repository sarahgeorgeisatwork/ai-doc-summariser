using AiDocSummariser.Models;
using Microsoft.EntityFrameworkCore;

namespace AiDocSummariser.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Summary> Summaries => Set<Summary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>().HasKey(d => d.Id);
        modelBuilder.Entity<Summary>().HasKey(s => s.Id);
        modelBuilder.Entity<Summary>()
            .HasOne<Document>()
            .WithMany()
            .HasForeignKey(s => s.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}