using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Pipelines;


namespace Tehsat
{
    public class ServiceSocketUDP : ServiceSocket
    {
        public UdpClient UDPService { get; set; }

        public ServiceSocketUDP(Channel ChannelObject) : base(ChannelObject)
        {
            // Setting up the service type
            Type = "UDP";
        }

        public override void Start()
        {
            ServiceThread = new Thread(async () =>
            {
                try {
                    // Creating the endpoint
                    IPEndPoint ep = new IPEndPoint(IPAddress.Any, ChannelObject.Port);
                    Console.WriteLine("Waiting for implants... ");

                    // Setting up a UdpClient for the endpoint created
                    UDPService = new UdpClient(ep);

                    // Setting up the service status
                    Status = true;

                    while (Status)
                    {
                        // Starting Async Receive
                        UdpReceiveResult result = new UdpReceiveResult();
                        try
                        {
                            result = await UDPService.ReceiveAsync();
                        }
                        catch (ObjectDisposedException)
                        {                            
                            break;
                        }

                        // Make the incoming implant endpoint a string for Pipe dictionary
                        string ClientEndPoint = result.RemoteEndPoint.ToString();

                        if (ImplantPipes.ContainsKey(ClientEndPoint))
                        {
                            // If the implant endpoint has a pipe already, get it and use
                            ImplantPipes.TryGetValue(ClientEndPoint, out Object pipe);
                            Pipe p = (Pipe)pipe;
                            await p.Writer.WriteAsync(result.Buffer);
                        }
                        else
                        {
                            // Generate an Implant and add to Implants
                            Implant i = GenerateImplant("UDP",ClientEndPoint);
                            Implants[ClientEndPoint] = i;

                            // If the implant endpoint doesn't have a pipe
                            // create a pipe and run a task for receive/write
                            Pipe p = new Pipe();
                            ImplantPipes.TryAdd(ClientEndPoint, (Object)p);
                            await p.Writer.WriteAsync(result.Buffer);
                            _ = Task.Run(() => ReceiveAsync(ClientEndPoint,p));
                        }
                    }

                }
                catch (SocketException e)
                {
                    Console.Error.WriteLine("UDP Service Exception on Receive: " + e);
                    Status = false;
                }
                finally
                {
                    UDPService.Close();
                    Console.WriteLine("Updating the implants' status.");
                    foreach (var i in Implants)
                    {
                        i.Value.LastSeen = DateTime.Now.ToString();
                        i.Value.Status = "Disconnected";
                    }
                }
            });

            ServiceThread.Start();
        }

        public override bool Stop()
        {

            Console.WriteLine("The UDP service is stopping.");

            ServiceThread.Interrupt();
            Status = false;
            UDPService.Close();

            Console.WriteLine("Updating the implants' status.");
            foreach (var i in Implants)
            {
                i.Value.LastSeen = DateTime.Now.ToString();
                i.Value.Status = "Disconnected";
            }

            Console.WriteLine("The UDP service stopped successfully.");

            return true;

        }

        public override bool Send(object imp, byte[] data)
        {
            if (data.Length == 0) return false;

            IPEndPoint implant = (IPEndPoint)imp;

            try
            {
                if (Status)
                {
                    Console.WriteLine("Data is sending to {0}:{1}", implant.Address, implant.Port);
                    UDPService.Send(data, data.Length, implant);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                Console.Error.WriteLine("Data couldn't send to {0}:{1}.", implant.Address, implant.Port);
                Implants[implant.ToString()].LastSeen = DateTime.Now.ToString();
                Implants[implant.ToString()].Status = "Disconnected";
                return false;
            }            
        }

        public override async Task ReceiveAsync(string ClientEndPoint, Object obj)
        {

            // Getting the pipe 
            Pipe p = (Pipe)obj;

            // Start receiving
            while (true)
            {
                // Read from the buffer and advence to the end
                var readResult = await p.Reader.ReadAsync();
                var instruction = Encoding.ASCII.GetString(readResult.Buffer.FirstSpan.ToArray());
                p.Reader.AdvanceTo(readResult.Buffer.End);

                // Update the last seen regardless of null connection or data received
                Implants[ClientEndPoint].LastSeen = DateTime.Now.ToString();
    
                // Continue if a socket connected, but no data sent
                if (instruction == "XXXX")
                {
                    continue;
                }

                // Trim the instruction for \n
                instruction = instruction.TrimEnd();
                Console.WriteLine($"UDP Endpoint: {ClientEndPoint} Received: {instruction}");

                //Sending received message for debugging
                byte[] sdata;
                if (instruction == ChannelObject.RequestContent)
                {
                    sdata = Encoding.UTF8.GetBytes(ChannelObject.ResponseContent);
                }
                else
                {
                    sdata = Encoding.UTF8.GetBytes(ChannelObject.ResponseError);
                }

                IPEndPoint cep = IPEndPoint.Parse(ClientEndPoint);
                UDPService.Send(sdata, sdata.Length, cep);
            }
        }
    }
}
