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

        public JobApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<JobApplication>> GetAllAsync()
        {
            return await _context.JobApplications.ToListAsync();
        }

        public async Task<JobApplication?> GetByIdAsync(int id)
        {
            return await _context.JobApplications.FindAsync(id);
        }

        public async Task AddAsync(JobApplication jobApp)
        {
            _context.JobApplications.Add(jobApp);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(JobApplication jobApp)
        {
            _context.Entry(jobApp).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(JobApplication jobApp)
        {
            _context.JobApplications.Remove(jobApp);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.JobApplications.AnyAsync(e => e.Id == id);
        }
    }
}
