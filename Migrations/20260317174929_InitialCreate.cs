using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SemantiCore_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IndexCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryIndexes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndexType = table.Column<int>(type: "int", nullable: false),
                    IndexName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IndexValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IndexCategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryIndexes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryIndexes_IndexCategories_IndexCategoryId",
                        column: x => x.IndexCategoryId,
                        principalTable: "IndexCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndexedDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryIndexId = table.Column<int>(type: "int", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoredFilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexedDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndexedDocuments_CategoryIndexes_CategoryIndexId",
                        column: x => x.CategoryIndexId,
                        principalTable: "CategoryIndexes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentChunks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndexedDocumentId = table.Column<int>(type: "int", nullable: false),
                    ChunkText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChunkOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentChunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentChunks_IndexedDocuments_IndexedDocumentId",
                        column: x => x.IndexedDocumentId,
                        principalTable: "IndexedDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTexts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndexedDocumentId = table.Column<int>(type: "int", nullable: false),
                    ExtractedText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtractedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTexts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentTexts_IndexedDocuments_IndexedDocumentId",
                        column: x => x.IndexedDocumentId,
                        principalTable: "IndexedDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentEmbeddings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentChunkId = table.Column<int>(type: "int", nullable: false),
                    Vector = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentEmbeddings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentEmbeddings_DocumentChunks_DocumentChunkId",
                        column: x => x.DocumentChunkId,
                        principalTable: "DocumentChunks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryIndexes_IndexCategoryId_IndexName",
                table: "CategoryIndexes",
                columns: new[] { "IndexCategoryId", "IndexName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentChunks_IndexedDocumentId_ChunkOrder",
                table: "DocumentChunks",
                columns: new[] { "IndexedDocumentId", "ChunkOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentEmbeddings_DocumentChunkId",
                table: "DocumentEmbeddings",
                column: "DocumentChunkId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTexts_IndexedDocumentId",
                table: "DocumentTexts",
                column: "IndexedDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_IndexCategories_CategoryName",
                table: "IndexCategories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndexedDocuments_CategoryIndexId",
                table: "IndexedDocuments",
                column: "CategoryIndexId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentEmbeddings");

            migrationBuilder.DropTable(
                name: "DocumentTexts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "DocumentChunks");

            migrationBuilder.DropTable(
                name: "IndexedDocuments");

            migrationBuilder.DropTable(
                name: "CategoryIndexes");

            migrationBuilder.DropTable(
                name: "IndexCategories");
        }
    }
}
