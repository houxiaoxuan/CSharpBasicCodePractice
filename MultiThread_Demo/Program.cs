using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThread_Demo
{
    class Program
    {
        
        static void Main(string[] args)
        {
            
            
            //delegate
            Thread t0 = new Thread(new ThreadStart(delegate { PrintThreadName(); }));
            t0.Name = "t0";
            t0.Start();
            Thread t0_1 = new Thread(PrintThreadName,2);//
            t0_1.Name = "t0_1";
            t0_1.Start();
            t0_1.Join();
            //带参数
            Thread t1 = new Thread(new ParameterizedThreadStart(PrintThreadName));
            t1.Name = "t1";
            t1.Start(1433233);
            #region Action 无返回值，参数可选
            //1.Action 无参数
            Thread t1_1 = new Thread(new ThreadStart(()=>PrintThreadName()));//等效于：Thread t1 = new Thread(new ThreadStart((Action)(()=>PrintThreadName())));
            t1_1.Name = "t1_1";
            t1_1.Start();
            //2.Action 有参数，写法1
            Action<int> action_param = new Action<int>(PrintThreadName_Int);
            Thread t1_2 = new Thread(new ThreadStart(delegate { action_param(1433233); }));
            //Thread t1_2 = new Thread(new ThreadStart(delegate { new Action<int>(PrintThreadName_Int)(1433233); }));//写成一行
            t1_2.Name = "t1_2";
            t1_2.Start();
            //2.Action 有参数，写法2
            Thread t1_3 = new Thread(new ThreadStart(() => PrintThreadName_Int(1433233)));//等效于：Thread t1_2 = new Thread(new ThreadStart((Action<int>)(()=>PrintThreadName())));
            t1_3.Name = "t1_3";
            t1_3.Start();
            //2.Action 有参数，写法3
            AsyncVoidPrint((num) => { PrintThreadName(ref num); Console.WriteLine(num); }, -2);//将创建线程放在异步打印（AsyncPrint）方法内，并lambda构造Action
            #endregion

            #region Func 有返回值，参数可选
            string data = "Default ";
            Func<int, string> func = new Func<int, string>(PrintThreadName_Int_Return_String);
            Thread t2 = new Thread(new ThreadStart(delegate { data+=func(111); }));
            //Thread t2 = new Thread(new ThreadStart(delegate { data+=new Func<int,string>(PrintThreadName_Int_Return_String)(111); }));//上两行写成一行
            t2.Name = "t2";
            t2.Start();
            t2.Join();
            Console.WriteLine("data=" + data);//若不调用t2.Join()则打印出data初始值"Default "，调用后会阻塞控制台主线程等待t2执行结束(data会被重新赋值)
            Console.WriteLine($"data2={func(333)}");
            //写法2
            Func<int, string> func1 = (num)=> { return PrintThreadName_Int_Return_String(num); };
            Thread t2_1 = new Thread(new ThreadStart(delegate { func1(-1); }));
            t2_1.Start();
            t2_1.Join();
            //写法3
            Console.WriteLine($"Sync result={SyncReturnPrint((num) => { return PrintThreadName_Int_Return_String(num); }, 0)}");
            #endregion


            #region *Predicate
            //bool ist0 = isT0(lists[0]);
            List<string> lists = new List<string>() { "T0", "T1" };
            Predicate<string> isT0 = new Predicate<string>(IsT0);//true如果obj满足此委托; 所表示的方法中定义的条件否则为false。
            List<string> T0 = lists.FindAll(isT0);
            //Func<string, bool> func0 =(ss)=>{ return  ss== "T0" ? true : false; };
            List<string> T0_lambda = lists.FindAll(ss => ss == "T0");//lambda表达式写法

            #endregion

            
            //Thread t2 = new Thread(new ThreadStart(new Func<string,string>(PrintThreadName)));
            //t2.Name = "T2";
            //t2.Start();

            //t0.Join();
            Console.ReadKey();
        }
        private static string PrintThreadName()
        {
            string ret =string.Format(Thread.CurrentThread.Name + "{0}{1}", " is running!", Thread.CurrentThread.IsBackground);
            Console.WriteLine(ret);
            return ret;
        }
        private static bool IsT0(string ss)
        {
            return ss == "T0" ? true : false;
        }
        private static void PrintThreadName_Int(int info)
        {
            string ret = string.Format(Thread.CurrentThread.Name + "{0}{1}", " is running! Int=",info);
            Console.WriteLine(ret);
        }
        private static string PrintThreadName_Int_Return_String(int info)
        {
            string ret = string.Format(Thread.CurrentThread.Name + "{0}{1}", " is running! Int=", info);
            Console.WriteLine(ret);
            return ret;
        }
        static void AsyncVoidPrint(Action<int> func, int num)
        {
            Thread t = new Thread(new ThreadStart(delegate { func(num); }));
            t.Start();
        }
        static string SyncReturnPrint(Func<int, string> func, int num)
        {
            string ret = "sync default";
            Thread t = new Thread(new ThreadStart(delegate { ret = func(num); }));
            t.Start();
            t.Join();
            return ret;
        }
        private static void PrintThreadName(ref int obj)
        {
            string ret = string.Format(Thread.CurrentThread.Name + "{0}{1}", " is running! ref int=", obj);
            Console.WriteLine(ret);
            obj++;
        }
        private static void PrintThreadName(object obj)
        {
            string ret = string.Format(Thread.CurrentThread.Name + "{0}{1}", " is running! Object=", obj?.ToString());
            Console.WriteLine(ret);
            
        }
}
}
