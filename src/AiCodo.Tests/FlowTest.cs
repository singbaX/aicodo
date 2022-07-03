using AiCodo.Flow.Configs;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCodo.Tests
{
    public class FlowTest
    {
        public FlowTest()
        {
            ApplicationConfig.LocalConfigFolder = "";
            var type = typeof(FlowTestMethods);
            FuncService.Current.RegisterMethod("Plus", type, type.GetMethod("Plus"));
            FuncService.Current.RegisterMethod("Plus1", type, type.GetMethod("Plus1"));
        }

        [Test]
        public void BasicLogic()
        {
            var flow = new FunctionFlowItem();
            var action1 = new FunctionFlowAction
            {
                ID = "plus",
                FunctionName = "Plus",
                Parameters = new CollectionBase<ActionInputParameter>
                {
                    new ActionInputParameter
                    {
                        Name = "x",
                        IsInherit=false,
                        Expression = "x"
                    },
                    new ActionInputParameter
                    {
                        Name = "y",
                        IsInherit=false,
                        Expression = "y"
                    },
                },                
            };
            action1.ResultParameters.Add(new ActionOutputParameter
            {
                Name = "Value", ResetInputName="v"
            });
            flow.Actions.Add(action1);

            var action2 = new FunctionFlowAction
            {
                ID = "plus1",
                FunctionName = "Plus1",
                Parameters = new CollectionBase<ActionInputParameter>
                {
                    new ActionInputParameter
                    {
                        Name = "x",
                        IsInherit=false,
                        Expression = "v"
                    }
                }                
            };
            action2.ResultParameters.Add(new ActionOutputParameter
            {
                Name = "Value", ResetInputName="v2"
            });
            flow.Actions.Add(action2);

            flow.Parameters.Add(new FlowInputParameter
            {
                Name ="x",
                DefaultValue="=x"
            });
            flow.Parameters.Add(new FlowInputParameter
            {
                Name ="y",
                DefaultValue="=y"
            });

            var context = new FlowContext(new Dictionary<string, object>
            {
                { "x",12},
                { "y",13},
            });

            var result = context.ExecuteFlow(flow).Result;
            Assert.IsNotNull(result);
        }
    }

    public class FlowTestMethods
    {
        public Dictionary<string,object> Plus(int x,int y)
        {
            return new Dictionary<string, object> { { "Value", x + y } };
        }
        public Dictionary<string,object> Plus1(int x)
        {
            return new Dictionary<string, object> { { "Value", x + 1 } };
        }
    }
}
