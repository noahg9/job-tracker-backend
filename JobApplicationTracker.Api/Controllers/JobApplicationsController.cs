using Microsoft.AspNetCore.Mvc;
using JobApplicationTracker.Api.Services;
using JobApplicationTracker.Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

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

    private string GetCurrentUserEmail()
    {
        if (!Request.Headers.TryGetValue("x-ms-client-principal", out var header)) return string.Empty;

        try
        {
            var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(header));
            var principal = JsonSerializer.Deserialize<ClientPrincipal>(decoded);
            return principal?.UserId ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse x-ms-client-principal header.");
            return string.Empty;
        }
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

        var job = await _service.GetByIdAsync(id);
        if (job == null || job.Username != email) return NotFound();

        return Ok(job);
    }

    [HttpPost]
    public async Task<ActionResult<JobApplication>> Create(JobApplication jobApp)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var email = GetCurrentUserEmail();
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        jobApp.Username = email;
        var created = await _service.CreateAsync(jobApp, email);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, JobApplication jobApp)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var email = GetCurrentUserEmail();
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        jobApp.Username = email;
        var success = await _service.UpdateAsync(id, jobApp, email);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var email = GetCurrentUserEmail();
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var success = await _service.DeleteAsync(id, email);
        if (!success) return NotFound();

        return NoContent();
    }

    private class ClientPrincipal
    {
        public string IdentityProvider { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserDetails { get; set; } = string.Empty;
        public string[] UserRoles { get; set; } = Array.Empty<string>();
    }
}
