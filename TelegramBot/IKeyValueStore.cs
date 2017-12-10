using System.Threading.Tasks;

namespace TelegramBot
{
    public interface IKeyValueStore
    {
        Task Set(string key, string value);
        string Get(string key);
    }

    public interface IDatabaseValueStore
    {
        Task<IKeyValueStore> GetUserData(int id);
    }
}