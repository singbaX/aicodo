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
