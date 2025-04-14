using Microsoft.EntityFrameworkCore;

public interface ICatDBContext: ISaveChanges
{
    DbSet<CatEntity> Cats { get; }
}

public interface ITagDBContext : ISaveChanges
{
    DbSet<TagEntity> Tags { get; }
}

public interface ICatTagDBContext : ISaveChanges
{
    DbSet<CatTag> CatTags { get; }
}

public interface ISaveChanges
{
    Task<int> SaveAsync();
}

public class ApplicationDbContext : DbContext, ICatDBContext, ITagDBContext, ICatTagDBContext
{
    public string guid = Guid.NewGuid().ToString();
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<CatEntity> Cats => Set<CatEntity>();
    public DbSet<TagEntity> Tags => Set<TagEntity>();
    public DbSet<CatTag> CatTags => Set<CatTag>();


    //// Explicit implementation for interface usage
    //Task<int> ICatDBContext.SaveChangesAsync(CancellationToken cancellationToken)
    //    => base.SaveChangesAsync(cancellationToken);

    //Task<int> ITagDbContext.SaveChangesAsync(CancellationToken cancellationToken)
    //    => base.SaveChangesAsync(cancellationToken);
    public async Task<int> SaveAsync() => await base.SaveChangesAsync();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatTag>()
            .HasKey(ct => new { ct.CatEntityId, ct.TagEntityId });

        modelBuilder.Entity<CatTag>()
            .HasOne(ct => ct.Cat)
            .WithMany(c => c.CatTags)
            .HasForeignKey(ct => ct.CatEntityId);

        modelBuilder.Entity<CatTag>()
            .HasOne(ct => ct.Tag)
            .WithMany(t => t.CatTags)
            .HasForeignKey(ct => ct.TagEntityId);

        // Optional: enforce tag name uniqueness
        modelBuilder.Entity<TagEntity>()
            .HasIndex(t => t.Name)
            .IsUnique();
    }
}