
using Microsoft.AspNetCore.Http;
using SystemManager.Abstractions.Common;

namespace BLL.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public int? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return null;

            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        public string GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        }
    }
}
