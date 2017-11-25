namespace TelegramBot
{
    public interface IKeyValueStore
    {
        void Set(string key, string value);
        string Get(string key);
    }
}