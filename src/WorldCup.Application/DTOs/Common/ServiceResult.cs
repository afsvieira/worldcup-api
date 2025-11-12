namespace WorldCup.Application.DTOs.Common;

/// <summary>
/// Base result type for service operations
/// Provides consistent response structure across all services
/// </summary>
public class ServiceResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ServiceResult Successful(string? message = null)
        => new() { Success = true, Message = message };

    public static ServiceResult Failed(string message)
        => new() { Success = false, Message = message };

    public static ServiceResult Failed(IEnumerable<string> errors)
        => new() { Success = false, Errors = errors.ToList() };
}
