using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntitySplitting.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table =>
                    new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table =>
                    new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                        Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "BlogPost",
                columns: table =>
                    new
                    {
                        BlogId = table.Column<int>(type: "int", nullable: false),
                        PostId = table.Column<int>(type: "int", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPost", x => new { x.BlogId, x.PostId });
                    table.ForeignKey(
                        name: "FK_BlogPost_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_BlogPost_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_BlogPost_PostId",
                table: "BlogPost",
                column: "PostId",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "BlogPost");

            migrationBuilder.DropTable(name: "Blogs");

            migrationBuilder.DropTable(name: "Posts");
        }
    }
}
