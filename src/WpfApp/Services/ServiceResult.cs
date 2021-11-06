using Newtonsoft.Json;
using System.ComponentModel;

namespace AiCodo.Services
{
    public class ServiceResult
    {
        [JsonProperty("result"), DefaultValue(null)]
        public object Data { get; set; }

        [JsonProperty("error"), DefaultValue("")]
        public string Error { get; set; }

        [JsonProperty("errorCode"), DefaultValue("")]
        public string ErrorCode { get; set; }
    }

    public class ServiceResult<T>:ServiceResult
    {
        [JsonProperty("result"), DefaultValue(null)]
        public T Data { get; set; }
    }
}
