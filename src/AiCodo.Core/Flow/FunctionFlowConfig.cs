// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using System;
using System.Collections.Generic;

namespace AiCodo.Flow.Configs
{
    public partial class FunctionRefFlowConfig
    {
        public static FunctionRefFlowConfig Load(string id)
        {
            return CreateOrLoad<FunctionRefFlowConfig>($"services\\{id}.xml");
        }
    }
     
    public partial class FunctionFlowConfig
    {
        public static FunctionFlowConfig Load(string id)
        {
            return CreateOrLoad<FunctionFlowConfig>($"services\\{id}.xml");
        }

        public IEnumerable<FunctionRefFlowConfig> GetRefFlowItems()
        {
            if (RefItem.IsNotEmpty())
            {
                var names = RefItem.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var name in names)
                {
                    var refItem = FunctionRefFlowConfig.Load(name);;
                    if (refItem != null)
                    {
                        yield return refItem;
                    }
                }
            }
        }
    }
}
