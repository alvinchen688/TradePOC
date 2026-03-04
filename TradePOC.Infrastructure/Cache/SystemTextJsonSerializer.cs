using System.Text.Json;

namespace TradePOC.Infrastructure.Cache
{
    // JSON序列化接口（解耦）
    public interface IJsonSerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
    }

    public class SystemTextJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

        public string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, _options);
        public T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _options);
    }
}
