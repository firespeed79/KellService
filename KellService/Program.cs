using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Runtime.Serialization;
using System.Configuration;

namespace KellService
{
    [ServiceContract]
    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void Invoke(params object[] args);
    }

    public static class ClientProxy<T>
    {
        /// <summary>
        /// 注册客户端代理
        /// </summary>
        /// <param name="address">服务所在地址</param>
        /// <param name="isDualChannel">是否为双工通讯</param>
        /// <param name="instance">客户端的回调上下文不能共用，每次使用都要new出一个新的InstanceContext</param>
        /// <returns></returns>
        public static T CreateProxy(string address, bool isDualChannel = false, InstanceContext instance = null)
        {
            T proxy = default(T);
            if (isDualChannel)
            {
                if (instance == null)
                    throw new Exception("双工通讯模式下InstanceContext不能为空！");
                DuplexChannelFactory<T> factory = new DuplexChannelFactory<T>(instance, new WSDualHttpBinding(), new EndpointAddress(address));
                proxy = factory.CreateChannel();
                IContextChannel chn = proxy as IContextChannel;
                if (chn != null)
                {
                    int sec = 5;
                    string OperationTimeout = ConfigurationManager.AppSettings["OperationTimeout"];
                    if (!string.IsNullOrEmpty(OperationTimeout))
                    {
                        int R;
                        if (int.TryParse(OperationTimeout, out R))
                            sec = R;
                    }
                    chn.OperationTimeout = TimeSpan.FromSeconds(sec);
                }
            }
            else
            {
                proxy = ChannelFactory<T>.CreateChannel(new BasicHttpBinding(), new EndpointAddress(address));
            }
            return proxy;
        }

        public static IContextChannel GetProxyChannel(T proxy)
        {
            IContextChannel chn = proxy as IContextChannel;
            return chn;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string serviceName = ConfigurationManager.AppSettings["serviceName"];
            string inter = ConfigurationManager.AppSettings["interface"];
            string clas = ConfigurationManager.AppSettings["class"];
            string ass = ConfigurationManager.AppSettings["assembly"];
            string dua = ConfigurationManager.AppSettings["dual"];
            string[] services = serviceName.Split('|');
            if (services.Length > 0)
            {
                List<SelfHost.MyHost> hosts = new List<SelfHost.MyHost>();
                for (int i = 0; i < services.Length; i++)
                {
                    try
                    {
                        string service = services[i];
                        string interf = inter.Split('|')[i];
                        string clasa = clas.Split('|')[i];
                        string asse = ass.Split('|')[i];
                        string d = dua.Split('|')[i];
                        bool dual = false;
                        if (d == "1")
                            dual = true;
                        SelfHost.MyHost host = new SelfHost.MyHost(service, interf, clasa, asse, dual);
                        if (host != null)
                        {
                            host.Opened += Host_Opened;
                            host.Closed += Host_Closed;
                            host.Faulted += Host_Faulted;
                            host.UnknownMessageReceived += Host_UnknownMessageReceived;
                            host.Start();
                            hosts.Add(host);
                        }
                        else
                        {
                            Console.WriteLine("服务创建失败，请检查配置信息和程序集是否对应.");
                            Console.ReadLine();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("创建服务时出现异常，可能是配置信息有误：" + e.Message);
                    }
                }
                Console.Write("想要停止服务，请按任意键...");
                Console.ReadLine();
                foreach (SelfHost.MyHost host in hosts)
                {
                    host.Stop();
                }
            }
        }

        private static void Host_UnknownMessageReceived(object sender, SelfHost.ServiceArgs e)
        {
            //Console.WriteLine("服务收到未知消息：" + e);
        }

        private static void Host_Faulted(object sender, SelfHost.ServiceArgs e)
        {
            //Console.WriteLine("服务运行出错：" + e);
        }

        private static void Host_Closed(object sender, SelfHost.ServiceArgs e)
        {
            //Console.WriteLine("已经停止服务：" + e);
        }

        private static void Host_Opened(object sender, SelfHost.ServiceArgs e)
        {
            //Console.WriteLine("已经启动服务：" + e);
        }
    }
}
