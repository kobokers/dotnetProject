using System.ComponentModel.DataAnnotations.Schema;

namespace project.Models;

public class StoryImage
{
    public int StoryImageId { get; set; }
    public int StoryId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int Order { get; set; }
    public Story Story { get; set; } = null!;
}
