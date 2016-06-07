using System;
using System.ServiceModel;
using KellService;
namespace ServiceLib.BLL
{
    [ServiceContract]
    public interface ICommonBusiness
    {
        [OperationContract]
        double Add(params double[] input);
        [OperationContract]
        double Div(double a, double b);
        [OperationContract]
        double Mul(params double[] input);
        [OperationContract]
        double Sub(double a, double b);
    }
}
