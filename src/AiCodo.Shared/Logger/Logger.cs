using System;
using System.Collections.Generic;
using System.Text;

namespace AiCodo
{
    public static class Logger
    {
        static List<LogDelegate> _Methods = new List<LogDelegate>();

        static Logger()
        {
            AddLogger(Log4NetLogger.Logger);
        }

        public static void AddLogger(LogDelegate method)
        {
            lock (_Methods)
            {
                if (!_Methods.Contains(method))
                {
                    _Methods.Add(method);
                }
            }
        }

        public static void ClearLogger()
        {
            lock (_Methods)
            {
                _Methods.Clear();
            }
        }

        public static void Log(this object sender, string msg, Category category = Category.Info)
        {
            _Methods.ForEach(m =>
            {
                m(sender, msg, category);
            });
        }
    }
}
