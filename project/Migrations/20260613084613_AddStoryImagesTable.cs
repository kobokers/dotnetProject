using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project.Migrations
{
    /// <inheritdoc />
    public partial class AddStoryImagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF OBJECT_ID('dbo.StoryImages', 'U') IS NULL BEGIN
                CREATE TABLE [dbo].[StoryImages] (
                    [StoryImageId] int IDENTITY(1,1) NOT NULL,
                    [StoryId] int NOT NULL,
                    [ImageUrl] nvarchar(max) NOT NULL,
                    [Order] int NOT NULL,
                    CONSTRAINT [PK_StoryImages] PRIMARY KEY ([StoryImageId]),
                    CONSTRAINT [FK_StoryImages_Stories_StoryId] FOREIGN KEY ([StoryId]) REFERENCES [Stories]([StoryId]) ON DELETE CASCADE
                );
                CREATE INDEX IX_StoryImages_StoryId ON [dbo].[StoryImages]([StoryId]);
            END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF OBJECT_ID('dbo.StoryImages', 'U') IS NOT NULL DROP TABLE [dbo].[StoryImages];");
        }
    }
}
