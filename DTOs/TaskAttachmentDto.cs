

public record TaskAttachmentDto
(
    int AttachmentId,

    string FileName,

    byte[] FileData,

    int? TaskId
);
