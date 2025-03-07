namespace SystemManager.Abstractions.Common
{
    public interface ICurrentUserService
    {
        int? GetCurrentUserId();
        string GetCurrentUserName();
    }
}
