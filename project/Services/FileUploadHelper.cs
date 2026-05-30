namespace project.Services;

public static class FileUploadHelper
{
    private static readonly Dictionary<string, string> AllowedExtensions = new()
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".webp", "image/webp" }
    };

    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public static string SaveFile(IFormFile file, string subDirectory, string wwwRootPath)
    {
        if (file.Length == 0)
            throw new ArgumentException("File is empty.");

        if (file.Length > MaxFileSize)
            throw new ArgumentException("File size exceeds 5MB limit.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.ContainsKey(ext))
            throw new ArgumentException($"File type {ext} not allowed. Allowed: {string.Join(", ", AllowedExtensions.Keys)}");

        var uploadsDir = Path.Combine(wwwRootPath, "uploads", subDirectory);
        if (!Directory.Exists(uploadsDir))
            Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid():N}_{Path.GetFileNameWithoutExtension(file.FileName)}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        file.CopyTo(stream);

        return $"/uploads/{subDirectory}/{fileName}";
    }
}
