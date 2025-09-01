using JobApplicationTracker.Api.Data;
using JobApplicationTracker.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        // Get all job applications for a specific user
        public async Task<List<JobApplication>> GetAllByUserAsync(string username)
        {
            var jobs = await _context.JobApplications
                                     .Where(j => j.Username == username)
                                     .ToListAsync();
            _logger.LogInformation("Retrieved {Count} job applications for user {User}.", jobs.Count, username);
            return jobs;
        }

        // Get a job application by ID, optional check for owner
        public async Task<JobApplication?> GetByIdAsync(int id)
        {
            var job = await _context.JobApplications.FindAsync(id);
            if (job == null)
                _logger.LogWarning("Job application with ID {Id} not found.", id);
            else
                _logger.LogInformation("Retrieved job application with ID {Id}.", id);
            return job;
        }

        // Add a job application and assign the username
        public async Task AddAsync(JobApplication jobApp)
        {
            _logger.LogDebug("Adding job application for {Company} by {User}.", jobApp.Company, jobApp.Username);
            _context.JobApplications.Add(jobApp);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Job application for {Company} added successfully for {User}.", jobApp.Company, jobApp.Username);
        }

        // Update only if the user owns the job application
        public async Task UpdateAsync(JobApplication jobApp)
        {
            var existing = await _context.JobApplications.FindAsync(jobApp.Id);
            if (existing == null || existing.Username != jobApp.Username)
            {
                _logger.LogWarning("User {User} attempted to update job application with ID {Id} without permission.", jobApp.Username, jobApp.Id);
                return;
            }

            _context.Entry(existing).CurrentValues.SetValues(jobApp);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Job application with ID {Id} updated by {User}.", jobApp.Id, jobApp.Username);
        }

        // Delete only if the user owns the job application
        public async Task DeleteAsync(JobApplication jobApp)
        {
            var existing = await _context.JobApplications.FindAsync(jobApp.Id);
            if (existing == null || existing.Username != jobApp.Username)
            {
                _logger.LogWarning("User {User} attempted to delete job application with ID {Id} without permission.", jobApp.Username, jobApp.Id);
                return;
            }

            _context.JobApplications.Remove(existing);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Job application with ID {Id} deleted by {User}.", jobApp.Id, jobApp.Username);
        }

        // Check if a job application exists by ID
        public async Task<bool> ExistsAsync(int id)
        {
            bool exists = await _context.JobApplications.AnyAsync(e => e.Id == id);
            _logger.LogDebug("Checked existence of job application with ID {Id}: {Exists}", id, exists);
            return exists;
        }
    }
}
