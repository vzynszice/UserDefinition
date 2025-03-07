namespace SystemManager.Abstractions.Auth
{
    public interface IAuthorizationManager
    {
        Task<bool> HasPermissionAsync(int userId, string permission);
        Task<IEnumerable<string>> GetUserPermissionsAsync(int userId);
        Task<bool> IsInRoleAsync(int userId, string role);
        Task<bool> ValidateAccessAsync(int userId, string resource);
    }
}
