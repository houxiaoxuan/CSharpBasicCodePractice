using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Generic
{
    class Program
    {
        
        #region generic delegate
        /// <summary>
        /// 委托可以定义它自己的类型参数。 引用泛型委托的代码可以指定类型参数以创建封闭式构造类型，就像实例化泛型类或调用泛型方法一样
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public delegate void Del<T>(T item);
        public static void Notify(int i) { }

        static readonly Del<int> m1 = new Del<int>(Notify);
        static Del<int> m2 = Notify;//简化上一行

        /// <summary>
        /// 在泛型类中定义的委托可以用类方法使用的相同方式来使用泛型类类型参数。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        class Stack<T>
        {
            T[] items;
            int index;

            public delegate void StackDelegate(T[] items);
        }
        /// <summary>
        /// 引用委托的代码必须指定包含类的类型参数
        /// </summary>
        /// <param name="items"></param>
        private static void DoWork(float[] items) { }

        public static void TestStack()
        {
            Stack<float> s = new Stack<float>();
            Stack<float>.StackDelegate d = DoWork;
            d(new float[] { 1.2222223f});
        }
        /// <summary>
        /// 根据典型设计模式定义事件时，泛型委托特别有用，因为发件人参数可以为强类型，无需在它和 Object 之间强制转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        delegate void StackEventHandler<T, U>(T sender, U eventArgs);

        class Stacks<T>
        {

            public class StackEventArgs : System.EventArgs { }
            public event StackEventHandler<Stacks<T>, StackEventArgs> stackEvent;

            public virtual void OnStackChanged(StackEventArgs a)
            {
                stackEvent(this, a);
            }
        }

        class SampleClass
        {
            public void HandleStackChange<T>(Stacks<T> stack, Stacks<T>.StackEventArgs args)
            {

            }
                
        }

        public static void Test()
        {
            Stacks<double> s = new Stacks<double>();
            SampleClass o = new SampleClass();
            s.stackEvent += o.HandleStackChange;
            //触发事件
            s.OnStackChanged(new Stacks<double>.StackEventArgs());
        }
        #endregion
        static void Main1(string[] args)
        {

            decimal x = 0.999m;
            decimal y = 9999999999999999999999999999m;
            Console.WriteLine("My amount = {0:C}", x);//货币格式是使用标准货币格式字符串“C”或“c”指定的，小数位超出0.99所以会为1
            Console.WriteLine("Your amount = {0:C}", y);
            Console.WriteLine(1.222222344342m);
            Console.WriteLine(1.222222344342m*10000);
            Console.WriteLine(1.222222344342M);
            Console.WriteLine(1.222222344342M * 10000);
            Console.WriteLine(1.222222344342f);
            Console.WriteLine(1.222222344342f * 10000);
            Console.WriteLine(1.222222344342F);
            Console.WriteLine(1.222222344342F * 10000);
            Console.WriteLine(1.222222344342D);
            Console.WriteLine(1.222222344342D * 10000);
            Console.WriteLine(1.222222344342d);
            Console.WriteLine(1.222222344342d * 10000);
            Test();
            m1(1);
            m2(1);
        }
        /// <summary>
        /// 开放式构造和封闭式构造类型可用作方法参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        void Swap<T>(List<T> list1, List<T> list2)
        {
            //code to swap items
        }

        void Swap(List<int> list1, List<int> list2)
        {
            //code to swap items
        }
        //如果一个泛型类实现一个接口，则该类的所有实例均可强制转换为该接口。
        //泛型类是不变量。 换而言之，如果一个输入参数指定 List<BaseClass>，且你尝试提供 List<DerivedClass>，则会出现编译时错误。
    }
    #region  generic class
    /// <summary>
    /// 对于泛型类 Node<T>,客户端代码可通过指定类型参数来引用类，创建封闭式构造类型 (Node<int>)。
    /// 或者，可以不指定类型参数（例如指定泛型基类时），创建开放式构造类型 (Node<T>)。 泛型类可继承自具体的封闭式构造或开放式构造基类
    /// </summary>
    class BaseNode { }
    class BaseNodeGeneric<T> { }

    // concrete type
    class NodeConcrete<T> : BaseNode { }

    //closed constructed type
    class NodeClosed<T> : BaseNodeGeneric<int> { }

    //open constructed type
    class NodeOpen<T> : BaseNodeGeneric<T> { }

    /// <summary>
    /// 非泛型类（即，具体类）可继承自封闭式构造基类，但不可继承自开放式构造类或类型参数，因为运行时客户端代码无法提供实例化基类所需的类型参数。
    /// </summary>

    //No error
    class Node1 : BaseNodeGeneric<int> { }

    //Generates an error
    //class Node2 : BaseNodeGeneric<T> {}

    //Generates an error
    //class Node3 : T {}

    /// <summary>
    /// 继承自开放式构造类型的泛型类必须对非此继承类共享的任何基类类型参数提供类型参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    class BaseNodeMultiple<T, U> { }

    //No error
    class Node4<T> : BaseNodeMultiple<T, int> { }

    //No error
    class Node5<T, U> : BaseNodeMultiple<T, U> { }

    //Generates an error
    //class Node6<T> : BaseNodeMultiple<T, U> {}

    /// <summary>
    /// 继承自开放式构造类型的泛型类必须指定作为基类型上约束超集或表示这些约束的约束
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class NodeItem<T> where T : System.IComparable<T>, new() { }
    class SpecialNodeItem<T> : NodeItem<T> where T : System.IComparable<T>, new() { }

    /// <summary>
    /// 泛型类型可使用多个类型参数和约束
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="U"></typeparam>
    class SuperKeyType<K, V, U>
    where U : System.IComparable<U>
    where V : new()
    { }

    class GenericList<T>
    {
        // CS0693
#pragma warning disable CS0693 // 类型参数与外部类型中的类型参数同名
        void SampleMethod<T>() { }
#pragma warning restore CS0693 // 类型参数与外部类型中的类型参数同名
    }

    class GenericList2<T>
    {
        //No warning
        void SampleMethod<U>() { }
        #region generic method
        /// <summary>
        /// 使用约束在方法中的类型参数上实现更多专用操作
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        void SwapIfGreater<U>(ref U lhs, ref U rhs) where U : System.IComparable<U>
        {
            U temp;
            if (lhs.CompareTo(rhs) > 0)
            {
                temp = lhs;
                lhs = rhs;
                rhs = temp;
            }
        }
        /// <summary>
        /// 泛型方法可重载在数个泛型参数上
        /// </summary>
        void DoWork() { }
        void DoWork<U>() { }
        void DoWork<K, U>() { }
        #endregion
    }
    #endregion

    #region generic interface
    /// <summary>
    /// 可将多个接口指定为单个类型上的约束
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class Stack<T> where T : System.IComparable<T>, IEnumerable<T>
    {
    }
    /// <summary>
    /// 一个接口可定义多个类型参数
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    interface IDictionary<K, V>
    {
    }
    /// <summary>
    /// 适用于类的继承规则也适用于接口
    /// </summary>
    interface IMonth<T> { }
    
    interface IJanuary : IMonth<int> { }  //No error
    interface IFebruary<T> : IMonth<int> { }  //No error
    interface IMarch<T> : IMonth<T> { }    //No error
    //interface IApril<T>  : IMonth<T, U> {}  //Error

    /// <summary>
    /// 具体类可实现封闭式构造接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IBaseInterface<T> { }

    class SampleClass : IBaseInterface<string> { }
    /// <summary>
    /// 只要类形参列表提供接口所需的所有实参，泛型类即可实现泛型接口或封闭式构造接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IBaseInterface1<T> { }
    interface IBaseInterface2<T, U> { }

    class SampleClass1<T> : IBaseInterface1<T> { }          //No error
    class SampleClass2<T> : IBaseInterface2<T, string> { }  //No error
    #endregion

    #region  covariant、contravariant
    interface ICotraviant<in T>
    {
        void Sub2Base(T t);
    }
    interface IContravariant<out T>
    {
        T Base2Sub();
    }

    interface Base
    {
        public int SubType { get; set; }
    }
    class FacadeBase<T,U>: IContravariant<U>
    {
        private TheEnum TheEnum;
        BaseToSub<TheEnum, Base, T> Converter ;
        public IList<T> Ts=new List<T>();
        public FacadeBase()
        {
            TheEnum = TheEnum._1;
            Converter = new BaseToSub<TheEnum, Base, T>(Convert);
        }
        public T Convert(TheEnum theEnum,Base @base)
        {
            return default(T);
        }
        

        public U Base2Sub()
        {
            throw new NotImplementedException();
        }
    }
    public enum TheEnum { _1, _2, _3 };
    public delegate Tsub BaseToSub<T_enum, in Base, out Tsub>(T_enum enumbale, Base the_base) where T_enum : Enum ;
    //public delegate void DContravariant<in A>(A a);

    interface IContravariant_1<in A>
    {
        void SetSomething(A sampleArg);
        void DoSomething<T>() where T : A;
        // The following statement generates a compiler error.
        // A GetSomething();
    }
    class Sub0 : Base
    {
        public string Sub0Property = "Sub0";
        private int Type = 0;
        public Sub0()
        {
            
        }

        public int SubType
        {
            get
            {
                return Type;
            }

            set
            {
                Type=value;
            }
        }
    }
    //class Sub1 : Base
    //{
    //    public string Sub1Property = "Sub1";
    //}
    class Debug
    {
        public delegate R DVariant<in A, out R>(A a);
        IList<Base> bases=new List<Base>();
        static void Main1(string[] args)
        {
            Sub0 sub0 = new Sub0() { SubType = 1 };
            FacadeBase<Sub0,Base> facadeBase = new FacadeBase<Sub0,Base>();;
        }
    }
    public class Type1 { }
    public class Type2 : Type1 { }
    public class Type3 : Type2 { }

    public class Program1
    {
        public static Type2 MyMethod(Type1 t)
        {
            return t as Type3 ?? new Type3();
        }

        static void Main2()
        {
            Func<Type2, Type1> f1 = MyMethod;
            Type1 t0 = f1(new Type3());
            // Covariant return type and contravariant parameter type.
            Func<Type3, Type1> f2 = f1;
            Type1 t1 = f2(new Type3());
        }
    }
    class Base2
    {
        public string Obj = "Base";
        public static void PrintBases(IEnumerable<Base2> bases)
        {
            foreach (Base2 b in bases)
            {
                Console.WriteLine(b);
            }
        }
    }

    class Derived : Base2
    {
        public Derived()
        {
            Obj = "Derived";
        }

        public static void Main3()
        {
            List<Derived> dlist = new List<Derived>();

            //Derived.PrintBases(dlist);
            IEnumerable<Base2> bIEnum = dlist;
            SortedSet<int> vs = new SortedSet<int>() { 1,3,2};
                    // The following line generates a compiler error, because classes are invariant.
                    // List<Object> list = new List<String>();
            IEnumerable<Base2> base2s = new List<Derived>();//IEnumerable是协变接口
            //IEnumerable<Derived> deriveds = base2s;//无法将类型“System.Collections.Generic.IEnumerable<Generic.Base2>”隐式转换为“System.Collections.Generic.IEnumerable<Generic.Derived>”。
            //存在一个显式转换(是否缺少强制转换?)
            //如要实现上述转换，需要自定义的逆变接口（即变体），并将Base2转换为Derived（new一个Derived并根据Base2属性将其赋值，自己实现显式转换），个人觉得实际上小工程中不存在这样的使用场景（需要时直接返回派生对象即可）
            Action<Base2> action = new Action<Base2>((o) => Console.WriteLine(o.Obj.ToString()));
            action(new Base2());
            action(new Derived());
            Action<Derived> action1 = action;
            //action1(new Base2());//无法从“Generic.Base2”转换为“Generic.Derived”
            action1(new Derived());
        }
    }
    #endregion


    class SampleCollection<T>
    {
        // Declare an array to store the data elements.
        private T[] arr = new T[100];
        int nextIndex = 11111;

        // Define the indexer to allow client code to use [] notation.
        public T this[int i] => arr[i];

        public void Add(T value)
        {
            if (nextIndex >= arr.Length)
                throw new IndexOutOfRangeException($"The collection can hold only {arr.Length} elements.");
            arr[nextIndex++] = value;
        }
    }

    class Program2
    {
        static void Main3()
        {
            var stringCollection = new SampleCollection<string>();
            stringCollection.Add("Hello, World");
            System.Console.WriteLine(stringCollection[0]);
        }
    }
    // The example displays the following output:
    //       Hello, World.
    public class Employee
    {
        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set => _firstName = value;
        }
        private string _lastName;
        public virtual string LastName
        {
            get => _lastName;
            set => _lastName = value;
        }
        public void IMethod() { Console.WriteLine("Method of Employee"); }
        public virtual void IMethodForNew() { Console.WriteLine("New Method of Employee"); }
        public virtual void IMethodForOverride() { Console.WriteLine("Override Method of Employee"); }//被abstract修饰亦然（注意类也要添加abstract修饰符），接口同理
        
    }

    public class Manager : Employee
    {
        private string _firstName;

        // Notice the use of the new modifier:
        public new string FirstName//new出来的属性在Manager对象中会隐藏基类的属性（即实际上基类对象的该属性仍存在，可通过将其隐式转换为基类对象或赋值给基类对象并对其进行修改，达到基类中被隐藏属性的修改），即new不会修改基类对象被隐藏属性的属性的引用地址
        {
            get => _firstName;
            set =>_firstName = value + ", Manager";
           
        }
        private string _lastName;
        public override string LastName//重写后的该属性即使转换或赋值给基类对象，即override会修改基类对象该属性的引用地址
        {
            get => _lastName;
            set => _lastName = value;
        }
        public new void IMethod() { Console.WriteLine("Method of Manager"); }
        public new void IMethodForNew() { Console.WriteLine("New Method of Manager"); }
        //注意此处new会隐藏基类中的方法(无论原基类中被new的方法或属性，有无virtual关键字修饰，有的话只是可以被重写而已)，在构建了子类对象后，无论是将其转换为基类对象、还是赋值给基类对象，调用该基类对象并执行的都是基类中的方法（方法的引用地址未被修改）
        public override void IMethodForOverride() { Console.WriteLine("Override Method of Manager"); }
        //此处override，在构建子类对象后，若将其转换为基类对象、或是赋值给基类对象，调用该基类对象并执行的都是子类中的方法（方法的引用地址已被修改）
    public void SetBaseName(string s)
    {
        base.FirstName = s;
        Console.WriteLine("Base's FirstName property is set.");
    }
    public string GetBaseName()
    {
        Console.WriteLine($"Base's FirstName property is {base.FirstName} .");
        return base.FirstName;
    }
    }

    class TestHiding
    {
        
        static void Main1()
        {
            
            Manager m1 = new Manager();
            // Derived class property.
            m1.FirstName = "John";
            m1.LastName = "Lily";
            m1.IMethod();
            m1.IMethodForNew();
            m1.IMethodForOverride();
            //仍可通过子类去设置父类被隐藏的属性
            m1.SetBaseName(m1.LastName+ " set by Derived class");
            m1.GetBaseName();
            // Base class property.
            ((Employee)m1).FirstName = "Mary";
            ((Employee)m1).IMethod();
            ((Employee)m1).IMethodForNew();
            ((Employee)m1).IMethodForOverride();
            Console.WriteLine(((Employee)m1).FirstName);
            Employee m2 = m1;
            m2.FirstName = "hhh";
            m2.IMethod();
            m2.IMethodForNew();
            m2.IMethodForOverride();
            Console.WriteLine(m2.FirstName);
            Employee m3 = new Employee();
            m3.IMethod();
            m3.IMethodForNew();
            m3.IMethodForOverride();
            System.Console.WriteLine("Name in the derived class is: {0}", m1.FirstName);
            System.Console.WriteLine("Name in the base class is: {0}-{1}", ((Employee)m1).FirstName,m2.FirstName);
        }
    }
    /* Output:
        Name in the derived class is: John, Manager
        Name in the base class is: Mary
    */
    
}
