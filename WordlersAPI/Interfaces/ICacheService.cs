namespace WordlersAPI.Interfaces
{
    public interface ICacheService
    {
        T Get<T>(string key);
        T Set<T>(string key, T value, TimeSpan time);

        Task<bool> AddToStore(string key, string value);  
        Task<bool> IsStoreContains(string key, string value);

        Task<long> GetCounter(string key);   
        Task ClearStore(string key);
    }
}
