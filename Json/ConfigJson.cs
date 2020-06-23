using Newtonsoft.Json;

namespace SKTestBot
{
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
        [JsonProperty("eodDataToken")]
        public string EodDataToken { get; private set; }
    }
}
