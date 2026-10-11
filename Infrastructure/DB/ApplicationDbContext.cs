using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CourseDocument> CourseDocuments => Set<CourseDocument>();
    public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

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

        modelBuilder.Entity<ChatMessage>(builder =>
        {
            builder.Property(m => m.UserId).IsRequired().HasMaxLength(450);
            builder.Property(m => m.Role).IsRequired().HasMaxLength(20);
            builder.Property(m => m.Text).IsRequired();
            builder.HasIndex(m => m.UserId);

            builder.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}