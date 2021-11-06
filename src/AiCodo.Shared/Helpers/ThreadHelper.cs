/* 
 * author      : singba 
 * email       : singba@163.com 
 * version     : 20210831
 * package     : AiCodo
 * license     : MIT
 * description : let me think a while
 */
namespace AiCodo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    public class DelayAction
    {
        private DateTime _ActiveTime = DateTime.Now;
        private bool _IsStart = false;
        private object _ThreadLock = new object();

        public DelayAction(Action action, int delaySeconds = 5)
        {
            _Do = action;
            _DelaySeconds = delaySeconds;
        }

        public bool IsStart
        {
            get
            {
                return _IsStart;
            }
        }

        #region 属性 Do
        private Action _Do = null;
        public Action Do
        {
            get
            {
                return _Do;
            }
            set
            {
                _Do = value;
            }
        }
        #endregion

        #region 属性 DelaySeconds
        private int _DelaySeconds = 5;
        public int DelaySeconds
        {
            get
            {
                return _DelaySeconds;
            }
            set
            {
                if (_DelaySeconds == value)
                {
                    return;
                }
                if (value < 1)
                {
                    throw new Exception("延时时间不能小于1秒");
                }
                _DelaySeconds = value;
            }
        }
        #endregion

        public void CheckStart()
        {
            _ActiveTime = DateTime.Now;
            if (_IsStart)
            {
                return;
            }
            lock (_ThreadLock)
            {
                if (_IsStart)
                {
                    return;
                }
                _IsStart = true;
                Threads.StartNew(Check);
            }
        }

        public void Cancel()
        {
            if (_IsStart)
            {
                _IsStart = false;
            }
        }

        private void Check()
        {
            while (_IsStart)
            {
                Thread.Sleep(_DelaySeconds * 1000);
                if (_ActiveTime.AddSeconds(_DelaySeconds) <= DateTime.Now)
                {
                    try
                    {
                        _Do?.Invoke();
                        break;
                    }
                    catch (Exception ex)
                    {
                        "DelayActon".Log(ex.Message);
                    }
                }
            }
            if (_IsStart)
            {
                _IsStart = false;
            }
        }
    }

    public static class Threads
    {
        static object _NewThreadLock = new object();

        static int _MaxNewThreads = 1000;

        static int _NewThreads = 0;

        static int _NewWorkThreads = 0;

        static Threads()
        {
            var threadPoolSize = System.Configuration.ConfigurationManager.AppSettings["ThreadPoolSize"];
            if (!string.IsNullOrEmpty(threadPoolSize))
            {
                var sizes = threadPoolSize.Split(',');
                var wsize = Convert.ToInt32(sizes[0]);
                var ioSize = sizes.Length > 1 ? Convert.ToInt32(sizes[1]) : wsize;
                ThreadPool.SetMaxThreads(wsize, ioSize);
            }
            var maxNewThreads = System.Configuration.ConfigurationManager.AppSettings["MaxNewThreads"];
            if (!string.IsNullOrEmpty(maxNewThreads))
            {
                _MaxNewThreads = Convert.ToInt32(maxNewThreads);
            }

        }

        private static void Execute(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                "Threads".Log(ex.Message);
            }
        }

        public static Thread StartNew(Action action, string name = "")
        {
            //lock (_NewThreadLock)
            //{
            //    if (_NewWorkThreads >= _MaxNewThreads)
            //    {
            //        AddToThreadPool(action, name);
            //        Debug.WriteLine($"threads {_NewWorkThreads}/ {_MaxNewThreads}");
            //        return null;
            //    }
            //}
            if (string.IsNullOrEmpty(name))
            {
                StackTrace trace = new StackTrace(true);
                if (trace.FrameCount > 1)
                {
                    var f = trace.GetFrame(1);
                    var m = f.GetMethod();
                    name = $"{m.Module.Name}-{m.MemberType}-{m.Name}-{ f.GetFileLineNumber()}";
                }
            }

            Interlocked.Increment(ref _NewThreads);
            Interlocked.Increment(ref _NewWorkThreads);
            ThreadStart start = new ThreadStart(() =>
            {
                __SafeAction(action, name);
                Interlocked.Decrement(ref _NewWorkThreads);
            });

            Thread thr = new Thread(start);
            thr.Name = name;
            thr.IsBackground = true;
            thr.Start();
            return thr;
        }

        private static void __SafeAction(Action action, string name = "")
        {
            try
            {
                action();
                //"Threads".Log($"thread {name} end", Category.Debug);
            }
            catch (Exception ex)
            {
                "Threads".Log($"thread {name} {ex.ToString()}", Category.Exception);
            }
        }

        //private static void __SafeAction(object actionObject)
        //{
        //    try
        //    {
        //        ((Action)actionObject)();
        //        "Threads".Log($"thread end", Category.Debug);
        //    }
        //    catch (Exception ex)
        //    {
        //        "Threads".Log(ex.ToString(), Category.Exception);
        //    }
        //}

        /// <summary>
        /// 不是很着急的零碎任务
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool AddToThreadPool(Action action, string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                StackTrace trace = new StackTrace(true);
                if (trace.FrameCount > 1)
                {
                    var f = trace.GetFrame(1);
                    var m = f.GetMethod();
                    name = $"ThreadPool {m.Module.Name}-{m.MemberType}-{m.Name}-{ f.GetFileLineNumber()}";
                }
            }
            return ThreadPool.QueueUserWorkItem((obj) =>
            {
                __SafeAction(action, name);
            });
        }

        public static void DelayRun(Action action, double checkSeconds, Func<bool> canRun, Func<bool> isCancel, Action callback = null)
        {
            StartNew(() =>
            {
                while (!isCancel())
                {
                    if (canRun())
                    {
                        action();
                        break;
                    }
                    Thread.Sleep((int)(checkSeconds * 1000));
                }
                callback?.Invoke();
            });
        }
    }
}
