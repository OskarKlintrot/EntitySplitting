namespace EntitySplitting;

internal static class DatabaseScript
{
    public const string CreatePostTable = """
        CREATE TABLE [dbo].[Posts](
            [Id] [int] IDENTITY(1,1) NOT NULL,
            [Title] [nvarchar](max) NOT NULL,
            [Content] [nvarchar](max) NOT NULL,
            CONSTRAINT [PK_Posts] PRIMARY KEY CLUSTERED 
        (
            [Id] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
        """;

    public const string CreateBlogTable = """
        CREATE TABLE [dbo].[Blogs](
            [Id] [int] IDENTITY(1,1) NOT NULL,
            [Url] [nvarchar](max) NOT NULL,
            CONSTRAINT [PK_Blogs] PRIMARY KEY CLUSTERED 
        (
            [Id] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
        """;

    public const string CreateJoinTable = """
        CREATE TABLE [dbo].[BlogPost](
            [BlogId] [int] NOT NULL,
            [PostId] [int] NOT NULL,
            CONSTRAINT [PK_BlogPost] PRIMARY KEY CLUSTERED 
        (
            [BlogId] ASC,
            [PostId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
        ) ON [PRIMARY]
        
        CREATE UNIQUE NONCLUSTERED INDEX [IX_BlogPost_PostId] ON [dbo].[BlogPost]
        (
        	[PostId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
        """;

    public const string AddConstraints = """
        ALTER TABLE [dbo].[BlogPost]  WITH CHECK ADD  CONSTRAINT [FK_BlogPost_Blogs_BlogId] FOREIGN KEY([BlogId])
        REFERENCES [dbo].[Blogs] ([Id])
        ON DELETE CASCADE
            
        ALTER TABLE [dbo].[BlogPost] CHECK CONSTRAINT [FK_BlogPost_Blogs_BlogId]
            
        ALTER TABLE [dbo].[BlogPost]  WITH CHECK ADD  CONSTRAINT [FK_BlogPost_Posts_PostId] FOREIGN KEY([PostId])
        REFERENCES [dbo].[Posts] ([Id])
        ON DELETE CASCADE

        ALTER TABLE [dbo].[BlogPost] CHECK CONSTRAINT [FK_BlogPost_Posts_PostId]
        """;
}
