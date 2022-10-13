using EntitySplitting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static EntitySplitting.DatabaseScript;

using (var dbContext = new BloggingContext())
{
    await BloggingContext.InitializeAsync(dbContext);
}

using (var dbContext = new BloggingContext())
{
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
    var blog = await dbContext.Blogs.Include(x => x.Posts).SingleAsync();

    Console.WriteLine($"Blog {blog.Url} contains {blog.Posts.Count} posts.");
}

namespace EntitySplitting
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=EntitySplitting;Trusted_Connection=True;MultipleActiveResultSets=true"
            );

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BloggingContext).Assembly);
        }

        public static async Task InitializeAsync(BloggingContext context)
        {
            await context.Database.EnsureDeletedAsync();

            await context.Database.MigrateAsync();

            await context.Database.ExecuteSqlRawAsync(CreatePostTable);

            await context.Database.ExecuteSqlRawAsync(CreateBlogTable);

            await context.Database.ExecuteSqlRawAsync(CreateJoinTable);

            await context.Database.ExecuteSqlRawAsync(AddConstraints);
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

        internal sealed class Configuration : IEntityTypeConfiguration<Blog>
        {
            public void Configure(EntityTypeBuilder<Blog> builder)
            {
                builder
                    .HasMany(d => d.Posts)
                    .WithMany(p => p.Blogs)
                    .UsingEntity<Dictionary<string, object>>(
                        "BlogPost",
                        l => l.HasOne<Post>().WithMany().HasForeignKey("PostId"),
                        r => r.HasOne<Blog>().WithMany().HasForeignKey("BlogId"),
                        j =>
                        {
                            j.HasKey("BlogId", "PostId");

                            j.ToTable("BlogPost");

                            j.HasIndex(new[] { "PostId" }).IsUnique();
                        }
                    );
            }
        }
    }

    public class Post
    {
        public Post()
        {
            Blogs = new HashSet<Blog>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;

        public virtual ICollection<Blog> Blogs { get; set; }
    }
}
