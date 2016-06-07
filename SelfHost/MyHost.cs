using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;

namespace SelfHost
{
    public class ServiceArgs : EventArgs
    {
        string uri;
        Binding binding;

        public string Uri
        {
            get
            {
                return uri;
            }
        }

        public Binding Binding
        {
            get
            {
                return binding;
            }
        }

        public ServiceArgs(string uri, Binding binding)
        {
            this.uri = uri;
            this.binding = binding;
        }

        public override string ToString()
        {
            return "["+ binding.Name + "]"+ uri;
        }
    }

    public class MyHost : IDisposable
    {
        ServiceHost host;
        string interfaceFullName;
        string classFullName;
        string assemblyFile;
        bool dualChannel;
        public event EventHandler<ServiceArgs> Opened;
        public event EventHandler<ServiceArgs> Closed;
        public event EventHandler<ServiceArgs> Faulted;
        public event EventHandler<ServiceArgs> UnknownMessageReceived;

        public string InterfaceFullName
        {
            get
            {
                return interfaceFullName;
            }
        }

        public string ClassFullName
        {
            get
            {
                return classFullName;
            }
        }

        public string AssemblyFile
        {
            get
            {
                return assemblyFile;
            }
        }

        public bool DualChannelMode
        {
            get
            {
                return dualChannel;
            }
        }

        public MyHost(string serviceName, string interfaceFullName, string classFullName, string assemblyFile = null, bool dual = false)
        {
            if (serviceName == null) serviceName = "Service";
            this.interfaceFullName = interfaceFullName;
            this.classFullName = classFullName;
            this.assemblyFile = assemblyFile;
            this.dualChannel = dual;
            try
            {
                string hostName = GetHostName();
                int port = GetListenPort();
                string uri = "http://" + hostName + ":" + port + "/" + serviceName;
                Console.WriteLine("配置的服务地址：" + uri);
                Type typeClass = GetType(assemblyFile, classFullName);
                Type typeInterface = GetType(assemblyFile, interfaceFullName);
                if (typeClass != null)
                {
                    host = new ServiceHost(typeClass, new Uri(uri));
                    ServiceMetadataBehavior behavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                    if (behavior == null)
                    {
                        behavior = new ServiceMetadataBehavior();
                        behavior.HttpGetEnabled = true;
                        host.Description.Behaviors.Add(behavior);
                    }
                    else
                    {
                        behavior.HttpGetEnabled = true;
                    }
                    host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "MEX");
                    ServiceDebugBehavior debug = host.Description.Behaviors.Find<ServiceDebugBehavior>();
                    if (debug == null)
                    {
                        debug = new ServiceDebugBehavior();
                        debug.HttpHelpPageEnabled = true;
                        debug.IncludeExceptionDetailInFaults = true;
                        host.Description.Behaviors.Add(debug);
                    }
                    else
                    {
                        debug.HttpHelpPageEnabled = true;
                        debug.IncludeExceptionDetailInFaults = true;
                    }
                    host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "DEBUG");
                    if (typeInterface != null)
                    {
                        Binding binding = null;
                        if (dual)
                        {
                            binding = new WSDualHttpBinding();
                            ((WSDualHttpBinding)binding).MaxReceivedMessageSize = int.MaxValue;
                        }
                        else
                        {
                            binding = new BasicHttpBinding();
                            ((BasicHttpBinding)binding).MaxReceivedMessageSize = int.MaxValue;
                        }

                        host.Opened += delegate
                        {
                            if (Opened != null)
                                Opened(this, new ServiceArgs(uri, binding));
                            Console.WriteLine("已经启动服务：" + uri);
                        };
                        host.Closed += delegate
                        {
                            if (Closed != null)
                                Closed(this, new ServiceArgs(uri, binding));
                            Console.WriteLine("已经停止服务：" + uri);
                        };
                        host.Faulted += delegate
                        {
                            if (Faulted != null)
                                Faulted(this, new ServiceArgs(uri, binding));
                            Console.WriteLine("服务运行出错：" + uri);
                        };
                        host.UnknownMessageReceived += delegate
                        {
                            if (UnknownMessageReceived != null)
                                UnknownMessageReceived(this, new ServiceArgs(uri, binding));
                            Console.WriteLine("服务收到未知消息：" + uri);
                        };

                        host.AddServiceEndpoint(typeInterface, binding, string.Empty);
                        Console.WriteLine("服务" + uri + "初始化成功.");
                    }
                    else
                    {
                        Console.WriteLine("服务" + uri + "初始化失败.");
                        Console.WriteLine("找不到协议参照的接口[" + interfaceFullName + "]！");
                    }
                }
                else
                {
                    Console.WriteLine("服务" + uri + "初始化失败.");
                    Console.WriteLine("找不到类型[" + classFullName + "]！");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("初始化服务时出错：" + ex.Message);
                throw ex;
            }
        }

        public bool Start()
        {
            if (host != null)
            {
                try
                {
                    if (host.State != CommunicationState.Opened)
                    {
                        host.Open();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("启动服务时出错：" + ex.Message);
                    throw ex;
                }
            }
            return false;
        }

        public bool Stop()
        {
            if (host != null)
            {
                try
                {
                    if (host.State != CommunicationState.Closed)
                    {
                        host.Close();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("停止服务时出错：" + ex.Message);
                    throw ex;
                }
            }
            return false;
        }

        private static int GetListenPort()
        {
            int port = 8888;
            string p = ConfigurationManager.AppSettings["port"];
            if (!string.IsNullOrEmpty(p))
            {
                int RET;
                if (int.TryParse(p, out RET))
                    port = RET;
            }
            return port;
        }

        private static string GetIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry host = Dns.GetHostEntry(hostName);
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }

        private static string GetHostName()
        {
            string hostName = Dns.GetHostName();
            return hostName;
        }

        private static Type GetType(string assemblyFile, string typeFullName)
        {
            Assembly ass = null;
            if (!string.IsNullOrEmpty(assemblyFile))
            {
                ass = Assembly.LoadFrom(assemblyFile);
            }
            else
            {
                ass = Assembly.GetEntryAssembly();
            }
            if (ass != null)
            {
                try
                {
                    Type type = ass.GetType(typeFullName, false, true);
                    if (type == null)//找不到时，可以猜测当前目录下的动态库
                    {
                        string[] name = typeFullName.Split('.');
                        if (name.Length > 0)
                        {
                            ass = Assembly.LoadFrom(name[0] + ".dll");
                            type = ass.GetType(typeFullName, false, true);
                        }
                    }
                    return type;
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetType时，在程序集["+ assemblyFile + "]中获取不到类型["+ typeFullName + "]：" + e.Message);
                    return null;
                }
            }
            else
            {
                Console.WriteLine("GetType时，获取不到任何程序集[" + assemblyFile + "]");
            }
            return null;
        }

        public void Dispose()
        {
            this.Stop();
            if (host != null)
            {
                IDisposable dis = host as IDisposable;
                dis.Dispose();
            }
        }
    }
}
