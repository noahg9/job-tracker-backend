namespace JobApplicationTracker.Api.Models;

public enum ApplicationStatus
{
    Applied,
    Interview,
    Offer,
    Rejected,
    Accepted
}

public class JobApplication
{
    public int Id { get; set; }
    public required string Company { get; set; }
    public required string Role { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime AppliedDate { get; set; }
    public string? Notes { get; set; }
    public required string Username { get; set; }
}

