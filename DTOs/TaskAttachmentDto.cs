namespace PmsApi.DTOs;

public record TaskAttachmentDto
(
    int AttachmentId,

    string FileName,

    string FileData,

    int? TaskId
);
