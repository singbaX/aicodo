// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
namespace AiCodo.Services
{
    using AiCodo.Data;
    using AiCodo.Flow.Configs;
    using Microsoft.AspNetCore.Builder;
    public static class AiCodoServiceLifetimeExtensions
    {
        public static IApplicationBuilder UseAiCodo(this IApplicationBuilder hostBuilder,
            string dataRoot = "", string configRoot = "")
        {
            //全局初始化
            //Step01:set db provider(AiCodo)
            DbProviderFactories.SetFactory("mysql", MySqlProvider.Instance);
            MethodServiceFactory.RegisterService("sql", SqlMethodService.Current);

            //Step02:设置配置及数据目录 
            if (dataRoot.IsNullOrEmpty())
            {
                dataRoot = ApplicationConfig.GetAppSetting<string>("DataRoot", "");
                if (dataRoot.IsNullOrEmpty())
                {
                    dataRoot = "App_Data";
                }
            }
            ApplicationConfig.LocalDataFolder = dataRoot.FixedAppBasePath();

            if (configRoot.IsNullOrEmpty())
            {
                configRoot = ApplicationConfig.GetAppSetting<string>("ConfigRoot", "");
                if (configRoot.IsNullOrEmpty())
                {
                    configRoot = "Configs";
                }
            }
            ApplicationConfig.LocalConfigFolder = configRoot.FixedAppDataPath();

            return hostBuilder;
        }

        public static IApplicationBuilder StartAutofac(this IApplicationBuilder hostBuilder,
            Action<ContainerBuilder> configBuilder)
        {
            var builder = new ContainerBuilder();
            configBuilder?.Invoke(builder);
            var container = builder.Build();

            var scope = container.BeginLifetimeScope();
            ServiceLocator.Current.SetContext(scope);

            return hostBuilder;
        }
    }
}
