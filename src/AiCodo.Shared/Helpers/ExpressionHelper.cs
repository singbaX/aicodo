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

        static List<Type> _ReferenceTypes = new List<Type>();

        public static void SetFunction(string name, Delegate dg)
        {
            _DefaultFunctions[name] = dg;
        }

        public static void AddReferenceType(params Type[] types)
        {
            if (types != null)
            {
                lock (_ReferenceTypes)
                {
                    types.ForEach(t =>
                    {
                        if (!_ReferenceTypes.Contains(t))
                        {
                            _ReferenceTypes.Add(t);
                        }
                    });
                }
            }
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
            foreach(var t in _ReferenceTypes)
            {
                context.Reference(t);
            }
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

        public static TFunc CreateFunc<TFunc>(this string expression, params string[] names)
        {
            var context = CreateInterpreter();
            return context.ParseAsExpression<TFunc>(expression, names).Compile();
        }

        public static string Format(string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static object IF(bool condition, object trueValue, object falseValue)
        {
            return condition ? trueValue : falseValue;
        }
    }
}
