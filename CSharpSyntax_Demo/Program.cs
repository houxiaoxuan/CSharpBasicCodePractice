#define DEBUG
//#undef DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
    class Program
    {
        static void Main(string[] args)
        {
            Ope oa = new Ope(1,2);
            Ope ob = new Ope(3, 4);
            Ope v = oa + ob;
        }
    }
}
