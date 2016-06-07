using System;
using System.ServiceModel;
using KellService;
using System.Collections.Generic;
namespace ServiceLib.BLL
{
    [ServiceContract]
    [ServiceKnownType(typeof(ResultObject))]
    [ServiceKnownType(typeof(List<object>))]
    [ServiceKnownType(typeof(List<bool>))]
    [ServiceKnownType(typeof(List<byte>))]
    [ServiceKnownType(typeof(List<sbyte>))]
    [ServiceKnownType(typeof(List<float>))]
    [ServiceKnownType(typeof(List<double>))]
    [ServiceKnownType(typeof(List<decimal>))]
    [ServiceKnownType(typeof(List<short>))]
    [ServiceKnownType(typeof(List<ushort>))]
    [ServiceKnownType(typeof(List<int>))]
    [ServiceKnownType(typeof(List<uint>))]
    [ServiceKnownType(typeof(List<long>))]
    [ServiceKnownType(typeof(List<ulong>))]
    [ServiceKnownType(typeof(List<char>))]
    [ServiceKnownType(typeof(List<string>))]
    public interface ICustomBusiness
    {
        [OperationContract]
        bool CompileAndInvoke(string source, string typeFullName, string memberName, object[] args, System.Reflection.BindingFlags binding, out ResultObject result);
    }
}
