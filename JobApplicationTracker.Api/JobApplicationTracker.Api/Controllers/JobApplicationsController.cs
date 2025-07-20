using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobApplicationTracker.Api.Data;
using JobApplicationTracker.Api.Models;

namespace JobApplicationTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobApplicationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public JobApplicationsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/JobApplications
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobApplication>>> GetAll()
    {
        return await _context.JobApplications.ToListAsync();
    }

    // GET: api/JobApplications/5
    [HttpGet("{id}")]
    public async Task<ActionResult<JobApplication>> GetById(int id)
    {
        var jobApp = await _context.JobApplications.FindAsync(id);
        if (jobApp == null) return NotFound();
        return jobApp;
    }

    // POST: api/JobApplications
    [HttpPost]
    public async Task<ActionResult<JobApplication>> Create(JobApplication jobApp)
    {
        _context.JobApplications.Add(jobApp);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = jobApp.Id }, jobApp);
    }

    // PUT: api/JobApplications/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, JobApplication jobApp)
    {
        if (id != jobApp.Id) return BadRequest();

        _context.Entry(jobApp).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!JobApplicationExists(id)) return NotFound();
            else throw;
        }

        return NoContent();
    }

    // DELETE: api/JobApplications/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var jobApp = await _context.JobApplications.FindAsync(id);
        if (jobApp == null) return NotFound();

        _context.JobApplications.Remove(jobApp);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool JobApplicationExists(int id)
    {
        return _context.JobApplications.Any(e => e.Id == id);
    }
}
