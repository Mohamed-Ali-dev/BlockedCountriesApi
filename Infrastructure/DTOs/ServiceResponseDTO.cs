namespace Infrastructure.DTOs
{
    public record ServiceResponseDTO(bool Success = false, string Message = null!);
}
