namespace JobApplicationTracker.Api.Models;

public enum ApplicationStatus
{
    Applied,
    Interview,
    Offer,
    Rejected
}

public class JobApplication
{
    public int Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime AppliedDate { get; set; }
    public ApplicationStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
}
