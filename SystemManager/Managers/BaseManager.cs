using Microsoft.Extensions.Logging;
using SystemManager.Abstractions;
using SystemManager.Abstractions.Common;
using SystemManager.Abstractions.Data;
namespace SystemManager.Managers
{
    // SystemManager/Managers/BaseManager.cs
    // SystemManager/Managers/BaseManager.cs
    public abstract class BaseManager
    {
        protected readonly IRepositoryManager RepositoryManager;
        protected readonly ILogger Logger;
        protected readonly ICurrentUserService CurrentUserService;

        protected BaseManager(
            IRepositoryManager repositoryManager,
            ILogger logger,
            ICurrentUserService currentUserService)
        {
            RepositoryManager = repositoryManager;
            Logger = logger;
            CurrentUserService = currentUserService;
        }

        protected async Task LogActionAsync(string action, string details = null)
        {
            var userId = CurrentUserService.GetCurrentUserId();
            var userName = CurrentUserService.GetCurrentUserName();

            Logger.LogInformation(
                "User {UserId} ({UserName}) performed action: {Action}. Details: {Details}",
                userId,
                userName,
                action,
                details ?? "No additional details"
            );
        }
    }
}
