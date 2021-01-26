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
                            _spinLock.Enter(ref _lock);
                            Interlocked.Increment(ref count);
                            //若要打印或保存s，需要使用互斥锁以保证线程间的同步；此时的自旋锁反而会因为粒度过大导致不能起到很好的效果
                            //lock (lockObj)
                            //{
                            //    s += count + Environment.NewLine;
                            //}
                            //Thread.SpinWait(0);//如果需要等待某个条件满足的时间很短，而且不希望发生上下文切换，基于自旋的【等待】是一种很好的解决方案。
                            _spinLock.Exit();
                            _lock = false;
                        }
                    });
                }
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
