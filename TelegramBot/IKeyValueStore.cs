using System.Threading.Tasks;

namespace TelegramBot
{
    public interface IKeyValueStore
    {
        Task Set(string key, string value);
        string Get(string key);
        UriHandler GetUriHandler();
        Task Save(UriHandler urihandler);
    }

    public interface IDatabaseValueStore
    {
        Task<IKeyValueStore> GetUserData(int id);
    }
}