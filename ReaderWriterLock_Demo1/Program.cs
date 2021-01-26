using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDebug
{

    class Program
    {
        private static ReaderWriterLock m_readerWriterLock = new ReaderWriterLock();
        //private static AutoResetEvent auto = new AutoResetEvent(false);
        private static int m_int = 0;
        [STAThread]
        static void Main(string[] args)
        {
            Thread readThread = new Thread(new ThreadStart(Read));
            readThread.Name = "1 Get";
            Thread readThread2 = new Thread(new ThreadStart(Read));
            readThread2.Name = "2 Get";
            Thread writeThread = new Thread(new ParameterizedThreadStart(Writer));
            writeThread.Name = "Set";
            readThread.Start();
            readThread2.Start();
            writeThread.Start();
            readThread.Join();
            readThread2.Join();
            writeThread.Join();

            Console.ReadKey();
        }
        private static void Read()
        {
            while (true)
            {
                Console.WriteLine(Thread.CurrentThread.Name + " Lock");
                m_readerWriterLock.AcquireReaderLock(200);
                Console.WriteLine(String.Format("{0} m_int : {1}", Thread.CurrentThread.Name, m_int));
                m_readerWriterLock.ReleaseReaderLock();
                Thread.Sleep(1001);
            }
        }

        private static void Writer(object oo)
        {
            while (true)
            {
                Console.WriteLine(Thread.CurrentThread.Name + " Lock");
                m_readerWriterLock.AcquireWriterLock(1000);
                //Interlocked.Increment(ref m_int);
                m_int += 10;
                m_readerWriterLock.ReleaseWriterLock();
                Console.WriteLine(Thread.CurrentThread.Name + " Unlock");
                Thread.Sleep(1000);
            }
        }
    }
}
