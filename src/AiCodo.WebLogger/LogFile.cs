namespace AiCodo.WebLogger
{

    class LogFile
    {
        int _WriteCount = 0;
        int _ReceivedCount = 0;
        private string _FileName;
        Queue<DynamicEntity> _Queue = new Queue<DynamicEntity>();
        private bool _IsClosed = false;
        private bool _IsWaitTask = false;
        private ManualResetEvent _WaitEvent = new ManualResetEvent(false);

        public event EventHandler<CloseArgs> Closing;
        public event EventHandler Closed;

        public LogFile(string fileName)
        {
            _FileName = fileName;
            Threads.StartNew(StartWrite, $"{fileName.GetName()}");
        }

        public string FileName { get { return _FileName; } }

        public int WriteCount { get { return _WriteCount; } }
        public int ReceivedCount { get { return _ReceivedCount; } }

        public int AddItem(DynamicEntity obj)
        {
            lock (_Queue)
            {
                _Queue.Enqueue(obj);
                Interlocked.Increment(ref _ReceivedCount);

                if (_IsWaitTask)
                {
                    _WaitEvent.Set();
                }
            }
            return _Queue.Count;
        }

        private void StartWrite()
        {
            FileStream fs = new FileStream(_FileName, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            while (true)
            {
                DynamicEntity obj = null;
                lock (_Queue)
                {
                    if (_Queue.Count > 0 && _Queue.TryDequeue(out obj))
                    {
                        if (obj == null)
                        {
                            Console.WriteLine($"obj is null");
                        }
                    }
                    if (obj == null)
                    {
                        continue;
                    }
                }

                try
                {
                    var line = obj.ToJson();
                    sw.WriteLine(line);
                    Interlocked.Increment(ref _WriteCount);
                }
                catch (Exception ex)
                {
                    ex.WriteErrorLog();
                }

                if (_IsClosed)
                {
                    break;
                }

                if (_Queue.Count == 0)
                {
                    _IsWaitTask = true;
                    _WaitEvent.Reset();
                    _WaitEvent.WaitOne(3000);
                    _IsWaitTask = false;
                    if (_Queue.Count == 0)
                    {
                        var closeArgs = new CloseArgs { Closed = true };
                        Closing?.Invoke(this, closeArgs);
                        if (closeArgs.Closed)
                        {
                            _IsClosed = true;
                        }
                        if (_Queue.Count == 0)
                        {
                            break;
                        }
                    }
                }
            }
            sw.Flush();
            sw.Close();
            Closed?.Invoke(this, EventArgs.Empty);
        }

    }
}
