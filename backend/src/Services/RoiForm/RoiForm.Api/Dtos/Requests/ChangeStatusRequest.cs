using RoiForm.Domain.FormManagement.Enums;

namespace RoiForm.Api.Dtos.Requests;

public class ChangeStatusRequest
{
    public required RoiFormStatus Status { get; init; }
}
