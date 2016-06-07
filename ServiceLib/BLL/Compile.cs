using System;
using System.Reflection;
using System.Globalization;
using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text;

namespace ServiceLib.BLL
{
    public static class Compile
    {
        public static bool CompileAndInvoke(string source, string typeFullName, string memberName, object[] args, BindingFlags binding, out object result)
        {
            // 1.CSharpCodePrivoder
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();
            // 2.ICodeComplier
            //ICodeCompiler objICodeCompiler = objCSharpCodePrivoder.CreateCompiler();//使用接口来编译已经过时，直接用类来编译，故屏蔽！
            // 3.CompilerParameters
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = true;

            // 4.CompilerResults
            CompilerResults cr = objCSharpCodePrivoder.CompileAssemblyFromSource(objCompilerParameters, source);

            if (cr.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("编译错误：" + Environment.NewLine + Environment.NewLine);
                foreach (CompilerError err in cr.Errors)
                {
                    sb.Append(err.ErrorText + Environment.NewLine);
                }
                result = sb.ToString();
            }
            else
            {
                //通过反射得到程序集，调用类的成员
                Assembly objAssembly = cr.CompiledAssembly;
                if (objAssembly != null)
                {
                    Type objType = objAssembly.GetType(typeFullName, false, true);
                    if (objType != null)
                    {
                        result = objType.InvokeMember(memberName, binding, null, null, args);
                        return true;
                    }
                    else
                    {
                        result = "反射得到程序集中找不到类[" + typeFullName + "]";
                    }
                }
                else
                {
                    result = "反射得到程序集为空";
                }
            }
            return false;
        }
    }
}
