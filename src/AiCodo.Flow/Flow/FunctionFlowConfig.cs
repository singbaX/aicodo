using System;
using System.Collections.Generic;

namespace AiCodo.Flow.Configs
{
    public class FunctionFlowConfig: FunctionFlowItem
    {
        public static FunctionFlowConfig Load(string id)
        {
            return CreateOrLoad<FunctionFlowConfig>($"serivces\\{id}.xml");
        }

        public static FunctionFlowItemRef LoadRefItem(string id)
        {
            return CreateOrLoad<FunctionFlowItemRef>($"serivces\\{id}.xml");
        }

        public override IEnumerable<FunctionFlowItemRef> GetRefFlowItems()
        {
            if (RefItem.IsNotEmpty())
            {
                var names = RefItem.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var name in names)
                {
                    var refItem = LoadRefItem(name);;
                    if (refItem != null)
                    {
                        yield return refItem;
                    }
                }
            }
        }
    }
}
