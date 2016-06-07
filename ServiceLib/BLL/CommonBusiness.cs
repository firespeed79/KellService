using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using KellService;

namespace ServiceLib.BLL
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerSession)]
    public class CommonBusiness : ICommonBusiness
    {
        public double Div(double a, double b)
        {
            double result = 0;
            if (b == 0)
                result = double.NaN;
            else
                result = a / b;
            return result;
        }

        public double Mul(params double[] input)
        {
            double result = 1;
            foreach (double d in input)
            {
                result *= d;
            }
            return result;
        }

        public double Sub(double a, double b)
        {
            double result = a - b;
            return result;
        }

        public double Add(params double[] input)
        {
            double result = 0;
            foreach (double d in input)
            {
                result += d;
            }
            return result;
        }
    }
}
