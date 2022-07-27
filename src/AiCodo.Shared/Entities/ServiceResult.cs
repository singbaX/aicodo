// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
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
