using Autofac;
using lesson_0.Models;
using lesson_0.Models.Requests.Dialog;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using Saunter.Attributes;
using StackExchange.Redis;
using System.Security.AccessControl;

namespace lesson_0.Accession
{
    public interface IRedisCacheProvider
    {
        public Task SaveKeyAsync<T>(string key, T obj);

        public Task<T> LoadKeyAsync<T>(string key);

        public Task<RedisResult> GetDialogAsync(string fromUserId, string toUserId);
        public Task<RedisResult> SendMessageAsync(string fromUserId, string toUserId, long createdAt, string text);
    }

    public class RedisCacheProvider : IRedisCacheProvider
    {
        private readonly IDatabase _db;

        public RedisCacheProvider(IConnectionMultiplexer connection)
        {
            _db = connection.GetDatabase();
        }

        public async Task SaveKeyAsync<T>(string key, T obj)
        {


            string output = JsonConvert.SerializeObject(obj);

            bool resultSave = await _db.StringSetAsync(key, output);


        }

        public async Task<RedisResult> SendMessageAsync(string fromUserId, string toUserId, long createdAt, string text)
        {
            try
            {
                var res = await _db.ExecuteAsync("FCALL", "SendMessage", 2, fromUserId, toUserId, createdAt, text);

                return res;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public async Task<RedisResult> GetDialogAsync(string fromUserId, string toUserId)
        {
            try
            {
                var res = await _db.ExecuteAsync("FCALL", "GetDialog", 2, fromUserId, toUserId);

                return res;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public async Task<T> LoadKeyAsync<T>(string key)
        {
            string resultValue = await _db.StringGetAsync(key);

            var result = resultValue == null
              ? default
              : JsonConvert.DeserializeObject<T>(resultValue);

            return result;
        }
    }
}
