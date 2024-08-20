namespace PmsApi.Models;

public partial class TaskAttachment
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public byte[] FileData { get; set; } = null!;
    public int? TasksId { get; set; }
    public virtual Task? Tasks { get; set; }
}