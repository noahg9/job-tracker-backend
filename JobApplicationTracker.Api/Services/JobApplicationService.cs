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
            return await _repository.GetAllAsync();
        }

        public async Task<JobApplication?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<JobApplication> CreateAsync(JobApplication jobApp)
        {
            await _repository.AddAsync(jobApp);
            return jobApp;
        }

        public async Task<bool> UpdateAsync(int id, JobApplication jobApp)
        {
            if (id != jobApp.Id) return false;

            if (!await _repository.ExistsAsync(id)) return false;

            await _repository.UpdateAsync(jobApp);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var jobApp = await _repository.GetByIdAsync(id);
            if (jobApp == null) return false;

            await _repository.DeleteAsync(jobApp);
            return true;
        }
    }
}
