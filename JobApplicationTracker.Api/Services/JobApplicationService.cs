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

        public async Task<List<JobApplication>> GetAllAsync(string username)
        {
            return await _repository.GetAllByUserAsync(username);
        }

        public async Task<JobApplication?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<JobApplication> CreateAsync(JobApplication jobApp, string username)
        {
            jobApp.Username = username;
            await _repository.AddAsync(jobApp);
            return jobApp;
        }

        public async Task<bool> UpdateAsync(int id, JobApplication jobApp, string username)
        {
            if (id != jobApp.Id)
                return false;

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null || existing.Username != username)
                return false;

            await _repository.UpdateAsync(jobApp);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string username)
        {
            var jobApp = await _repository.GetByIdAsync(id);
            if (jobApp == null || jobApp.Username != username)
                return false;

            await _repository.DeleteAsync(jobApp);
            return true;
        }
    }
}
