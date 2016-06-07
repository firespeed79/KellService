using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using KellService;

namespace ServiceLib
{
    [ServiceContract]
    public interface ITest
    {
        [DataMember]
        int Id { get; set; }
        [DataMember]
        string Name { get; set; }
        [OperationContract]
        void Say(string msg);
        [OperationContract]
        double Sum(double a, double b);
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerSession)]
    public class Test : ITest
    {
        public int Id
        {
            get;

            set;
        }

        public string Name
        {
            get;

            set;
        }

        public void Say(string msg)
        {
            Console.WriteLine(msg);
        }

        public double Sum(double a, double b)
        {
            return a + b;
        }
    }

    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IDualTest
    {
        [DataMember]
        int Id { get; set; }
        [DataMember]
        string Name { get; set; }
        [OperationContract(IsOneWay = true)]//这里一定要加上IsOneWay=true，否则会造成回调死锁！
        void Say(string msg);
        [OperationContract]
        double Sum(double a, double b);
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.PerSession)]//这里一定要把ConcurrencyMode设置为Reentrant或者Multiple，否则会造成回调死锁！因为Multiple是多线程并发模式，不适合在分布式事务中使用，所以采用单线程的Reentrant
    public class DualTest : IDualTest
    {
        static List<ICallback> _callbacks;

        public void AddCallback()
        {
            lock (_callbacks)
            {
                ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
                if (!_callbacks.Contains(callback))
                {
                    _callbacks.Add(callback);
                    Console.WriteLine("增加一个回调：" + callback.GetHashCode());
                }
            }
        }

        public void RemoveCallback()
        {
            lock (_callbacks)
            {
                ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
                if (_callbacks.Contains(callback))
                {
                    _callbacks.Remove(callback);
                    Console.WriteLine("移除一个回调：" + callback.GetHashCode());
                }
            }
        }

        public DualTest()
        {
            _callbacks = new List<ICallback>();
            OperationContext.Current.Channel.Closing += new EventHandler(Channel_Closing);  
            AddCallback();
        }

        void Channel_Closing(object sender, EventArgs e)
        {
            lock (_callbacks)
            {
                ICallback callback = sender as ICallback;
                if (_callbacks.Contains(callback))
                {
                    _callbacks.Remove(callback);
                    Console.WriteLine("客户端关闭连接，自动移除一个回调：" + callback.GetHashCode());
                }
            }
        }

        ~DualTest()
        {
            _callbacks.Clear();
        }

        public int Id
        {
            get;

            set;
        }

        public string Name
        {
            get;

            set;
        }

        public void Say(string msg)
        {
            Console.WriteLine(msg);
            foreach (ICallback callBack in _callbacks)
                callBack.Invoke("服务器已经收到招呼[" + msg + "]！");
        }

        public double Sum(double a, double b)
        {
            return a + b;
        }
    }
}
