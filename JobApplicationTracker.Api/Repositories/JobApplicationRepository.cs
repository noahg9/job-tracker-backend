using JobApplicationTracker.Api.Data;
using JobApplicationTracker.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobApplicationTracker.Api.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<JobApplicationRepository> _logger;

        public JobApplicationRepository(AppDbContext context, ILogger<JobApplicationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<JobApplication>> GetAllAsync()
        {
            var jobs = await _context.JobApplications.ToListAsync();
            _logger.LogInformation("Retrieved {Count} job applications from database.", jobs.Count);
            return jobs;
        }

        public async Task<JobApplication?> GetByIdAsync(int id)
        {
            var job = await _context.JobApplications.FindAsync(id);
            if (job == null)
                _logger.LogWarning("Job application with ID {Id} not found in database.", id);
            else
                _logger.LogInformation("Retrieved job application with ID {Id}.", id);
            return job;
        }

        public async Task AddAsync(JobApplication jobApp)
        {
            _logger.LogDebug("Adding job application for {Company}.", jobApp.Company);
            _context.JobApplications.Add(jobApp);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Job application for {Company} added successfully.", jobApp.Company);
        }

        public async Task UpdateAsync(JobApplication jobApp)
        {
            _logger.LogDebug("Updating job application with ID {Id}.", jobApp.Id);
            _context.Entry(jobApp).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Job application with ID {Id} updated.", jobApp.Id);
        }

        public async Task DeleteAsync(JobApplication jobApp)
        {
            _logger.LogDebug("Deleting job application with ID {Id}.", jobApp.Id);
            _context.JobApplications.Remove(jobApp);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Job application with ID {Id} deleted.", jobApp.Id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            bool exists = await _context.JobApplications.AnyAsync(e => e.Id == id);
            _logger.LogDebug("Checked existence of job application with ID {Id}: {Exists}", id, exists);
            return exists;
        }
    }
}