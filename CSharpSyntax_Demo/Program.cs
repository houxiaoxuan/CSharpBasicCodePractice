#define DEBUG
//#undef DEBUG
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpSyntax_Demo
{
    #region 操作符重载
    public class Ope
    {

        private int a;
        private int b;
        public Ope()
        {
            a = 0;
            b = 0;
        }
        public Ope(int v1, int v2)
        {
            this.a = v1;
            this.b = v2;
        }

        public static Ope operator +(Ope oa, Ope ob)
        {
            Ope op = new Ope();
            op.a = oa.a + ob.a;
            op.b = oa.b + ob.b;
            return op;
        }

    }

    #endregion
    #region 委托回调
    class DelegateTest
    {
        public delegate void Workdone(bool IsTrue);
        public void ToWork(Workdone workdone)
        {
            workdone(true);
        }

        public void WorkDoneHandler(bool istrue)
        {
            if(istrue)
                Console.WriteLine("Work has done!");
        }
    }
    //接口回调
    class MyBack : IBack
    {
        public void Run(bool isFalse)
        {
            Console.WriteLine(DateTime.Now);
        }
    }
    public class Controller
    {
        public IBack Back = null;
        public Controller(IBack back)
        {
            Back = back;
        }
        public void Start()
        {
            Console.WriteLine("敲键盘任意键就显示当前的时间，直到按ESC退出...."); 
            while (Console.ReadKey(true).Key != ConsoleKey.Escape) { Back.Run(true); }
        }
    }
    public interface IBack
    {
        void Run(bool isFalse);
    }
    //事件
    
    public class MyEvent
    {
        public delegate void EventHandler(bool isFalse);
        public event EventHandler eventHandler;

        public void OnRaise()
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            eventHandler(true);
        }
    }
    public class MyEventReceiver
    {
        public BackgroundWorker Worker;
        public void HostHandleEvent(bool isFalse)
        {
            Worker.RunWorkerAsync();
        }
        public MyEventReceiver(MyEvent eventArgs,BackgroundWorker worker)
        {
            Worker = worker;
            eventArgs.eventHandler += new MyEvent.EventHandler(HostHandleEvent);
        }
    }
    #endregion

    class Program
    {
        private static DelegateTest call = new DelegateTest();
        static void Main(string[] args)
        {
            DelegateTest.Workdone test = new DelegateTest.Workdone(call.WorkDoneHandler);
            call.ToWork(test);
            //Controller cc = new Controller(new MyBack());
            //cc.Start();
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            MyEvent myEvent = new MyEvent();
            //myEvent.eventHandler += new MyEvent.EventHandler(call.WorkDoneHandler);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(BackgroundWorkerDowork);
            MyEventReceiver myEventReceiver= new MyEventReceiver(myEvent, worker);
            //myEvent.eventHandler += new MyEvent.EventHandler(myEventReceiver.HostHandleEvent);
            myEvent.OnRaise();
            Console.ReadKey();

            Ope oa = new Ope(1,2);
            Ope ob = new Ope(3, 4);
            Ope v = oa + ob;
        }
        private static void BackgroundWorkerDowork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            Console.WriteLine("has received");
            call.WorkDoneHandler(true);
        }
    }
}


