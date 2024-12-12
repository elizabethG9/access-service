namespace access_service.Src.Services.Interfaces
{
    public interface IBlackListService
    {
        void AddToBlacklist(string token);
        bool IsBlacklisted(string token);
    }   
}