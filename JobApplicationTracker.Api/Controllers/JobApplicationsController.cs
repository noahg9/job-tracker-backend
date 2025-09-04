using Microsoft.AspNetCore.Mvc;
using JobApplicationTracker.Api.Services;
using JobApplicationTracker.Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class JobApplicationsController : ControllerBase
{
    private readonly IJobApplicationService _service;
    private readonly ILogger<JobApplicationsController> _logger;

    public JobApplicationsController(IJobApplicationService service, ILogger<JobApplicationsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // Helper to get the current user's email
    private string GetCurrentUserEmail()
    {
        var email = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("Unable to extract user email from JWT claims.");
        }
        else
        {
            _logger.LogInformation("Authenticated request from user: {Email}", email);
        }
        return email;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobApplication>>> GetAll()
    {
        var email = GetCurrentUserEmail();
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        _logger.LogInformation("Fetching all job applications for {Email}", email);

        var jobs = await _service.GetAllAsync(email);
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobApplication>> GetById(int id)
    {
        var email = GetCurrentUserEmail();
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        _logger.LogInformation("Fetching job application {Id} for {Email}", id, email);

        var job = await _service.GetByIdAsync(id);
        if (job == null || job.Username != email)
        {
            _logger.LogWarning("Job application {Id} not found or does not belong to {Email}", id, email);
            return NotFound();
        }

        return Ok(job);
    }

    [HttpPost]
    public async Task<ActionResult<JobApplication>> Create(JobApplication jobApp)
    {
        _logger.LogInformation("Received Create request with payload: {@JobApp}", jobApp);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model validation failed: {@ModelState}", ModelState);
            return BadRequest(ModelState);
        }

        var email = GetCurrentUserEmail();
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        jobApp.Username = email;

        var created = await _service.CreateAsync(jobApp, email);
        _logger.LogInformation("Created job application {Id} for {Email}", created.Id, email);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, JobApplication jobApp)
    {
        _logger.LogInformation("Received Update request for {Id} with payload: {@JobApp}", id, jobApp);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model validation failed: {@ModelState}", ModelState);
            return BadRequest(ModelState);
        }

        var email = GetCurrentUserEmail();
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        jobApp.Username = email;

        var success = await _service.UpdateAsync(id, jobApp, email);
        if (!success)
        {
            _logger.LogWarning("Update failed. Job application {Id} not found or does not belong to {Email}", id, email);
            return NotFound();
        }

        _logger.LogInformation("Updated job application {Id} for {Email}", id, email);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Received Delete request for {Id}", id);

        var email = GetCurrentUserEmail();
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var success = await _service.DeleteAsync(id, email);
        if (!success)
        {
            _logger.LogWarning("Delete failed. Job application {Id} not found or does not belong to {Email}", id, email);
            return NotFound();
        }

        _logger.LogInformation("Deleted job application {Id} for {Email}", id, email);
        return NoContent();
    }
}
