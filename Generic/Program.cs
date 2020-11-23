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
                // do work
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
        static void Main(string[] args)
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
}
