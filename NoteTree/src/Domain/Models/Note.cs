namespace Domain.Models;
public sealed class Note
{
    public string? Content { get; set; }
    public Note[]? Children { get; set; }
}
