using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CourseDocument> CourseDocuments => Set<CourseDocument>();
    public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CourseDocument>(builder =>
        {
            builder.Property(d => d.Title).IsRequired().HasMaxLength(200);
            builder.Property(d => d.OriginalFileName).IsRequired().HasMaxLength(260);
            builder.Property(d => d.StoredFileName).IsRequired().HasMaxLength(100);
            builder.Property(d => d.ContentType).IsRequired().HasMaxLength(100);
            builder.HasIndex(d => d.StoredFileName).IsUnique();

            builder.HasMany(d => d.Chunks)
                .WithOne(c => c.CourseDocument)
                .HasForeignKey(c => c.CourseDocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DocumentChunk>(builder =>
        {
            builder.Property(c => c.Text).IsRequired();
            builder.HasIndex(c => new { c.CourseDocumentId, c.ChunkIndex }).IsUnique();
        });
    }
}