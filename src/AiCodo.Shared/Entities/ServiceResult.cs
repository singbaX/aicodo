using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AiCodo
{
    public class ServiceResult
    {
        [JsonProperty("data"), DefaultValue(null)]
        public object Data { get; set; }

        [JsonProperty("error"), DefaultValue("")]
        public string Error { get; set; }

        [JsonProperty("errorCode"), DefaultValue("")]
        public string ErrorCode { get; set; }
    }

    public class ServiceResult<T> : ServiceResult
    {
        [JsonProperty("data"), DefaultValue(null)]
        public new T Data { get; set; }
    }
}
