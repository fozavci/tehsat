using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace Tehsat
{
    public class ServiceSocket
    {
        public string Type { get; set; }
        public int Port { get; set; }
        public bool Status { get; set; }
        public string[] ServerAddress { get; set; }
        public string[] ServerDomain { get; set; }
        public Channel ChannelObject { get; set; }                  
        public Thread ServiceThread { get; set; }

        // Dictionary for Implants 
        public ConcurrentDictionary<string, Implant> Implants = new ConcurrentDictionary<string, Implant>();


        // Dictionary for Implant Pipes (TCP&UDP)
        public ConcurrentDictionary<string, object> ImplantPipes = new ConcurrentDictionary<string, object>();

        public ServiceSocket(Channel ChannelObject)
        {
            this.ChannelObject = ChannelObject;
            Status = false;
        }
        public virtual void Start()
        {
            
        }
        public virtual bool Stop()
        {
            return false;
        }

        public virtual bool Send(object implant, byte[] data)
        {
            return false;
        }
        public virtual void Receive(string ClientEndPoint, Object obj)
        {

        }

        #pragma warning disable
        public virtual async Task ReceiveAsync(string ClientEndPoint, Object obj)
        {

        }
        #pragma warning restore
        
        public virtual Implant GenerateImplant(string proto, object obj)
        {
            Implant implant = new Implant();

            implant.Id = "Not Registered";
            implant.Status = "Connected";
            implant.Protocol = proto;
            implant.FirstSeen = DateTime.Now.ToString();
            implant.LastSeen = DateTime.Now.ToString();

            switch (proto) { 
                case "UDP":
                    IPEndPoint ClientEndPoint = IPEndPoint.Parse((string)obj);
                    implant.Endpoint = ClientEndPoint.ToString();
                    break;
                case "TCP":
                    TcpClient tcpsocket = (TcpClient)obj;
                    implant.Endpoint = tcpsocket.Client.RemoteEndPoint.ToString();
                    implant.SocketObject = tcpsocket;
                    implant.Pipe = tcpsocket.GetStream();
                    break;
                case "HTTP Websocket":
                    WebSocket wsocket = (WebSocket)obj;
                    implant.SocketObject = wsocket;
                    break;
                default:
                    break;
            }
            

            return implant;
        }

    }
}

