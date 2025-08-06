using JobApplicationTracker.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobApplicationTracker.Api.Repositories
{
    public interface IJobApplicationRepository
    {
        Task<List<JobApplication>> GetAllAsync();
        Task<JobApplication?> GetByIdAsync(int id);
        Task AddAsync(JobApplication jobApp);
        Task UpdateAsync(JobApplication jobApp);
        Task DeleteAsync(JobApplication jobApp);
        Task<bool> ExistsAsync(int id);
    }
}
