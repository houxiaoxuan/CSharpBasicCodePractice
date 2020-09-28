//#define DEBUG
#undef DEBUG

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

[assembly: Description(" This Assembly demonstrates custom attributes creation and their run - time query. " )] 

namespace Attribute_Demo
{

    #region CustomAttributes
    // 一个自定义特性 BugFix 被赋给类及其成员
    [AttributeUsage(AttributeTargets.Class |
    AttributeTargets.Constructor |
    AttributeTargets.Field |
    AttributeTargets.Method |
    AttributeTargets.Property,
    AllowMultiple = true,//为true方可添加两个自定义特性类
        Inherited =true)]//为true则主程序的type.GetCustomAttributes(true)方可发现父类特性描述

    public class DeBugInfo : Attribute
    {
        private int bugNo;
        private string developer;
        private string lastReview;
        public string message;

        public DeBugInfo(int bg, string dev, string d)
        {
            this.bugNo = bg;
            this.developer = dev;
            this.lastReview = d;
        }
        
        public int BugNo
        {
            get
            {
                return bugNo;
            }
        }
        public string Developer
        {
            get
            {
                return developer;
            }
        }
        public string LastReview
        {
            get
            {
                return lastReview;
            }
        }
        public string Message
        {
            get
            {
                return message;
            }
            set//若不能set，下文给特性设置Message属性时编译器报错“Message”不是有效的命名特性参数。命名特性参数必须是非只读、非静态或非常数的字段，或者是公共的和非静态的读写属性。
            {
                message = value;
            }
        }
    }
    #endregion

    #region custom class has CustomAttributes
    [DeBugInfo(45, "Zara Ali", "12/8/2012", Message = "Return type mismatch")]
    [DeBugInfo(49, "Nuha Ali", "10/10/2012", Message = "Unused variable")]
    public class Rectangle
    {
        // 成员变量
        protected double length;
        protected double width;
        public Rectangle(double l, double w)
        {
            length = l;
            width = w;
        }
        [DeBugInfo(55, "Zara Ali", "19/10/2012",
        Message = "Return type mismatch")]
        public double GetArea()
        {
            return length * width;
        }
        [DeBugInfo(56, "Zara Ali", "19/10/2012")]
        [Conditional("DEBUG")]
        public void Display()
        {
            Console.WriteLine("Length: {0}", length);
            Console.WriteLine("Width: {0}", width);
            Console.WriteLine("Area: {0}", GetArea());

        }
    }
    [DeBugInfo(49, "Nuha Ali", "10/10/2012", Message = "Sub Attribute")]
    public class SubRectangle:Rectangle
    {
        public SubRectangle(double l, double w):base(l,w)
        {
        }
    }
    #endregion

