namespace PmsApi.DTOs;

using Task = PmsApi.Models.Task;
public record AttachmentWithTaskDto
(
    int AttachmentId,

    string FileName,

    string FileData,

    int TaskId,

    Task Task
);