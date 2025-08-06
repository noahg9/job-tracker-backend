using JobApplicationTracker.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobApplicationTracker.Api.Services
{
    public interface IJobApplicationService
    {
        Task<List<JobApplication>> GetAllAsync();
        Task<JobApplication?> GetByIdAsync(int id);
        Task<JobApplication> CreateAsync(JobApplication jobApp);
        Task<bool> UpdateAsync(int id, JobApplication jobApp);
        Task<bool> DeleteAsync(int id);
    }
}
