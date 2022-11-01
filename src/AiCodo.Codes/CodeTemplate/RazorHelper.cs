// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using AiCodo.Data;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AiCodo.Codes
{
    class CodeCompliledItem
    {
        public string Key { get; set; }

        public string FileName { get; set; }

        public DateTime ComplileTime { get; set; }

        //ITemplateRunner<T>
        public object Runner { get; set; }
    }

    public static class RazorHelper
    {
        static Dictionary<string, CodeCompliledItem> _Templates
            = new Dictionary<string, CodeCompliledItem>();

        static List<Assembly> _TemplateAssemblies = new List<Assembly>
        {
            typeof(System.Linq.Enumerable).Assembly,
            typeof(System.IO.File).Assembly,
            typeof(ObjectConverters).Assembly,
            typeof(SqlData).Assembly,
        };

        public static void AddTemplateAssembly(Assembly asm)
        {
            if (_TemplateAssemblies.Contains(asm)) return;
            _TemplateAssemblies.Add(asm);
        }

        public static string RunTemplateFile<T>(this string fileName, T model, params object[] viewBagParameters)
        {
            if (!fileName.IsFileExists())
            {
                throw new Exception($"文件不存在[{fileName}]");
            }

            if (model == null)
            {
                throw new Exception($"文件模型不能为空[{fileName}]");
            }

#if RazorEngineCore
            var info = new FileInfo(fileName);
            var key = fileName.ToLower();
            if (_Templates.TryGetValue(key, out CodeCompliledItem item))
            {
                if (item.ComplileTime > info.LastWriteTime)
                {
                    var runner = item.Runner as IRazorEngineCompiledTemplate<RazorEngineTemplateBase<T>>;
                    return runner.Run(c => c.Model = model);
                }
            }

            var fileContent = fileName.ReadFileText();
            var template = CreateTemplate<T>(fileContent);
            item = new CodeCompliledItem
            {
                Key = key,
                FileName = fileName,
                ComplileTime = DateTime.Now,
                Runner = template
            };
            _Templates[key] = item;
            return template.Run(c => c.Model = model);
#endif

#if RazorEngine
            //var viewBag = new DynamicViewBag();
            //if (viewBagParameters != null && viewBagParameters.Length > 1)
            //{
            //    for (int i = 0; i < viewBagParameters.Length - 1; i++)
            //    {
            //        viewBag.AddValue(viewBagParameters[i].ToString(), viewBagParameters[i + 1]);
            //    }
            //}

            var info = new FileInfo(fileName);
            var key = fileName.ToLower();
            if (_Templates.TryGetValue(key, out CodeCompliledItem item))
            {
                if (item.ComplileTime > info.LastWriteTime)
                {
                    var runner = item.Runner as ITemplateRunner<T>;
                    return runner.Run(model, viewBag);
                }
            }

            var fileContent = fileName.ReadFileText();
            var newRunner = Engine.Razor.CompileRunner<T>(fileContent);
            item = new CodeCompliledItem
            {
                Key = key,
                FileName = fileName,
                ComplileTime = DateTime.Now,
                Runner = newRunner
            };
            _Templates[key] = item;
            return newRunner.Run(model, viewBag);
#endif
        }

        public static IRazorEngineCompiledTemplate<RazorEngineTemplateBase<T>> CreateTemplate<T>(string fileContent)
        {
            IRazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<RazorEngineTemplateBase<T>> compiledTemplate = razorEngine.Compile<RazorEngineTemplateBase<T>>(fileContent, builder =>
            {
                _TemplateAssemblies.ForEach(a =>
                {
                    builder.AddAssemblyReference(a);
                });
                //builder.AddAssemblyReferenceByName("System.Security"); // by name
                //builder.AddAssemblyReference(typeof(System.IO.File)); // by type
                //builder.AddAssemblyReference(Assembly.Load("source")); // by reference
            });
            return compiledTemplate;
        }
    }
}
