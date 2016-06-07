using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using KellService;
using System.Runtime.Serialization;

namespace ServiceLib.BLL
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerSession)]
    public class CustomBusiness : ICustomBusiness
    {
        public bool CompileAndInvoke(string source, string typeFullName, string memberName, object[] args, BindingFlags binding, out ResultObject result)
        {
            object obj;
            bool success = Compile.CompileAndInvoke(source, typeFullName, memberName, args, binding, out obj);
            result = new ResultObject(obj);
            return success;
        }
    }

    [DataContract]
    public class ResultObject// : ISerializable
    {
        object result;

        [DataMember]
        public object Result
        {
            get { return result; }
            set { result = value; }
        }

        public ResultObject(object result)
        {
            this.result = result;
        }

        public override string ToString()
        {
            if (result != null)
                return result.ToString();
            else
                return "[空对象]";
        }

        //protected ResultObject(SerializationInfo info, StreamingContext context)
        //{
        //    this.result = info.GetValue("Result", typeof(object));
        //}

        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("Result", this.result);
        //}
    }

    public static class KnownTypesProvider
    {
        static Type[] GetKnownTypes(ICustomAttributeProvider knownTypeAttributeTarget)
        {
            Type contractType = (Type)knownTypeAttributeTarget;
            return contractType.GetGenericArguments();
        }
    }
}
