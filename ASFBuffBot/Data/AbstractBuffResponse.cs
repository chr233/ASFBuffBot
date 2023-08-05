using Newtonsoft.Json;

namespace ASFBuffBot.Data;
public abstract record AbstractBuffResponse<T> : BaseBuffResponse where T : class
{
    [JsonProperty(PropertyName = "data", Required = Required.Default)]
    public T? Data { get; set; }
}
