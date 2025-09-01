using Microsoft.AspNetCore.Identity;

namespace JobApplicationTracker.Api.Services
{
    public interface IJwtService
    {
        string GenerateToken(IdentityUser user);
    }
}
