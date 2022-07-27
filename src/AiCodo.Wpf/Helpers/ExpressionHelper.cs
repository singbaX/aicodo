// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using DynamicExpresso;
using System;
using System.Collections.Generic;

namespace AiCodo
{
    delegate object IF(bool condition, object trueValue, object falseValue);
    delegate bool BoolDelegate(string fileName);
    delegate string FormatDelegate(string format, params object[] args);

    public static partial class ExpressionHelper
    {
        static Dictionary<string, Delegate> _DefaultFunctions = new Dictionary<string, Delegate>
        {
            {"IF",new IF(IF) },
            {"Format",new FormatDelegate(Format) },
            {"FileExists",new BoolDelegate(file=>file.IsFileExists()) },
            {"DirExists",new BoolDelegate(dir=>dir.IsDirectoryExists()) },
        };

        static string[] Part4Names = new string[] { "LT", "RT", "LB", "RB" };

        public static void SetFunction(string name, Delegate dg)
        {
            _DefaultFunctions[name] = dg;
        }

        public static Interpreter GetInterpreter(IEnumerable<KeyValuePair<string, object>> paras)
        {
            var context = CreateInterpreter();
            if (paras != null)
            {
                foreach (var p in paras)
                {
                    context.SetVariable(p.Key, p.Value);
                }
            }
            return context;
        }

        public static Interpreter CreateInterpreter()
        {
            var context = new Interpreter();
            _DefaultFunctions.ForEach(f =>
            {
                context.SetFunction(f.Key, f.Value);
            });

            context.Reference(typeof(ObjectConverters));
            context.Reference(typeof(System.IO.Path));
            return context;
        }

        public static object Eval(this string expresss, params object[] nameValues)
        {
            var context = CreateInterpreter();
            if (nameValues != null && nameValues.Length > 0)
            {
                for (int i = 0; i < nameValues.Length - 1; i += 2)
                {
                    context.SetVariable(nameValues[i].ToString(), nameValues[i + 1]);
                }
            }
            var result = context.Eval(expresss);
            return result;
        }

        public static object Eval(this string expresss, IDictionary<string, object> paras)
        {
            var context = GetInterpreter(paras);
            var result = context.Eval(expresss);
            return result;
        }

        public static  TFunc CreateFunc<TFunc>(this string expression,params string[] names)
        {
            var context = CreateInterpreter();
            return context.ParseAsExpression<TFunc>(expression,names).Compile();
        }

        public static string Format(string format,params object[] args)
        {
            return string.Format(format, args);
        }

        public static object IF(bool condition, object trueValue, object falseValue)
        {
            return condition ? trueValue : falseValue;
        }
    }
}
