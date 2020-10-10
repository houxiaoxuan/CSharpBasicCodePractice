using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public static BackgroundWorker Worker;
        
        private static Form form2;
        private static AssistClass assistClass = new AssistClass();
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
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Thread.Sleep(2000);
            textBox1.Text = "WorkerCompleted";
        }
        private delegate void EnableButtonCallBack(bool IsEnable);
        private delegate void ChangeTextCallBack();
        public AssistClass.AssistCallBack assistCall = new AssistClass.AssistCallBack(assistClass.Run);
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            
            //EnableButtonCallBack btnCall = new EnableButtonCallBack();
            while (AssistClass.count< 1000)
            {
                
                bool ret = assistClass.Add(assistCall);
                EnableBtn();
                if(!ret)
                    AssistClass.resetEvent.WaitOne();
                textBox1.BeginInvoke(new ChangeTextCallBack(ChangeText));//可以改变控件内容
            }
            //ChangeText();//不会改变控件内容
            //textBox1.Invoke(new EnableButtonCallBack(ChangeText));//可以改变控件内容
            //Thread.Sleep(1000);
        }
        private void EnableBtn()
        {
            textBox1.BeginInvoke(new ChangeTextCallBack(IsEnable));
        }
        private void IsEnable()
        {
            textBox1.Enabled = !AssistClass.isPausing;
        }
        private void ChangeText()
        {
            textBox1.Text = AssistClass.count.ToString();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!AssistClass.isPausing)
                AssistClass.Pause();
            else
                AssistClass.Resume();
        }
    }
    public class AssistClass
    {
        public static AutoResetEvent resetEvent = new AutoResetEvent(true);
        public static int count = 0;
        public static bool isPausing = false;
        public delegate int AssistCallBack();
        public bool Add(AssistCallBack assistCall)
        {
            if (Run() == 0)
                return false;
            else
                return true;
        }
        //public int Count { get { return count; }set { count = value; } }
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
            Thread.Sleep(2000);
            if (!isPausing)
                return Interlocked.Increment(ref count);
            else
                return 0;
        }
    }
}
