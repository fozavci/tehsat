using System;

namespace Tehsat
{
    public class Service
    {
        public string Id { get; set; }

        public Profile Profile { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ChannelType { get; set; }

        public Channel ChannelObject { get; set; }

        public ServiceSocket SocketObject { get; set; }

        public Boolean UpdateSocketObject()
        {
            switch (ChannelType)
            {
                case "HTTP Websocket":
                    SocketObject = new ServiceSocketWebsocket(ChannelObject);
                    return true;
                case "HTTP":
                    SocketObject = new ServiceSocketHTTP(ChannelObject);
                    return true;
                case "TCP":
                    SocketObject = new ServiceSocketTCP(ChannelObject);
                    return true;
                case "UDP":
                    SocketObject = new ServiceSocketUDP(ChannelObject);
                    return true;
                default:
                    return false;
            }
        }
    }
}