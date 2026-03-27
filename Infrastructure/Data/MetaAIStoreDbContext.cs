using SemantiCore_API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SemantiCore_API.Infrastructure.Data
{
    public class SemantiCoreDbContext : DbContext
    {
        public SemantiCoreDbContext(DbContextOptions<SemantiCoreDbContext> options)
            : base(options)
        {
        }

        // ===== DbSets =====
        public DbSet<IndexCategory> IndexCategories { get; set; }
        public DbSet<CategoryIndex> CategoryIndexes { get; set; }
        public DbSet<IndexedDocument> IndexedDocuments { get; set; }
        public DbSet<DocumentText> DocumentTexts { get; set; }
        public DbSet<DocumentChunk> DocumentChunks { get; set; }
        public DbSet<DocumentEmbedding> DocumentEmbeddings { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<DocumentChunk>()
                .HasOne(c => c.IndexedDocument)
                .WithMany(d => d.DocumentChunks)
                .HasForeignKey(c => c.IndexedDocumentId);

            modelBuilder.Entity<DocumentEmbedding>()
                .HasOne(e => e.DocumentChunk)
                .WithMany(c => c.DocumentEmbeddings)
                .HasForeignKey(e => e.DocumentChunkId);

            base.OnModelCreating(modelBuilder);

            // =========================
            // IndexCategory
            // =========================
            modelBuilder.Entity<IndexCategory>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.CategoryName)
                      .IsRequired()
                      .HasMaxLength(100);

                // Category name must be unique
                entity.HasIndex(e => e.CategoryName)
                      .IsUnique();
            });

            // =========================
            // CategoryIndex
            // =========================
            modelBuilder.Entity<CategoryIndex>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.IndexName)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.IndexType)
                      .IsRequired();

                // FK: CategoryIndex → IndexCategory
                entity.HasOne(e => e.IndexCategory)
                      .WithMany(c => c.CategoryIndexes)
                      .HasForeignKey(e => e.IndexCategoryId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Prevent duplicate index names inside same category
                entity.HasIndex(e => new { e.IndexCategoryId, e.IndexName })
                      .IsUnique();
            });

            // =========================
            // IndexedDocument
            // =========================
            modelBuilder.Entity<IndexedDocument>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.OriginalFileName)
                      .IsRequired();

                entity.Property(e => e.StoredFilePath)
                      .IsRequired();

                // FK: IndexedDocument → CategoryIndex
                entity.HasOne(e => e.CategoryIndex)
                      .WithMany()
                      .HasForeignKey(e => e.CategoryIndexId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<DocumentText>()
                .HasOne<IndexedDocument>()
                .WithMany()
                .HasForeignKey(d => d.IndexedDocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DocumentChunk>()
                .HasIndex(c => new { c.IndexedDocumentId, c.ChunkOrder });

            modelBuilder.Entity<DocumentEmbedding>()
                .HasIndex(e => e.DocumentChunkId)
                .IsUnique();
        }   
    }
}
