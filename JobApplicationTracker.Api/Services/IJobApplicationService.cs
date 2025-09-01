using JobApplicationTracker.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobApplicationTracker.Api.Services
{
    public interface IJobApplicationService
    {
        Task<List<JobApplication>> GetAllAsync(string username);
        Task<JobApplication?> GetByIdAsync(int id);
        Task<JobApplication> CreateAsync(JobApplication jobApp, string username);
        Task<bool> UpdateAsync(int id, JobApplication jobApp, string username);
        Task<bool> DeleteAsync(int id, string username);
    }
}
