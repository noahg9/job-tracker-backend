using JobApplicationTracker.Api.Models;
using JobApplicationTracker.Api.Repositories;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobApplicationTracker.Api.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _repository;
        private readonly ILogger<JobApplicationService> _logger;

        public JobApplicationService(IJobApplicationRepository repository, ILogger<JobApplicationService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<List<JobApplication>> GetAllAsync()
        {
            _logger.LogDebug("Fetching all job applications from repository.");
            return await _repository.GetAllAsync();
        }

        public async Task<JobApplication?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Fetching job application with ID {Id} from repository.", id);
            return await _repository.GetByIdAsync(id);
        }

        public async Task<JobApplication> CreateAsync(JobApplication jobApp)
        {
            _logger.LogDebug("Creating new job application for {Company}.", jobApp.Company);
            await _repository.AddAsync(jobApp);
            return jobApp;
        }

        public async Task<bool> UpdateAsync(int id, JobApplication jobApp)
        {
            _logger.LogDebug("Attempting to update job application with ID {Id}.", id);

            if (id != jobApp.Id)
            {
                _logger.LogWarning("Update failed: route ID ({RouteId}) does not match body ID ({BodyId}).", id, jobApp.Id);
                return false;
            }

            if (!await _repository.ExistsAsync(id))
            {
                _logger.LogWarning("Update failed: job application with ID {Id} does not exist.", id);
                return false;
            }

            await _repository.UpdateAsync(jobApp);
            _logger.LogInformation("Job application with ID {Id} updated successfully.", id);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogDebug("Attempting to delete job application with ID {Id}.", id);
            var jobApp = await _repository.GetByIdAsync(id);
            if (jobApp == null)
            {
                _logger.LogWarning("Delete failed: job application with ID {Id} not found.", id);
                return false;
            }

            await _repository.DeleteAsync(jobApp);
            _logger.LogInformation("Job application with ID {Id} deleted successfully.", id);
            return true;
        }
    }
}