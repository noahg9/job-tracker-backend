using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobApplicationTracker.Api.Services;
using JobApplicationTracker.Api.Repositories;
using JobApplicationTracker.Api.Models;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobApplication>>> GetAll()
    {
        _logger.LogInformation("GET request received: Get all job applications.");
        var jobs = await _service.GetAllAsync();
        _logger.LogInformation("Returning {Count} job applications.", jobs.Count);
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobApplication>> GetById(int id)
    {
        _logger.LogInformation("GET request received: Get job application with ID {Id}.", id);
        var job = await _service.GetByIdAsync(id);
        if (job == null)
        {
            _logger.LogWarning("Job application with ID {Id} not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Returning job application with ID {Id}.", id);
        return Ok(job);
    }

    [HttpPost]
    public async Task<ActionResult<JobApplication>> Create(JobApplication jobApp)
    {
        _logger.LogInformation("POST request received: Creating job application for {Company}.", jobApp.Company);
        var created = await _service.CreateAsync(jobApp);
        _logger.LogInformation("Job application created with ID {Id}.", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, JobApplication jobApp)
    {
        _logger.LogInformation("PUT request received: Update job application with ID {Id}.", id);
        var success = await _service.UpdateAsync(id, jobApp);
        if (!success)
        {
            _logger.LogWarning("Failed to update job application with ID {Id}. It may not exist or ID mismatch.", id);
            return NotFound();
        }

        _logger.LogInformation("Job application with ID {Id} updated successfully.", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("DELETE request received: Delete job application with ID {Id}.", id);
        var success = await _service.DeleteAsync(id);
        if (!success)
        {
            _logger.LogWarning("Failed to delete job application with ID {Id}. It may not exist.", id);
            return NotFound();
        }

        _logger.LogInformation("Job application with ID {Id} deleted successfully.", id);
        return NoContent();
    }
}
