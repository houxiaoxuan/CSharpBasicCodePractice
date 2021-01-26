#define DEBUG
//#undef DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            if (istrue)
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
        public MyEventReceiver(MyEvent eventArgs, BackgroundWorker worker)
        {
            Worker = worker;
            eventArgs.eventHandler += new MyEvent.EventHandler(HostHandleEvent);
        }
    }
    #endregion
    /*//委托回调+操作符重载  main
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
            MyEventReceiver myEventReceiver = new MyEventReceiver(myEvent, worker);
            //myEvent.eventHandler += new MyEvent.EventHandler(myEventReceiver.HostHandleEvent);
            myEvent.OnRaise();
            Console.ReadKey();

            Ope oa = new Ope(1, 2);
            Ope ob = new Ope(3, 4);
            Ope v = oa + ob;
        }

        private static void BackgroundWorkerDowork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            Console.WriteLine("has received");
            call.WorkDoneHandler(true);
        }
    }*/

    /*//自定义事件+事件处理委托  main
    class Program
    {
        static void Main(string[] args)
        {
            Counter c = new Counter(new Random().Next(10));
            c.ThresholdReached += c_ThresholdReached;
            c.ThresholdReached += c_ThresholdReached;
            Console.WriteLine("press 'a' key to increase total");
            while (Console.ReadKey(true).KeyChar == 'a')
            {
                Console.WriteLine("adding one");
                c.Add(1);
            }
            Console.ReadKey();
        }

        static void c_ThresholdReached(object sender, ThresholdReachedEventArgs e)
        {
            Console.WriteLine("The threshold of {0} was reached at {1}.", e.Threshold, e.TimeReached);
            //Environment.Exit(0);
        }
    }

    class Counter
    {
        private int threshold;
        private int total;

        public Counter(int passedThreshold)
        {
            threshold = passedThreshold;
        }

        public void Add(int x)
        {
            total += x;
            if (total >= threshold)
            {
                ThresholdReachedEventArgs args = new ThresholdReachedEventArgs();
                args.Threshold = threshold;
                args.TimeReached = DateTime.Now;
                OnThresholdReached(args);
            }
        }

        protected virtual void OnThresholdReached(ThresholdReachedEventArgs e)
        {
            EventHandler<ThresholdReachedEventArgs> handler = ThresholdReached;
            var vvv = handler.GetInvocationList().Length;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<ThresholdReachedEventArgs> ThresholdReached;
    }

    public class ThresholdReachedEventArgs : EventArgs
    {
        public int Threshold { get; set; }
        public DateTime TimeReached { get; set; }
    }*/

    /* //AutoResetEvent多线程同步 main
    class Program
    {
        public static AutoResetEvent resetEvent = new AutoResetEvent(true);
        private static int Max = 100;
        static void Main(string[] args)
        {
            Thread t = new Thread(new ThreadStart(ReadThread));
            t.Start();
            int i = 0;
            while(i++<10)
            {
                Console.WriteLine("Write: " + i);
                //resetEvent.Set();
                Thread.Sleep(1);
                //Thread.SpinWait(1);
            }
        }
        private static void ReadThread()
        {
            while(true)
            {
                var ret=resetEvent.WaitOne();
                Console.WriteLine("Read: "+Max);
            }
        }
    }
    */

    /*//Generic demo
    // type parameter T in angle brackets
    public class GenericList<T>
    {
        // The nested class is also generic on T.
        private class Node
        {
            // T used in non-generic constructor.
            public Node(T t)
            {
                next = null;
                data = t;
            }

            private Node next;
            public Node Next
            {
                get { return next; }
                set { next = value; }
            }

            // T as private member data type.
            private T data;

            // T as return type of property.
            public T Data
            {
                get { return data; }
                set { data = value; }
            }
        }

        private Node head;

        // constructor
        public GenericList()
        {
            head = null;
        }

        // T as method parameter type:
        public void AddHead(T t)
        {
            Node n = new Node(t);
            n.Next = head;
            head = n;
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node current = head;

            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }
    }
    class TestGenericList
    {
        static void Main()
        {
            // int is the type argument
            GenericList<int> list = new GenericList<int>();

            for (int x = 0; x < 10; x++)
            {
                list.AddHead(x);
            }

            foreach (int i in list)
            {
                System.Console.Write(i + " ");
            }
            System.Console.WriteLine("\nDone");
        }
    }
    public class BasicClass
    {
        public BasicClass()
        {

        }
    }
    public struct BasicStruct
    {

    }
    class GenericClass<T, Y> where T : class, IList<Y>, new()
        where Y : struct
    {

    }
    class GenericClass2<T> where T : new()
    {

    }
    class Program111
    {
        delegate T NumberChanger<T>(T n);
        static int num = 10;
        public static int AddNum(int p)
        {
            num += p;
            return num;
        }

        public static int MultNum(int q)
        {
            num *= q;
            return num;
        }
        public static int getNum()
        {
            return num;
        }

        static void Main2(string[] args)
        {
            GenericClass<List<BasicStruct>, BasicStruct> generic = new GenericClass<List<BasicStruct>, BasicStruct>();
            foreach (string s in args)
            {
                Console.WriteLine(s);
            }
            // 创建委托实例
            NumberChanger<int> nc1 = new NumberChanger<int>(AddNum);
            NumberChanger<int> nc2 = new NumberChanger<int>(MultNum);
            // 使用委托对象调用方法
            nc1(25);
            Console.WriteLine("Value of Num: {0}", getNum());
            nc2(5);
            Console.WriteLine("Value of Num: {0}", getNum());
            Console.ReadKey();
        }
    }
    */

    #region 继承、封装、多态
    class A
    {
        public void FuncA()
        {
            Console.WriteLine("A.A");
        }
        public virtual void FuncB()
        {
            Console.WriteLine("A.B");
        }
        public void FuncC()
        {
            Console.WriteLine("A.C");
        }
    }
    class B : A
    {
        public new void FuncA()
        {
            Console.WriteLine("B.A");
        }
        public override void FuncB()//重写后类型B实例中的FuncB函数指针不再指向base.FuncB
        {
            Console.WriteLine("B.B");
        }
        public new void FuncC()//new出来的和父类同名的函数
        {
            Console.WriteLine("B.C");
        }
    }

    #endregion
    #region  C#指针
    class Test
    {
        static void Display(long x) => Console.WriteLine($"{Convert.ToString(x, toBase: 2),100}");
        public struct Coords
        {
            public int X;
            public int Y;
            public override string ToString() => $"({X}, {Y})";
        }

        public class PointerMemberAccessExample
        {
            public static unsafe void Main1()
            {
                object obj = new object();
                Coords coords;
                Coords* p = &coords;
                p->X = 3;
                p->Y = 4;
                p->X += 1;
                *p = new Coords();
                Console.WriteLine(p->ToString());  // output: (3, 4)
            }
        }
        static void Main1(string[] args)
        {
            A a = new A();
            a.FuncA();//A.A
            a.FuncB();//A.B
            a.FuncC();//A.C
            B b = new B();
            b.FuncA();//B.A
            b.FuncB();//B.B
            b.FuncC();//B.C
            A b1 = new B();
            b1.FuncA();//A.A
            b1.FuncB();//B.B ->override
            b1.FuncC();//A.C ->

            //long i0 = 1000_0000_0001_1110;
            //long i1 = 1000_0000_0001_1110_1111;
            //long i2 = 0b1000_0000_0001_1110_1111;

            //long i3 = 0100_0000_0001_1110_1111;
            //long i4 = 0100_0000_0001_1110_1111_1111_1111;
            //long i5 = 0b0100_0000_0001_1110_1111_1111_1111;


            unsafe
            {
                const int Count = 3;
                int[] numbers = new int[Count] { 10, 20, 30 };
                fixed (int* pointerToFirst = &numbers[0])//&获取对象的地址
                {
                    *pointerToFirst += 1;//*获取指针对应的值
                    int* pointerToLast = pointerToFirst + (Count - 1);

                    Console.WriteLine($"Value {*pointerToFirst} at address {(long)pointerToFirst}");
                    Console.WriteLine($"Value {*pointerToLast} at address {(long)pointerToLast}");
                }
                numbers[0] += 1;


            }
            // Output is similar to:
            // Value 10 at address 1818345918136
            // Value 30 at address 1818345918144
            unsafe
            {
                int* numbers = stackalloc int[] { 0, 1, 2, 3, 4, 5 };
                int* p1 = &numbers[1];
                int p1_1 = numbers[1];
                int* p2 = &numbers[5];
                Console.WriteLine(p2 - p1);  // output: 4
            }
            unsafe
            {
                char* pointerToChars = stackalloc char[123];

                for (int i = 65; i < 123; i++)
                {
                    pointerToChars[i] = (char)i;
                }

                Console.Write("Uppercase letters: ");
                for (int i = 0; i < 124; i++)
                {
                    Console.Write(pointerToChars[0]);
                }
                for (int i = 65; i < 91; i++)
                {
                    Console.Write(pointerToChars[i]);
                }
            }
            // Output:
            // Uppercase letters: ABCDEFGHIJKLMNOPQRSTUVWXYZ
            unsafe
            {
                char letter = 'A';
                char* pointerToLetter = &letter;
                Console.WriteLine($"Value of the `letter` variable: {letter}");
                Console.WriteLine($"Address of the `letter` variable: {(long)pointerToLetter:X}");

                *pointerToLetter = 'Z';
                Console.WriteLine($"Value of the `letter` variable after update: {letter}");
            }
            // Output is similar to:
            // Value of the `letter` variable: A
            // Address of the `letter` variable: DCB977DDF4
            // Value of the `letter` variable after update: Z

            //Display(i0);
        }
    }
    #endregion
    //定义测试方法和异步委托
    public class AsyncDemo
    {
        // The method to be executed asynchronously.
        public string TestMethod(int callDuration, out int threadId)
        {
            Console.WriteLine("Test method begins.");
            Thread.Sleep(callDuration);
            Console.WriteLine("Test method ends.");
            threadId = Thread.CurrentThread.ManagedThreadId;
            return String.Format("My call time was {0}.", callDuration.ToString());
        }
    }
    //通过异步方式调用同步方法
    // The delegate must have the same signature as the method
    // it will call asynchronously.
    public delegate string AsyncMethodCaller(int callDuration, out int threadId);
    public class AsyncMain
    {
        public static void Main_Outdated()
        {
            // The asynchronous method puts the thread id here.
            int threadId;

            // Create an instance of the test class.
            AsyncDemo ad = new AsyncDemo();

            // Create the delegate.
            AsyncMethodCaller caller = new AsyncMethodCaller(ad.TestMethod);

            // Initiate the asychronous call.
            IAsyncResult result = caller.BeginInvoke(3000,
                out threadId, null, null);

            Thread.Sleep(0);
            Console.WriteLine("Main thread {0} does some work.",
                Thread.CurrentThread.ManagedThreadId);


            /* //1.使用 EndInvoke 等待异步调用
            // Call EndInvoke to wait for the asynchronous call to complete,
            // and to retrieve the results.
            string returnValue = caller.EndInvoke(out threadId, result);*/


            /*//2.使用 WaitHandle 等待异步调用
            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();//
            // Perform additional processing here.
            Console.WriteLine("This is additional process");
            // Call EndInvoke to retrieve the results.
            string returnValue = caller.EndInvoke(out threadId, result);
            // Close the wait handle.
            result.AsyncWaitHandle.Close();*/


            //3.对异步调用的完成情况进行轮询
            // Poll while simulating work.
            while (result.IsCompleted == false)
            {
                Thread.Sleep(250);
                Console.Write(".");
            }
            // Call EndInvoke to retrieve the results.
            string returnValue = caller.EndInvoke(out threadId, result);



            Console.WriteLine("The call executed on thread {0}, with return value \"{1}\".",
                threadId, returnValue);
        }

    }
    [Description("C# 8.0 Feature test")]
    class Debug_Class_8_0
    {
        public readonly Stack ss5 = new Stack();
        readonly ArrayList ss2 = new ArrayList();
        readonly Queue<string> ss3 = new Queue<string>();
        readonly Hashtable ss4 = new Hashtable();
        static readonly int[] ss6 = new int[3] { 1, 2, 3 };


        //值类型、引用类型、ref
        static PHY phy1 = new PHY("1");
        static PHY phy2 = new PHY("2");
        static Product real = new Product();
        static Product lib = new Product();
        static Product upper = new Product();
        public enum Rainbow
        {
            Red,
            Orange,
            Yellow,
            Green,
            Blue,
            Indigo,
            Violet
        }
        static void UpdateStatus(string i)
        {
            lib.iop_entity.Data=Convert.ToInt32(i);
        }
        static void UpdateStatus(string i,ref Product product)
        {
            product.iop_entity.Data = Convert.ToInt32(i);
        }
        static void Change(string ss)
        {
            ss = "changed";
        }
        static void Change(ref string  ss)
        {
            ss = "changedRef";
        }
        static void Update(PHY phy)
        {
            phy.refData = "changedObj";
        }
        static void Update(ref PHY phy)
        {
            phy.refData = "changedObjRef";
        }
        static void Add(ref int i)
        {
            i++;
            //Add(i);//不会修改传入的值
        }
        static void Add(int i)
        {
            i++;
        }
        /*
        static void Main()
        {
            
            //a1 = new Board1();
            //a1.Change(1);

            //b2 = new Product();
            //不变
            //b2.a_in_b = b1.a_in_b;
            //b1.a_in_b = a1;
            //b2 = b1;//修改引用对象下的局部变量两者都可以改变
            lib.iop_entity = real.iop_entity;
            upper.iop_entity = lib.iop_entity;
            real.iop_entity.Data = 2;
            lib.iop_entity.Data = 3;
            upper.iop_entity.Data = 4;
            //phy1 = upper.iop_entity;
            lib.iop_entity.ChangePhy("3");//changed
            upper.iop_entity.ChangePhy("4");//changed
            upper.iop_entity.refData = "5";
            //phy1.Data = "5";
            lib.iop_entity = phy2;

            //upper = real;
            //real.iop_entity = phy1;
            //Thread t = new Thread(new ThreadStart(delegate { b2.a_in_b = new Board1(); ; }));
            //t.Start();
            //t.Join();
            int i1 = 1;
            int i2 = i1;
            upper.iop_entity.Data = lib.iop_entity.Data;
            upper.iop_entity.Data++;
            Add(upper.iop_entity.Data);
            Add(ref upper.iop_entity.Data);
            Change(upper.iop_entity.refData);
            Change(ref upper.iop_entity.refData);
            Update(upper.iop_entity);
            Update(ref upper.iop_entity);
            upper.iop_entity = lib.iop_entity;


            upper.iop_entity.Data = lib.iop_entity.Data;
            UpdateStatus("4");
            UpdateStatus("3", ref lib);
            //b2.a_in_b=a1;
            //a1.Data+=1;
            //b1.a_in_b.Change(2);
            //b1.a_in_b.Change(2,ref b2.a_in_b);
            string[] ss = new string[2] { "", "" };
            ArrayList ss2 = new ArrayList();
            Queue<string> ss3 = new Queue<string>();
            Hashtable ss4 = new Hashtable();
            FromRainbow(Rainbow.Red);
            int x = 3;
            switch (x)
            {
                case 3:
                    int ret;
                    Func<int, int> func = x =>
                      {
                          ret = x * 3;
                          return ret;
                      };
                    func(3);
                    break;
            }
            var fn = CalcuMultiArgs<int, int, int>(f => (x, y) =>
            {
                var tmpresult = x * y;
                if (x > 0 || y > 0)
                {
                    return tmpresult + f(x - 1, y - 1);
                }
                else
                {
                    return tmpresult;
                }
            });
            var result = fn(3, 2);
            Console.WriteLine(result);

            //var (a, b, option) = (10, 5, "+");
            //var example1 = option switch
            //{
            //    "+" => a + b,
            //    "-" => a - b,
            //    _ => a * b,
            //};

            
            List<object> values = new List<object>();
            foreach(var item in values)
            {
                var (child_is_int, child_is_list) = (0,0);
                switch(item)
                {
                    case int int_item when int_item>0://类型模式+限定
                        child_is_int += int_item;
                        break;
                    case List<int> intList_item:
                        child_is_list =  intList_item.Sum();//Linq
                        break;
                }
            }

            DataTable dataTable = new DataTable();
            
            var value = 25;
            int example3 = value switch
            {
                _ when value > 10 => Calcu(10),
                _ when value <= 10 => 1
            };
            int Calcu(int value)
            {
                return value * 10;
            }
            Console.WriteLine("Example 3 : " + example3);

            string Null = null;
            var exaample4 = Null switch
            {
                _ when Null==null=>"",
                null => Null ?? "null",
            };

        }
        */
        class PHY
        {
            public PHY(string id)
            {
                refData = id;
            }
            public string refData = "";
            public int Data = 1;

            public void ChangePhy(string i)
            {
                this.refData= i;
            }
            public void Change(string i,PHY class_ref)
            {
                class_ref.ChangePhy(i);
            }
            public void Change(string i,ref PHY class_ref)
            {
                class_ref.ChangePhy(i);
            }
        }
        class Product
        {
            public PHY iop_entity;
            public Product()
            {
                iop_entity = new PHY("1");
            }
        }

        //public static Func<Rainbow, List<int>> GetColor<Rainbow, List<int>>()
        /// <summary>
        /// 多参递归函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func_arg"></param>
        /// <returns></returns>
        public static Func<T, T1, TResult> CalcuMultiArgs<T, T1, TResult>(Func<Func<T, T1, TResult>, Func<T, T1, TResult>> func_arg)
        {
            return (x, y) => func_arg(CalcuMultiArgs(func_arg))(x,y);
            //return (x, y) => func_arg(CalcuMultiArgs(func_arg))(x, y);
        }
        
        //static Func<Rainbow, List<int>> isRed = colorBand =>
        //{
        //    Console.WriteLine("oh,this is red color!");
        //    return new List<int> { 0xFF, 0x00, 0x00 };
        //};
        public static List<int> FromRainbow(Rainbow colorBand) =>
    colorBand switch
    {
        Rainbow.Red=>new List<int> { 0xFF, 0x00, 0x00 },
        Rainbow.Orange => new List<int> { 0xFF, 0x7F, 0x00 },
        Rainbow.Yellow => new List<int>{0xFF, 0xFF, 0x00 },
        Rainbow.Green => new List<int>{0x00, 0xFF, 0x00 },
        Rainbow.Blue => new List<int>{0x00, 0x00, 0xFF },
        Rainbow.Indigo => new List<int>{0x4B, 0x00, 0x82 },
        Rainbow.Violet => new List<int>{0x94, 0x00, 0xD3 },
        _ => throw new ArgumentException(message: "invalid enum value", paramName: colorBand.ToString()),
    };

    }
    //4.异步调用完成时执行回调方法
    #region 可变字段
    class Test22
    {
        public static int result;
        public static volatile bool finished;

        static void Thread2()
        {
            result = 143;
            finished = true;
        }
        static void Cal(double value,out int x,out int y)
        {
            //if (value >= 0)
            //{
                x = (int)Math.Floor(value);
                y = (int)((value - x) * 10000);
            //}
            //else
            //{
            //    x = (int)Math.Ceiling(value);
            //    y = -(int)((value - x) * 10000);
            //}
        }

        /// <summary>
        /// 负数转十六进制
        /// </summary>
        /// <param name="iNumber"></param>
        /// <returns></returns>
        private static string NegativeToHexString(int iNumber)
        {
            string strResult = string.Empty;

            if (iNumber < 0)
            {
                iNumber = -iNumber;

                string strNegate = string.Empty;

                char[] binChar = Convert.ToString(iNumber, 2).PadLeft(8, '0').ToArray();

                foreach (char ch in binChar)
                {
                    if (Convert.ToInt32(ch) == 48)
                    {
                        strNegate += "1";
                    }
                    else
                    {
                        strNegate += "0";
                    }
                }

                int iComplement = Convert.ToInt32(strNegate, 2) + 1;

                strResult = Convert.ToString(iComplement, 16).ToUpper();
            }

            return strResult;
        }
        static string  Int16ToHex(Int16 value)
        {
            //string ret = string.Empty;
            //string bin = Convert.ToString(value, 2);
            return  String.Format("{0:X}", value);
        }
        private static byte[] ConvertHex(Int16 vel)
        {
            Int16 velocity = vel;
            byte[] hex = new byte[2];
            hex[0] = (byte)(velocity & 0xff);
            hex[1] = (byte)((velocity >> 8) & 0xff);   //先右移再与操作
            //hex[2] = (byte)((velocity >> 16) & 0xff);
            //hex[3] = (byte)((velocity >> 24) & 0xff);
            return hex;
        }
        static void Main()
        {
            Int16 tempHead = (Int16)(1 + 0x37);
            byte[] b_ret = new byte[1] { Convert.ToByte(tempHead) };
            int[] args = new int[] { 0, 42, 24, 4994 };
            foreach (int item in args)
            {
                byte[] temp = ConvertHex((Int16)item);
                b_ret = b_ret.Concat(temp).ToArray();
            }
            string e_s =  Encoding.Default.GetString(b_ret);
            Console.WriteLine("ES="+e_s);

            Decoder decoder = Encoding.Default.GetDecoder();
            //计算字节数组对应的字符数组长度;
            byte[] bytes = new byte[e_s.Length];
            for (int i = 0; i < e_s.Length; i++)
            {
                bytes[i] = Convert.ToByte(e_s.ToCharArray()[i]);
            }
            int charSize = decoder.GetCharCount(bytes, 0, bytes.Length);
            Char[] chs = new char[charSize];
            //进行字符转换;
            int charLength = decoder.GetChars(bytes, 0, e_s.Length, chs, 0);
            Console.WriteLine("DS="+new string(chs));

            int x, y = 0;
            byte[] b=ConvertHex(-2);
            string ss = NegativeToHexString(-1);
            ss = NegativeToHexString(-2);
            ss=Int16ToHex(-1);
            ss=Int16ToHex(-2);
            string head = Convert.ToString(-1, 16).ToUpper().PadLeft(4, Convert.ToChar("0"));
            Cal(1.1,out x,out y);
            Cal(0.1, out x, out y);
            Cal(-0.1, out x, out y);
            Cal(-1.1, out x, out y);
            finished = false;

            // Run Thread2() in a new thread
            new Thread(new ThreadStart(Thread2)).Start();

            // Wait for Thread2 to signal that it has a result by setting
            // finished to true.
            for (; ; )
            {
                if (finished)
                {
                    Console.WriteLine("result = {0}", result);
                    return;
                }
                else
                {
                    Console.WriteLine("wait...");
                }
            }
        }
    }
    #endregion
}


