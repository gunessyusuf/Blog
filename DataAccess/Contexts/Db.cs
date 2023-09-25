using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class Db : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    public DbSet<Tag> Tags { get; set; }

    public DbSet<BlogTag> BlogTags { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public Db(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // many to many ilişki için
        modelBuilder.Entity<BlogTag>().HasKey(bt => new { bt.BlogId, bt.TagId });
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     var connectionString = "server=(localdb)\\mssqllocaldb;database=BlogDB;trusted_connection=true;multipleactiveresultsets=true;trustservercertificate=true;";
    //     optionsBuilder.UseSqlServer(connectionString);
    // }
}
