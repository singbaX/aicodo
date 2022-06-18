// *************************************************************************************************
// 代码文件：FlowContext.cs
// 
// 作    者：SingbaX
// 
// 创建日期：202206
// 
// 功能说明：流程执行服务
//
// 修改记录：
// *************************************************************************************************

namespace AiCodo.Flow.Configs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;

    public class FuncService : IMethodService
    {
        #region 属性 Current
        private static FuncService _Current = new FuncService();
        public static FuncService Current
        {
            get
            {
                return _Current;
            }
        }
        #endregion

        public IFunctionItem GetItem(string name)
        {
            return GetFunctionItem(name);
        }

        private static IFunctionItem GetFunctionItem(string name)
        {
            return FunctionConfig.Current.GetItem(name);
        }

        public IEnumerable<NameItem> GetItems()
        {
            return FunctionConfig.Current.Items
                .Select(f => new NameItem(f.Name, f.DisplayName));
        }

        public FunctionResult Run(string name, Dictionary<string, object> args)
        {
            var item = GetFunctionItem(name);
            if (item == null)
            {
                throw new MissingMethodException(name);
            }
            if (item is FunctionItemConfig funcItem)
            {
                return FunctionService.Run(funcItem, args);
            }
            else
            {
                throw new MissingMethodException(name);
            }
        }
    }

    internal static class FunctionService
    {
        static string[] _ImportDll = new string[] { "System.ComponentModel.Composition.dll" };

        static CollectibleAssemblyLoadContext _LoadContext = null;

        static FunctionService()
        {
            _LoadContext = new CollectibleAssemblyLoadContext();
        }

        #region 方法的查找及调用
        static readonly Dictionary<string, MethodInfo> _Methods = new Dictionary<string, MethodInfo>();

        public static object RunMethod(this string name, params object[] args)
        {
            #region 动态获取方法并缓存
            if (!_Methods.TryGetValue(name, out MethodInfo method))
            {
                lock (_Methods)
                {
                    if (!_Methods.TryGetValue(name, out method))
                    {
                        var methodItem = FunctionConfig.Current.CommonMethods.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                        if (methodItem == null)
                        {
                            throw new Exception($"方法[{name}]不存在或没有配置");
                        }
                        method = GetMethod(methodItem);
                        _Methods[name] = method;
                    }
                }
            }
            #endregion

            //执行方法
            object value = null;
            try
            {
                value = method.Invoke(null, args);
                return value;
            }
            catch (Exception ex)
            {
                nameof(FunctionService).Log(ex.ToString(), Category.Exception);
                throw new Exception($"执行算法方法[{name}]出错：{ex.Message}", ex);
            }
        }

        public static FunctionResult Run(string itemName, Dictionary<string, object> args)
        {
            var item = FunctionConfig.Current.Items.FirstOrDefault(f => f.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
            if (item != null && item is FunctionItemConfig itemconfig)
            {
                return Run(itemconfig, args);
            }
            return null;
        }

        public static FunctionResult Run(this FunctionItemConfig item, Dictionary<string, object> args)
        {
            if (args == null)
            {
                throw new ArgumentException($"执行方法缺少必须参数", nameof(args));
            }

            var name = item.Name;
            #region 动态获取方法并缓存
            if (!_Methods.TryGetValue(name, out MethodInfo method))
            {
                lock (_Methods)
                {
                    if (!_Methods.TryGetValue(name, out method))
                    {
                        method = GetMethod(item.Location);
                        _Methods[name] = method;
                    }
                }
            }
            #endregion

            #region 设置方法参数
            var pinfos = method.GetParameters();
            var parameters = pinfos.Select(p =>
            {
                if (args.TryGetValue(p.Name, out object v))
                {
                    if (p.ParameterType.IsByRef)
                    {
                        if (p.ParameterType.FullName.Equals("System.Int32&"))
                        {
                            int i = v.ToInt32();
                            args[p.Name] = i;
                            return i;
                        }
                        if (p.ParameterType.FullName.Equals("System.Double&"))
                        {
                            var i = v.ToDouble();
                            args[p.Name] = i;
                            return i;
                        }
                    }
                    return v;
                }
                if (p.HasDefaultValue)
                {
                    return p.DefaultValue;
                }
                throw new ArgumentException($"执行方法缺少必须参数[{method.Name}-{p.Name}]", p.Name);
            }).ToArray();

            #region 添加日志
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"开始执行：[{item.Name}]-[{item.DisplayName}]");
            for (int i = 0; i < pinfos.Length; i++)
            {
                var p = pinfos[i];
                if (parameters[i] != null && parameters[i] is IList list)
                {
                    sb.AppendLine($"{p.Name}= List,Count:{list.Count}");
                }
                else
                {
                    sb.AppendLine($"{p.Name}={parameters[i]}");
                }
            }
            nameof(FunctionService).Log(sb.ToString());
            #endregion
            #endregion

            //执行方法
            object value = null;
            try
            {
                nameof(FunctionService).Log($"Start [{method.Name}]");
                value = method.Invoke(null, parameters);
                nameof(FunctionService).Log($"End [{method.Name}]");
            }
            catch (Exception ex)
            {
                ex.WriteErrorLog();
                var sbError = new System.Text.StringBuilder();
                sb.AppendLine($"执行方法出错【{method.Name}】:{ex.Message}");
                sb.AppendLine(ex.ToString());
                //method.GetParameters().ForEach(p => sbError.AppendLine($"[{p.Name}]-{p.ParameterType}"));
                nameof(FunctionService).Log(sbError.ToString(), Category.Exception);
                throw new Exception($"执行方法出错【{method.Name}】：{ex.Message}", ex);
            }

            if (value == null)
            {
                sb.AppendLine($"执行方法无返回值：[{item.Name}]-[{item.DisplayName}]");
                return null;
            }

            if (value is FunctionResult result)
            {
                return result;
            }

            dynamic obj = value;
            result = new FunctionResult();
            try
            {
                result.ErrorCode = obj.ErrorCode;
                result.ErrorMessage = obj.ErrorMessage;

                if (!result.IsOK)
                {
                    $"FunctionService".Log($"请联系算法工程师，执行方法[{method.Name}]出错,错误码：{result.ErrorCode},错误说明：{result.ErrorMessage}", Category.Exception);
                    return result;
                }

                //因为不强依赖，动态加载，所以这里要反射判断返回值类型
                var dataProperty = value.GetType().GetProperty("Data");
                if (dataProperty != null)
                {
                    if (dataProperty.PropertyType.IsClass)
                    {
                        if (dataProperty.PropertyType == typeof(Dictionary<string, object>))
                        {
                            Dictionary<string, object> kvs = dataProperty.GetValue(value) as Dictionary<string, object>;
                            kvs.ForEach(kv => result.Data.Add(kv.Key, kv.Value));
                        }
                        else
                        {
                            var data = dataProperty.GetValue(value);
                            foreach (var pinfo in dataProperty.PropertyType.GetProperties())
                            {
                                result.Data.Add(pinfo.Name, pinfo.GetValue(data));
                            }
                        }
                    }
                    else if (dataProperty.PropertyType == typeof(bool))
                    {
                        result.Data.Add("Result", (bool)obj.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"执行方法成功，但转换结果出错：{ex.Message}", ex);
            }
            return result;
        }
        public static Type GetType(string asmName, string asmFileName, string className)
        {
            Assembly asm = GetAssembly(asmName, asmFileName);
            var cls = asm.GetType(className);
            if (cls == null)
            {
                throw new Exception($"算法配置错误，程序集[{asmName}]中未找到[{className}]");
            }
            return cls;
        }

        private static Assembly GetAssembly(string asmName, string asmFileName)
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(w => w.GetName() != null && w.GetName().Name.Equals(asmName, StringComparison.OrdinalIgnoreCase));

            if (asm == null)
            {
                asm = _LoadContext.GetAssemblies()
                    .FirstOrDefault(w => w.GetName() != null && w.GetName().Name.Equals(asmName, StringComparison.OrdinalIgnoreCase));
            }

            #region 如果程序集未加载，则通过配置加载程序集
            if (asm == null)
            {
                if (asmFileName.IsNullOrEmpty())
                {
                    throw new Exception($"算法配置错误，没有指定程序集路径[FileName]");
                }

                var fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, asmFileName);
                if (!System.IO.File.Exists(fileName))
                {
                    throw new Exception($"算法配置错误，文件不存在[{fileName}]");
                }

                try
                {
                    asm = _LoadContext.LoadFromAssemblyPath(fileName);
                    //asm = Assembly.LoadFrom(fileName);
                    if (asm != null)
                    {
                        if (asm.GetName().Name.Equals("ImageFunction"))
                        {
                            var version = asm.GetName().Version.ToString();
                            if (FunctionConfig.Current.ImageFunctionVersion.IsNotEmpty())
                            {
                                if (version != FunctionConfig.Current.ImageFunctionVersion)
                                {
                                    "FunctionService".Log($"算法程序版本[{version}]与配置版本[{FunctionConfig.Current.ImageFunctionVersion}]不一致：", Category.Warn);
                                }
                            }
                        }

                        LoadAllReferenced(asm);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"算法配置错误，程序集[{asmName}]加载失败：{ex.Message}", ex);
                }
            }
            #endregion
            return asm;
        }

        //public static void CheckMethodItems(string assemblyName = "", string fileName = "")
        //{
        //    var asm = GetAssembly(assemblyName, fileName);
        //    if (asm != null)
        //    {
        //        FunctionConfig.Current.ImageFunctionVersion = asm.GetName().Version.ToString();
        //        foreach (var type in asm.GetTypes())
        //        {
        //            foreach (var method in type.GetMethods().Where(m => m.GetCustomAttributes().FirstOrDefault(f => f.GetType().FullName.Equals(ExportFullName)) != null))
        //            {
        //                var methodName = method.Name;
        //                var configItem = FunctionConfig.Current.Items.FirstOrDefault(f => f.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));

        //                if (configItem != null && configItem is FunctionItemConfig aitem) //已有配置，检查参数名
        //                {
        //                    ResetItemParameters(aitem, method);
        //                }
        //                else
        //                {
        //                    var item = new FunctionItemConfig
        //                    {
        //                        Name = methodName,
        //                        DisplayName = methodName,
        //                        Location = new FunctionItemLocation
        //                        {
        //                            AsmName = assemblyName,
        //                            FileName = fileName,
        //                            ClassName = type.FullName,
        //                            MethodName = methodName
        //                        }
        //                    };
        //                    ResetItemParameters(item, method);
        //                    FunctionConfig.Current.Items.Add(item);
        //                }
        //            }
        //        }
        //    }
        //}

        public static MethodInfo GetMethod(this FunctionItemLocation item)
        {
            MethodInfo method;
            if (item.IsEmptyConfig())
            {
                throw new Exception($"算法配置错误，没有指定程序集路径");
            }
            var cls = GetType(item.AsmName, item.FileName, item.ClassName);
            if (cls == null)
            {
                throw new Exception($"算法配置错误，程序集[{item.AsmName}]中未找到[{item.ClassName}]");
            }

            method = cls.GetMethod(item.MethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null)
            {
                throw new MethodNotFoundException { MethodName = item.MethodName };
            }
            return method;
        }

        private static void LoadAllReferenced(Assembly asm)
        {
            asm.GetReferencedAssemblies()
                .ForEach(a =>
                {
                    if (a != null && a.Name.IsNotEmpty())
                    {
                        if (_LoadContext.GetAssemblies()
                             .FirstOrDefault(w => w.GetName() != null && w.GetName().Name.Equals(a.Name, StringComparison.OrdinalIgnoreCase)) == null)
                        {
                            var fileName = $"{a.Name}.dll".FixedAppBasePath();
                            if (fileName.IsFileExists())
                            {
                                try
                                {
                                    var subAsm = _LoadContext.LoadFromAssemblyPath(fileName);
                                    // var subAsm = Assembly.LoadFrom(fileName);
                                    typeof(FunctionService).Log($"加载文件{fileName}");
                                    LoadAllReferenced(subAsm);
                                }
                                catch (Exception loadEx)
                                {
                                    typeof(FunctionService).Log($"Load{fileName}\r\n {loadEx.ToString()}");
                                }
                            }
                        }
                    }
                });
        }
        #endregion

        public static void ResetItemParameters(FunctionItemConfig item, MethodInfo method)
        {
            var returnType = method.ReturnType;
            var parameters = item.Parameters.ToList();
            item.Parameters.Clear();
            method.GetParameters()
                .ForEach(mp =>
                {
                    var p = parameters.FirstOrDefault(f => f.Name.Equals(mp.Name, StringComparison.OrdinalIgnoreCase));
                    if (p == null)
                    {
                        p = new ParameterItem
                        {
                            Name = mp.Name,
                            TypeName = GetParameterTypeName(mp.ParameterType)
                        };
                    }
                    else if (p.TypeName == "String")
                    {
                        p.TypeName = GetParameterTypeName(mp.ParameterType);
                    }
                    item.Parameters.Add(p);
                });

            var resultParameters = item.ResultParameters.ToList();
            item.ResultParameters.Clear();
            var dataProperty = returnType.GetProperty("Data");
            if (dataProperty != null)
            {
                if (dataProperty.PropertyType.IsClass)
                {
                    if (dataProperty.PropertyType == typeof(Dictionary<string, object>))
                    {

                    }
                    else
                    {
                        foreach (var pinfo in dataProperty.PropertyType.GetProperties())
                        {
                            var p = resultParameters.FirstOrDefault(f => f.Name.Equals(pinfo.Name, StringComparison.OrdinalIgnoreCase));
                            if (p == null)
                            {
                                p = new ResultParameter
                                {
                                    Name = pinfo.Name,
                                    TypeName = GetParameterTypeName(pinfo.PropertyType)
                                };
                            }
                            item.ResultParameters.Add(p);
                        }
                    }
                }

                if (dataProperty.PropertyType == typeof(bool))
                {
                    var p = resultParameters.FirstOrDefault(f => f.Name.Equals("Result", StringComparison.OrdinalIgnoreCase));
                    if (p == null)
                    {
                        p = new ResultParameter
                        {
                            Name = "Result",
                            TypeName = GetParameterTypeName(typeof(bool))
                        };
                    }
                    item.ResultParameters.Add(p);
                }
            }
        }

        private static string GetParameterTypeName(Type type)
        {
            if (type.IsByRef)
            {
                var atype = type.Assembly.GetType(type.FullName.TrimEnd('&'));
                if (atype != null)
                {
                    type = atype;
                }
                else
                {
                    throw new Exception($"类型错误[{type.FullName}]");
                }
            }

            if (type == typeof(bool))
            {
                return "Bool";
            }

            if (type == typeof(int))
            {
                return "Int";
            }

            if (type == typeof(float) || type == typeof(Single))
            {
                return "Single";
            }
            if (type == typeof(double))
            {
                return "Double";
            }

            return "String";
        }
    }

    public class CollectibleAssemblyLoadContext
    {
        private Dictionary<string, Assembly> _Assemblies = new Dictionary<string, Assembly>();

        public IEnumerable<Assembly> GetAssemblies()
        {
            foreach(var a in _Assemblies)
            {
                yield return a.Value;
            }
            foreach(var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                yield return a;
            }
        }

        public Assembly LoadFromAssemblyPath(string fileName)
        {
            var asm= Assembly.LoadFrom(fileName);
            _Assemblies[asm.GetName().Name] = asm;
            return asm;
        }
    }
}
