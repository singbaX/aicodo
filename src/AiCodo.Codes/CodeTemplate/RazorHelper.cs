using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorEngine;
using RazorEngine.Templating;
using System.IO;

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

    static class RazorHelper
    {
        static Dictionary<string, CodeCompliledItem> _Templates
            = new Dictionary<string, CodeCompliledItem>();

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

            var viewBag = new DynamicViewBag();
            if (viewBagParameters != null && viewBagParameters.Length > 1)
            {
                for (int i = 0; i < viewBagParameters.Length - 1; i++)
                {
                    viewBag.AddValue(viewBagParameters[i].ToString(), viewBagParameters[i + 1]);
                }
            }

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
        }
    }
}