    #region Serializer 序列化特性运用
    [Serializable]  
    public class SerialClass
    {
        public int Param1 { get; set; }
        public string Param2 { get; set; }
        public bool Param3 { get; set; }
        public SerialClass()
        {

        }
        public SerialClass(int param1, string param2, bool param3)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
        }
    }
    [Serializable]
    public class MyClass 
    {
        public SerialClass serialClass1 { get; set; }
        public SerialClass serialClass2 { get; set; }
        public string Label { 
            get;
            //set;   //可选set
        } = "New Instance";
        public ObservableCollection<SerialClass> Items { get; set; }//System.Runtime.Serialization.SerializationException:“Soap 序列化程序不支持序列化一般类型: System.Collections.Generic.List`1[Attribute_Demo.SerialClass]。”
        = new ObservableCollection<SerialClass>(); //
        //若无此无参构造函数，会引起System.InvalidOperationException异常：
        //Each parameter in constructor 'Void .ctor(Int32, System.String, Boolean)' on type 'Attribute_Demo.MyClass' must bind to an object property or field on deserialization. 
        //Each parameter name must match with a property or field on the object. The match can be case-insensitive.
        public MyClass() { }
        public MyClass(int param1, string param2, bool param3)
        {
            serialClass1=new SerialClass(param1, param2, param3);
            serialClass2=new SerialClass(param1 + 1, param2 + "_2", !param3);
            Items.Add(serialClass1);
            Items.Add(serialClass2);
        }
    }



    #region Example from MSDN for Serializable
    public class Test
    {
        public static void _Main()
        {

            string data_binnary = "data_bin.xml";
            string data_xml = "data_xml.xml";
            // Creates a new TestSimpleObject object.
            TestSimpleObject obj = new TestSimpleObject();
            MyClass ClassToSerial = new MyClass(1, "2", true);
            Console.WriteLine("Before serialization the object contains: ");
            obj.Print();

            // Opens a file and serializes the object into it in binary format.
            Stream stream = File.Open(data_binnary, FileMode.Create);
            Stream stream2 = File.Open(data_xml, FileMode.Create);
            //SoapFormatter|BinaryFormatter都可进行序列化、反序列化操作，但.net在2.0之后已废弃SoapFormatter
            //SoapFormatter formatter = new SoapFormatter();//soap将对象序列化为XML
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            stream.Close();
            //XmlSerializer实现序列化
            XmlSerializer formatter_Xml = new XmlSerializer(typeof(MyClass));
            formatter_Xml.Serialize(stream2, ClassToSerial);
            stream2.Close();

            // Empties obj.
            obj = null;
            ClassToSerial = null;
            // Opens file "data.xml" and deserializes the object from it.
            stream = File.Open(data_binnary, FileMode.Open);
            //formatter = new SoapFormatter();
            formatter = new BinaryFormatter();
            obj = (TestSimpleObject)formatter.Deserialize(stream);
            stream.Close();
            Console.WriteLine("After deserialization the object contains: ");
            obj.Print();

            stream2 = File.Open(data_xml, FileMode.Open);
            ClassToSerial = (MyClass)formatter_Xml.Deserialize(stream2);
            stream2.Close();

            
            
        }
    }

    // A test object that needs to be serialized.
    [Serializable]
    public class TestSimpleObject
    {

        public int member1;
        public string member2;
        public string member3;
        public double member4;

        // A field that is not serialized.
        [NonSerialized] public string member5;

        public TestSimpleObject()
        {

            member1 = 11;
            member2 = "hello";
            member3 = "hello";
            member4 = 3.14159265;
            member5 = "hello world!";
        }

        public void Print()
        {

            Console.WriteLine("member1 = '{0}'", member1);
            Console.WriteLine("member2 = '{0}'", member2);
            Console.WriteLine("member3 = '{0}'", member3);
            Console.WriteLine("member4 = '{0}'", member4);
            Console.WriteLine("member5 = '{0}'", member5);
        }
    }
        #endregion
    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            
            #region get attributes 
            //获取程序集描述
            Process p = Process.GetCurrentProcess();
            String assemblyName = p.ProcessName+".exe";
            Assembly a = Assembly.LoadFrom(assemblyName);
            foreach (Attribute attr in a.GetCustomAttributes(true))
            {
                var v = attr as DescriptionAttribute;
                if(v!=null)
                    Console.WriteLine(v.Description);
            }
            Rectangle rect = new Rectangle(1, 2);
            rect.Display();
            //根据反射获取派生类基类的特性
            Type type = typeof(SubRectangle);
            foreach (object attributes in type.GetCustomAttributes(true))
            {
                DeBugInfo dbi = attributes as DeBugInfo; //打印dbi属性 
                if (dbi != null)
                    Console.WriteLine(dbi.Message);
            }
            #endregion 

            #region 序列化特性示例

            Test._Main();
            #endregion
            #region System.Text.Json序列化json示例
            MyClass ClassToSerial = new MyClass(1,"2",true);
            string jsonString = JsonSerializer.Serialize(ClassToSerial);
            Console.WriteLine(jsonString);//要将序列化的类对象，反序列化
            var SerialToClass = JsonSerializer.Deserialize<MyClass>(jsonString);
            Console.WriteLine(SerialToClass == null ? "NULL" : "Deserial OK");
            
            string jsonString1 = JsonSerializer.Serialize(ClassToSerial.Items?[0]);
            Console.WriteLine(jsonString1);
            var SerialToClass1 = JsonSerializer.Deserialize(jsonString1, typeof(SerialClass));
            Console.WriteLine(SerialToClass1 == null ? "NULL" : "1 Deserial OK");
            string jsonString2 = JsonSerializer.Serialize(ClassToSerial.Items?[1]);
            Console.WriteLine(jsonString2);
            var SerialToClass2 = JsonSerializer.Deserialize(jsonString2, typeof(SerialClass));
            Console.WriteLine(SerialToClass2 == null ? "NULL" : "2 Deserial OK");
            #endregion
        }
    }
}
