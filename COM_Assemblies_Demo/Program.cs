using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WordApp = Microsoft.Office.Interop.Word;

namespace COM_Assemblies_Demo
{
    class Program
    {
        //两种添加引用word互操作程序集的方式
        //①点击添加引用——COM——类型库——Microsoft Word XX.0 Object Library（会自动添加另外的依赖COM组件Microsoft Office Core）
        //②点击添加引用——程序集——扩展——Interop.Microsoft.Office.Interop.Word、office
        static void Main(string[] args)
        {
            //打开app
            Application app = new Application();
            //Application app = new ApplicationClass();//需要将嵌入互操作类型改为False
            object path = "D://test.doc";
            if (File.Exists(path.ToString()))
            {
                File.Delete(path.ToString());
            }
            object nothing =System.Reflection.Missing.Value;
            //新建空白页(使用带宏的默认模板则会导致写入内容失败)
            Document doc = app.Documents.Add(ref nothing, ref nothing, ref nothing,ref nothing);
            //写入内容
            doc.Content.InsertAfter("This is debug text!");
            //保存文档
            doc.SaveAs2(path, ref nothing, ref nothing, ref nothing,
                ref nothing, ref nothing, ref nothing, ref nothing,
                ref nothing, ref nothing, ref nothing, ref nothing);
            //关闭文档
            doc.Close();
            //关闭Word
            app.Quit();
        }
    }
}

