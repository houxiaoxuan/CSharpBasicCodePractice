using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spinlock_Debug
{
    class Program
    {
        private static readonly object lockObj = new object();
        static void Main(string[] args)
        {
            
            while (true)
            {

                var count = 0;
                var taskList = new Task[10];
                Stopwatch sp = new Stopwatch();
                sp.Start();
                string s = string.Empty;
                // 不要意外复制。每个实例都是独立的。
                SpinLock _spinLock = new SpinLock();
                for (int i = 0; i < taskList.Length; i++)
                {
                    taskList[i] = Task.Run(() =>
                    {
                        bool _lock = false;
                        for (int j = 0; j < 10_000; j++)
                        {
                            //_spinLock.Enter(ref _lock);
                            Interlocked.Increment(ref count);
                            lock (lockObj)
                            {
                                s += count + Environment.NewLine;
                            }
                            //Thread.SpinWait(0);
                            //count++;
                            //_spinLock.Exit();
                            _lock = false;
                        }
                    });
                }
                //System.Threading.SpinWait.SpinUntil(() => m_IsWork2Start);

                Task.WaitAll(taskList);
                sp.Stop();
                Console.WriteLine($"完成! 耗时:{sp.ElapsedMilliseconds}");
                Console.WriteLine($"结果:{count}");
                byte[] bytes = Encoding.Default.GetBytes(s);
                FileStream writer = new FileStream("D:/Debug.txt", FileMode.OpenOrCreate);
                writer.BeginWrite(bytes, 0, s.Length, new AsyncCallback(endWrite), writer);
                Console.ReadKey();
            }
        }
        private static void endWrite(IAsyncResult asr)
        {
            using (Stream str = (Stream)asr.AsyncState)
            {
                str.EndWrite(asr);
            }
        }
    }
}
