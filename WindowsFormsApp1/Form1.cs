using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public static BackgroundWorker Worker;
        private static AssistClass assistClass = new AssistClass();

        //.NET 中的事件基于委托模型。 委托模型遵循观察者设计模式，使订阅者能够向提供方注册并接收相关通知。 事件发送方推送事件发生的通知，事件接收器接收该通知并定义对它的响应。 
        //委托，是一种保存对方法的引用的类型。委托是通过显示所引用方法的返回类型和参数的签名来声明的，并可以仅保存与其签名匹配的方法的引用。 因此，委托等同于类型安全函数指针或回叫。 委托声明足以定义委托类。
        //事件,是由对象发送的用于发出操作信号的消息
        //Lambda

        //1.声明委托，定义签名（即方法的返回值类型和参数列表类型）
        private delegate void MyEventHandler<TEventArgs>(object sender, MyEventArgs args);
        
        private event MyEventHandler<MyEventArgs> MyEvent;//事件处理程序委托的标准签名定义不返回值的方法
        //2.定义事件数据
        public class MyEventArgs : EventArgs
        {
            public string Info;
            //...
        }
        //3.1定义触发事件的方法
        public void OnMyEvent(object sender, MyEventArgs args)
        {
            //异步执行事件上锁绑定委托的方法
            MyEvent?.BeginInvoke(sender,args,null,null);
        }
        //3.2 定义事件接收/处理方法(TEventArgs为EventArgs的泛型，使用EventHandler<TEventArgs> 的优点 是，如果事件生成事件数据，则不需要编写自己的自定义委托代码。 只需提供事件数据对象的类型作为泛型参数)
        public void Handler<TEventArgs>(object sender, TEventArgs args)
        {
            MessageBox.Show(sender?.ToString() + (args as MyEventArgs)?.Info.ToString());
        }
        
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "InitSuccess";
            Worker = new BackgroundWorker();
            Worker.DoWork += new DoWorkEventHandler(DoWork);
            Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
            if (!Worker.IsBusy)
                Worker.RunWorkerAsync();
            //4.将委托实例添加到事件
            MyEvent += new MyEventHandler<MyEventArgs>(Handler);
            //MyEvent -= new MyEventHandler<MyEventArgs>(Handler);//移除事件
            //5.触发事件
            OnMyEvent(null, new MyEventArgs() { Info = "This is async message!" }); ;
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Thread.Sleep(2000);
            textBox1.Text = "WorkerCompleted";
        }
        private delegate void EnableButtonCallBack(bool IsEnable);
        private delegate void ChangeTextCallBack();
        //public AssistClass.AssistCallBack assistCall = new AssistClass.AssistCallBack(assistClass.Run);
        private void DoWork(object sender, DoWorkEventArgs e)
        {

            //EnableButtonCallBack btnCall = new EnableButtonCallBack();
            while (AssistClass.count < 1000)
            {

                bool ret = assistClass.Add(()=>
                {
                    textBox1.BeginInvoke(new EnableButtonCallBack(IsEnable), new object[] { !AssistClass.isPausing });
                });
                if (!ret)
                    AssistClass.resetEvent.WaitOne();
                textBox1.BeginInvoke(new ChangeTextCallBack(ChangeText));//可以改变控件内容
            }
            //ChangeText();//不会改变控件内容
            //textBox1.Invoke(new EnableButtonCallBack(ChangeText));//可以改变控件内容
            //Thread.Sleep(1000);
        }
        private void IsEnable(bool isEnable)
        {
            textBox1.Enabled = isEnable;
        }
        private void ChangeText()
        {
            textBox1.Text = AssistClass.count.ToString();
        }
        private void ChangeText(object sender,EventArgs args)
        {
            textBox2.Text = args.ToString();
        }
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    if (!AssistClass.isPausing)
        //        AssistClass.Pause();
        //    else
        //        AssistClass.Resume();
        //}

        public delegate bool asyncChangeText(string text, ref MyEventArgs ret);
        public delegate void asyncChangeText2(string text1, string text2);
        public delegate bool asyncChangeText3();
        public delegate void MsgBoxCall();
        public bool ChangeTextNoArgs(string text, ref MyEventArgs ret)
        {
            //ret = new MyEventArgs();//若ref换成out，若注释该句，控制离开当前方法之前必须对 out 参数“ret”赋值

            ret.Info = "Old value=" + textBox2.Text + "\r\nNew value=" + text;
            asyncChangeText2 mi = new asyncChangeText2(UpdateForm);
            textBox2.BeginInvoke(mi, new Object[] { text, "" });
            return true;
        }
        public void UpdateForm(string param1, string parm2)
        {
            this.textBox2.Text = param1 + parm2;
        }
        public void ChangeText( string text)
        {
            textBox2.Text = text;
        }

        static string getMemory(object o) // 获取引用类型的内存地址方法    
        {
            GCHandle h = GCHandle.Alloc(o, GCHandleType.WeakTrackResurrection);
            IntPtr addr = GCHandle.ToIntPtr(h);
            return "0x" + addr.ToString("X");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //调用委托实例的BeginInvoke方法来异步执行委托实例上绑定的方法
            asyncChangeText changeText = new asyncChangeText(ChangeTextNoArgs);
            //changeText.BeginInvoke(null, null);
            //异步委托的回调
            MyEventArgs ret=new MyEventArgs();
            Console.WriteLine("1 "+getMemory(ret));
            IAsyncResult rst = changeText.BeginInvoke(AssistClass.count.ToString(), ref ret, new AsyncCallback(CallbackMethod), "This is msgBox callBack!");
        }
        // The callback method must have the same signature as the
        // AsyncCallback delegate.
        static void CallbackMethod(IAsyncResult ar)
        {
            // Retrieve the delegate.
            AsyncResult result = (AsyncResult)ar;
            asyncChangeText caller = (asyncChangeText)result.AsyncDelegate;

            // Retrieve the format string that was passed as state
            // information.
            string rst = (string)ar.AsyncState;
            
            // Define a variable to receive the value of the out parameter.
            // If the parameter were ref rather than out then it would have to
            // be a class-level field so it could also be passed to BeginInvoke.
            MyEventArgs outStr = new MyEventArgs() ;
            Console.WriteLine("2 " + getMemory(outStr));
            // Call EndInvoke to retrieve the results.
            bool returnValue = caller.EndInvoke(ref outStr,result);
            MessageBox.Show(rst+"\r\n"+outStr+ "\r\n" + returnValue.ToString());
            Console.WriteLine("3 " + getMemory(outStr));
        }

    }


    public class AssistClass
    {
        public static AutoResetEvent resetEvent = new AutoResetEvent(true);
        public static int count = 0;
        public static bool isPausing = false;
        public bool Add(Action assistCall)
        {
            assistCall();
            if (Run() == 0)
                return false;
            else
                return true;
        }
        public static void Pause()
        {
            resetEvent.Reset();
            isPausing = true;
        }
        public static void Resume()
        {
            resetEvent.Set();
            isPausing = false;
        }
        public int Run()
        {
            Thread.Sleep(1000);
            if (!isPausing)
                return Interlocked.Increment(ref count);
            else
                return 0;
        }
    }
}
