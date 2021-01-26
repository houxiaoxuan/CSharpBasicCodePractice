using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IO_Lock_Debug
{
    internal sealed class IntLock
    {
        public IntLock()
        {
            //初始化为0
            //没有锁
            g_Radom = 0;
        }

        //等于0指示没有锁,此时Lock方法应该返回成功(True)
        //等于1说明存在锁此时Lock方法应该返回失败(False)
        private int g_Radom;

        public bool Lock()
        {
            //原子比较方法
            //如果g_Radom等于0则替换为1且返回0,否则它是返回1的
            return Interlocked.CompareExchange(
                 ref g_Radom, 1, 0) == 0;
        }

        public bool UnLock()
        {
            //原子比较方法
            //如果g_Radom等于1则替换为0且返回1,否则它是返回0的
            return Interlocked.CompareExchange(
                  ref g_Radom, 0, 1) == 1;
        }
    }
    internal sealed class LockLock
    {
        private Hashtable ht = new Hashtable();
        public object this[object key]
        {
            get { return ht[key]; }
            set { ht[key] = value; }
        }
        private bool g_Locked;

        private object g_LockObj = new object();

        public bool Lock()
        {
            lock (g_LockObj)
            {
                if (!g_Locked)
                {
                    g_Locked = true;
                    return true;
                }
                else
                    return false;
            }
        }

        public bool UnLock()
        {
            lock (g_LockObj)
            {
                if (g_Locked)
                {
                    g_Locked = false;
                    return true;
                }
                else
                    return false;
            }
        }
    }
    class Program
    {
        //边写边读
        private static int data = 0;
        private static ReaderWriterLock rw_Lock = new ReaderWriterLock();//读写同步锁
        private const int timeout = 1500;
        private AutoResetEvent auto = new AutoResetEvent(true);
        //一边存到文件内
        private static DataTable datas=new DataTable();
        private static DataRow row;
        private const int maxRows = 100;
        private const int maxSaveRows = 100;
        private static int startRowIndex = 0;
        private static readonly object lockObj = new object();
        private static IntLock intLock;
        private static LockLock lockLock;
        //private static Queue<>
        private static void DataGenerator()
        {
            while(true)
            {
                try
                {
                    rw_Lock.AcquireWriterLock(3000);
                    Interlocked.Increment(ref data);
                    //lock (datas)//no need
                    //{
                    row = datas.NewRow();
                    row.ItemArray = new object[2] { data, DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss.fff") };//other data...
                    datas.Rows.Add(row);
                    //}
                    Console.WriteLine(Thread.CurrentThread.Name + " {0}", data);
                    rw_Lock.ReleaseWriterLock();
                }
                catch( ApplicationException exc)
                {
                    continue;
                }
                finally
                {
                    
                    Thread.Sleep(0);
                }
            }
        }
        private static void ReadAndShow()
        {
            while(true)
            {
                try
                {
                    rw_Lock.AcquireReaderLock(3000);
                    if (row.ItemArray != null)
                    {
                        Console.WriteLine(Thread.CurrentThread.Name + "{0}",  data);
                        Console.WriteLine(Thread.CurrentThread.Name + "{1}    {0}", row.ItemArray[0], row.ItemArray[1]);
                    }
                    rw_Lock.ReleaseReaderLock();
                }
                catch(ApplicationException exc)
                {
                    continue;
                }
                finally
                {
                    
                    Thread.Sleep(1);
                }
            }
        }
        public static string CommaSave(string s)
        {
            if (s.Contains(","))
            {
                s = "\"" + s.Replace("\"", "\"\"") + "\"";
            }
            return s;
        }
        private static void ReadAndSave()
        {
            while (true)
            {

                try
                {
                    rw_Lock.AcquireReaderLock(3000);
                    
                    Interlocked.CompareExchange(ref data, 0, maxRows);
                    //Console.WriteLine(Thread.CurrentThread.Name + "{0}", data);
                    Console.WriteLine(Thread.CurrentThread.Name + "{1}    {0}", row.ItemArray[0], row.ItemArray[1]);
                    int interlock = 0;
                    using (StreamWriter sw = new StreamWriter("D:/debug.csv", true))
                    {
                        string data = "";
                        //写出各行数据
                        if (datas.Rows.Count > 0)
                        {
                            for (int i = startRowIndex; i < datas.Rows.Count; i++)
                            {
                                data = "";
                                for (int j = 0; j < datas.Columns.Count; j++)
                                {
                                    string temp = datas.Rows[i][j]?.ToString();
                                    data += CommaSave(temp);
                                    if (j < datas.Columns.Count - 1)
                                    {
                                        data += ",";
                                    }
                                }
                                sw.WriteLine(data);
                            }
                            startRowIndex = datas.Rows.Count;

                            if (datas.Rows.Count > maxSaveRows)
                            {
                                interlock = 1;
                                //rw_Lock.ReleaseReaderLock();
                                //lock (datas)
                                //{

                                //rw_Lock.AcquireWriterLock(1000);
                                DataTable temp = new DataTable();
                                InitDT(ref temp);
                                for (int i = maxSaveRows; i < datas.Rows.Count; i++)
                                {
                                    temp.ImportRow(datas.Rows[i]);
                                }
                                //Fixme
                                //var cookie = rw_Lock.UpgradeToWriterLock(1000);
                                datas.Clear();
                                datas = temp;
                                //rw_Lock.ReleaseWriterLock();
                                //rw_Lock.DowngradeFromWriterLock(ref cookie);

                                //}
                            }
                        }
                        sw.Close();
                    }
                    rw_Lock.ReleaseReaderLock();
                }
                catch(ApplicationException exc)
                {
                    continue;
                }
                finally
                {
                    Thread.Sleep(5);
                }
                
                
            }
        }
        static void InitDT(ref DataTable dt)
        {
            dt.Columns.Add("data", Type.GetType("System.Int32"));
            dt.Columns.Add("time", Type.GetType("System.String"));
            row = dt.NewRow();
        }
        [STAThread]
        static void Main(string[] args)
        {
            intLock = new IntLock();
            lockLock = new LockLock();
            //lockLock[0] = new LockLock();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Double max = Math.Pow(10,3);
            for (int i=0;i< max; i++)
            {
                intLock.Lock();
                intLock.UnLock();
            }
            stopwatch.Stop();
            Console.WriteLine("InitLock timespan="+ stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();
            stopwatch.Start();
            for (int i = 0; i < max; i++)
            {
                lockLock.Lock();
                lockLock.UnLock();
            }
            stopwatch.Stop();
            Console.WriteLine("lockLock timespan=" + stopwatch.ElapsedMilliseconds);
            InitDT(ref datas);
            //reader、show thread
            Thread reader_show_T = new Thread(new ThreadStart(ReadAndShow));
            reader_show_T.Name = "Show Data";
            
            //writer thread
            Thread writer_T = new Thread(new ThreadStart(DataGenerator));
            writer_T.Name = "Set Data";
            reader_show_T.Start();
            writer_T.Start();

            //reader、save thread
            Thread reader_save_T = new Thread(new ThreadStart(ReadAndSave));
            reader_save_T.Name = "Save Data";
            reader_save_T.Start();

            reader_show_T.Join();
            writer_T.Join();
            Console.ReadKey();

        }
    }
}
