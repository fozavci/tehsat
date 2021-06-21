using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tehsat
{
    public class ServiceSocketTCP : ServiceSocket
    {
        public TcpListener TCPService { get; set; }

        public ServiceSocketTCP(Channel ChannelObject) : base(ChannelObject)
        {
            // Setting up the service type
            Type = "TCP";
        }

        public override void Start()
        {
            ServiceThread = new Thread(() =>
            {
                try
                {
                    // Setting up the Listener
                    TCPService = new TcpListener(IPAddress.Any, ChannelObject.Port);

                    // Service starts and wait for the implants
                    Console.WriteLine("Waiting for implants... ");
                    TCPService.Start();

                    // Setting up the service status
                    Status = true;

                    while (Status)
                    {
                        // Accepting implants to start receiving
                        TcpClient TCPClient = TCPService.AcceptTcpClient();

                        // Add the implant to the Implants list
                        Implant implant = GenerateImplant("TCP",TCPClient);
                        Implants.TryAdd(implant.Endpoint,implant);

                        // Starting receive from the implant
                        Console.WriteLine("Receive task is starting for {0}.", implant.Endpoint);
                        _ = Task.Run(() => Receive(implant.Endpoint,TCPClient));
                    }


                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("TCP Service Exception on Receive: " + e);
                    Status = false;
                }
            });

            ServiceThread.Start();
        }

        public override bool Stop()
        {

            // Stop listening for new clients.
            if (Implants.Count > 0)
            {
                Console.WriteLine("Closing the implant connections.");
                foreach (var i in Implants)
                {
                    TcpClient itcp = (TcpClient)i.Value.SocketObject;
                    itcp.Client.Close();
                    i.Value.LastSeen = DateTime.Now.ToString();
                    i.Value.Status = "Disconnected";
                }

            }
            Console.WriteLine("The TCP service is stopping.");
            ServiceThread.Interrupt();
            TCPService.Stop();
            Status = false;
            Console.WriteLine("The TCP service stopped successfully.");
            return true;
        }

        public override bool Send(object imp, byte[] data)
        {
            if (data.Length == 0) return false;

            TcpClient implant = (TcpClient)imp;

            try
            {
                // Get a stream object for reading and writing
                NetworkStream stream = implant.GetStream();

                if (implant.Connected && stream.CanWrite)
                {
                    Console.WriteLine("Data is sending to {0}", implant.Client.RemoteEndPoint);

                    //Sending data                
                    stream.Write(data);
                    return true;
                }
                else
                {
                    Console.WriteLine("{0} TCP Socket is not writable.", implant.Client.RemoteEndPoint);
                    return false;
                }

            }
            catch
            {
                Console.Error.WriteLine("Data couldn't send to {0}.", implant.Client.RemoteEndPoint);
                return true;
            }
        }

        public override void Receive(string ClientEndPoint, Object obj)
        {
            // Getting the TCP client
            TcpClient TCPClient = (TcpClient)obj;

            // If the the client isn't connected, don't proceed
            if (!TCPClient.Connected) { return; };

            // Get a stream object for reading and writing
            NetworkStream stream = TCPClient.GetStream();

            // Buffer for the data to read
            byte[] rdata = new byte[TCPClient.ReceiveBufferSize];

            if (!ImplantPipes.ContainsKey(ClientEndPoint))
            {
                // Save the pipe to the Impant Pipes for later use
                ImplantPipes.TryAdd(ClientEndPoint, (Object)stream);
            }


            try
            {
                // Read stream as string and process
                while (TCPClient.Connected)
                {
                    // Recieve from the socket
                    stream.Read(rdata);

                    // Update the last seen whenever receive data
                    Implants[ClientEndPoint].LastSeen = DateTime.Now.ToString();


                    // All buffer bytes converting to string, and it takes time
                    string instruction = Encoding.UTF8.GetString(rdata);

                    // Remove the chars after \0 and Trim the instruction for \n
                    instruction = instruction.Replace("\0", string.Empty).TrimEnd();

                    Console.WriteLine($"TCP Endpoint: {ClientEndPoint} Received: {instruction}");

                    // Sending the response content set
                    byte[] sdata;
                    if (instruction == ChannelObject.RequestContent)
                    {
                        sdata = Encoding.UTF8.GetBytes(ChannelObject.ResponseContent);
                    }
                    else
                    {
                        sdata = Encoding.UTF8.GetBytes(ChannelObject.ResponseError);
                    }
                    stream.Write(sdata);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} is disconnected: {1}", ClientEndPoint, e.Message);
                Implant i = Implants[ClientEndPoint];
                TcpClient itcp = (TcpClient)i.SocketObject;
                itcp.Client.Close();
                i.LastSeen = DateTime.Now.ToString();
                i.Status = "Disconnected";
            }

        }

    }
}
