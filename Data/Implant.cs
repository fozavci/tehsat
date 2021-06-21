using System;

namespace Tehsat
{
    public class Implant
    {
        // Generic implant options
        public string Id { get; set; }
        public string Status { get; set; }
        public string Protocol { get; set; }  // "HTTP", "HTTP Websocket", "TCP", "UDP" 
        public string FirstSeen { get; set; }
        public string LastSeen { get; set; }
        public string Endpoint { get; set; }
        public object Pipe { get; set; }
        public object SocketObject { get; set; }

        // Cloning
        public Implant Clone()
        {
            Implant newimplant = new Implant()
            {
                Id = this.Id,
                Status = this.Status,
                Protocol = this.Protocol,
                FirstSeen = this.FirstSeen,
                LastSeen = this.LastSeen,
                Endpoint = this.Endpoint,
                Pipe = this.Pipe,
                SocketObject = this.SocketObject,
            };

            return newimplant;
        }
        
    }
}
