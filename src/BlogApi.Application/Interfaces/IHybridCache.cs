namespace BlogApi.Application.Interfaces;

public interface IHybridCache
{
    T Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan expiration);
    void Remove(string key);
}
