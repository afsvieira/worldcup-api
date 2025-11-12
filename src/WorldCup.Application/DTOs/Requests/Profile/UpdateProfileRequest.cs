namespace WorldCup.Application.DTOs.Requests.Profile;

/// <summary>
/// Request model for updating user profile
/// </summary>
public class UpdateProfileRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
