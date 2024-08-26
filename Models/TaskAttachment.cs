namespace PmsApi.Models;

public partial class TaskAttachment
{
    public int AttachmentId { get; set; }
    public string FileName { get; set; } = null!;
    public string FileData { get; set; } = null!;
    public int? TaskId { get; set; }
    public Task? Task { get; set; }
}