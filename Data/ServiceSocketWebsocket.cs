using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;


namespace Tehsat
{
    public class ServiceSocketWebsocket : ServiceSocket
    {
        public IWebHost iWebHost { get; set; }
        private CancellationToken HTTPServiceCancellationToken = default;

        public ServiceSocketWebsocket(Channel ChannelObject) : base(ChannelObject)
        {
            // Setting up the service type
            Type = "HTTP Websocket";                        
        }


        public IWebHostBuilder CreateWebHostBuilder() =>
          WebHost.CreateDefaultBuilder()
          .ConfigureKestrel(serverOptions =>
          {
              // Setting up listener for the given Port and any IP
              serverOptions.ListenAnyIP(ChannelObject.Port, 
                listenOptions =>
                {
                    // Setting up the TLS settings
                    if (ChannelObject.TLS)
                    {
                        listenOptions.UseHttps(ChannelObject.TLSCert, ChannelObject.TLSCertPassword);
                    }
                });
          })
         ;

        public override void Start() {
      
            iWebHost = new WebHostBuilder()
                .UseKestrel()
                .ConfigureKestrel(serverOptions =>
                {            
                    // Setting up listener for the given Port and any IP
                    serverOptions.ListenAnyIP(ChannelObject.Port, 
                        listenOptions =>
                        {
                            // Setting up the TLS settings
                            if (ChannelObject.TLS)
                            {
                                listenOptions.UseHttps(ChannelObject.TLSCert, ChannelObject.TLSCertPassword);
                            }
                        });
                })
                .Configure(app => {
                    // Set the websocket options
                    WebSocketOptions webSocketOptions = new WebSocketOptions()
                    {
                        // Keepalive may be important for some detections
                        KeepAliveInterval = TimeSpan.FromSeconds(120),
                    };
                    app.UseWebSockets(webSocketOptions);

                    app.Run(async (context) =>
                    {                        
                        string reqURI=context.Request.Path+context.Request.QueryString;

                        // Print out the request details
                        Console.WriteLine(
                            $"HTTP Endpoint: {context.Connection}\n" +
                            $"Received: \n{reqURI}\n" +
                            $"{context.Request.Body}"
                            );

                        Console.WriteLine(ChannelObject.HTTPURI);
                        Console.WriteLine(reqURI);

                        if (
                            // The HTTP request should be presise for the URI, parameters and headers
                            reqURI == ChannelObject.HTTPURI
                            // If HTTP request based headers need to be verified for auth
                            // use context.Request.Headers.Contains and HTTPRequestHeaders
                            )
                        {
                            if (context.WebSockets.IsWebSocketRequest)
                            {
                                // tasking
                                var completion = new TaskCompletionSource<object>();
                                // getting the websocket
                                WebSocket wsocket = await context.WebSockets.AcceptWebSocketAsync();

                                // add the implant to the Implants list
                                Implant implant = GenerateImplant("HTTP Websocket", wsocket);
                                // set the implant endpoint
                                implant.Endpoint = context.Connection.RemoteIpAddress.ToString() + ":" + context.Connection.RemotePort.ToString();
                                Implants.TryAdd(implant.Endpoint, implant);

                                // starting receive from the implant
                                Console.WriteLine("Receive task is starting for {0}.", implant.Endpoint);
                                _ = Task.Run(() => ReceiveAsync(implant.Endpoint,wsocket));
                                await completion.Task;

                            }
                            else
                            {
                                context.Response.ContentType = "text/html";
                                await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(ChannelObject.ResponseError));
                            }
                        }
                        else
                        {
                            context.Response.ContentType = "text/html";
                            await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(ChannelObject.ResponseError));
                        }

                    });
                })
                .Build();
        
            iWebHost.RunAsync(HTTPServiceCancellationToken);
            Status = true;
        }

        public override bool Stop()
        {
            iWebHost.RunAsync(CancellationToken.None);       
            Status = false;
            return false;
        }

        public override bool Send(object imp, byte[] data)
        {
            return true;

        }

        public override async Task ReceiveAsync(string ClientEndPoint,Object obj)
        {

            // Casting the websocket 
            WebSocket wsocket = (WebSocket)obj;

            try
            {
                while (wsocket.State == WebSocketState.Open)
                {
                    // get the buffer length instruction first
                    byte[] rbuffer = new byte[4000];
                    var result = await wsocket.ReceiveAsync(new ArraySegment<byte>(rbuffer), CancellationToken.None);
                    string instruction = Encoding.UTF8.GetString(rbuffer).Replace("\0", string.Empty);

                    // Update the last seen regardless of null connection or data received
                    Implants[ClientEndPoint].LastSeen = DateTime.Now.ToString();

                    Console.WriteLine($"Websocket Endpoint: {ClientEndPoint} Received: {instruction}");

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

                    await wsocket.SendAsync(sdata, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("{0} is disconnected", ClientEndPoint);
                Implant i = Implants[ClientEndPoint];
                await wsocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                i.LastSeen = DateTime.Now.ToString();
                i.Status = "Disconnected";
            }

        }
    }
}