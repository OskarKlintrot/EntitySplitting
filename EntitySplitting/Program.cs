using EntitySplitting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using static System.Console;

using (var dbContext = new BloggingContext())
{
    await BloggingContext.InitializeAsync(dbContext);
}

using (var dbContext = new BloggingContext())
{
    dbContext.Log = WriteLine;

    dbContext.Blogs.Add(
        new Blog
        {
            Url = "https://blog.google.com",
            Posts =
            {
                new Post { Title = "Hello", Content = "world!" }
            }
        }
    );

    await dbContext.SaveChangesAsync();
}

using (var dbContext = new BloggingContext())
{
    dbContext.Log = WriteLine;

    var blog = await dbContext.Blogs.Include(x => x.Posts).SingleAsync();

    WriteLine($"Blog {blog.Url} contains {blog.Posts.Count} posts.");
}

namespace EntitySplitting
{
    public class BloggingContext : DbContext
    {
        public Action<string>? Log { get; set; }
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EntitySplitting;Trusted_Connection=True;MultipleActiveResultSets=true"
                )
                .LogTo(s => Log?.Invoke(s), LogLevel)
                .ConfigureWarnings(
                    config =>
                        config.Ignore(RelationalEventId.ForeignKeyPropertiesMappedToUnrelatedTables)
                );

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BloggingContext).Assembly);
        }

        public static async Task InitializeAsync(BloggingContext context)
        {
            await context.Database.EnsureDeletedAsync();

            await context.Database.MigrateAsync();
        }
    }

    public class Blog
    {
        public Blog()
        {
            Posts = new HashSet<Post>();
        }

        public int Id { get; set; }
        public string Url { get; set; } = null!;

        public virtual ICollection<Post> Posts { get; set; }
    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;

        public int? BlogId { get; set; }
        public virtual Blog? Blog { get; set; }

        internal sealed class Configuration : IEntityTypeConfiguration<Post>
        {
            public void Configure(EntityTypeBuilder<Post> builder)
            {
                builder.SplitToTable(
                    "BlogPost",
                    tableBuilder =>
                    {
                        tableBuilder.Property(x => x.Id).HasColumnName("PostId");

                        tableBuilder.Property(x => x.BlogId);
                    }
                );
            }
        }
    }
}
