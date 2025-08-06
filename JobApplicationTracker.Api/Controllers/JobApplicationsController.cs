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
        var jobs = await _service.GetAllAsync();
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobApplication>> GetById(int id)
    {
        var job = await _service.GetByIdAsync(id);
        if (job == null) return NotFound();
        return Ok(job);
    }

    [HttpPost]
    public async Task<ActionResult<JobApplication>> Create(JobApplication jobApp)
    {
        var created = await _service.CreateAsync(jobApp);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, JobApplication jobApp)
    {
        var success = await _service.UpdateAsync(id, jobApp);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
