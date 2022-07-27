using AiCodo.Data;
using AiCodo.Flow.Configs;
using AiCodo.Tests;

namespace AiCodo.Flow.Tests
{
    public class FlowConfigFileTest
    {
        [SetUp]
        public void Setup()
        {
            //Step01:set db provider(AiCodo)
            DbProviderFactories.SetFactory("mysql", MySqlProvider.Instance);
            //Step02:set config files path(AiCodo)
            ApplicationConfig.LocalDataFolder = System.IO.Path.Combine(ApplicationConfig.BaseDirectory, "App_Data");
            ApplicationConfig.LocalConfigFolder = "configs".FixedAppDataPath();
            MethodServiceFactory.RegisterService("sql", SqlMethodService.Current);
        }

        [Test]
        public void TestF0001()
        {
            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"CurrentUserID",1 },
                {"UserName","admin" },
                {"Password","123456" },
                {"Email","singba@163.com" }
            };
            var context = new FlowContext(args);
            if (TryGetFlow("sys_user.insert", out var flow))
            {
                var result = context.ExecuteFlow(flow).Result;
                Assert.IsTrue(result.GetInt32("UserID") > 0);
            }
        }

        private bool TryGetFlow(string flowName, out FunctionFlowConfig flow)
        {
            var item = FunctionFlowIndex.Current.Items.FirstOrDefault(f => f.Name.Equals(flowName, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                flow = FunctionFlowConfig.Load(item.ID);
                return true;
            }

            flow = null;
            return false;
        }
    }
}
