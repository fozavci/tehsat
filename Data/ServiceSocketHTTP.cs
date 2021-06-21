using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Tehsat
{
    public class ServiceSocketHTTP : ServiceSocket
    {
        public IWebHostBuilder HTTPService { get; set; }
        public IWebHost iWebHost { get; set; }
        private CancellationToken HTTPServiceCancellationToken = default;
        private string RequestPath;
        private string RequestVariables = "";

        public ServiceSocketHTTP(Channel ChannelObject) : base(ChannelObject)
        {
            // Setting up the service type
            Type = "HTTP";

            // Parsing the path and variables
            if (ChannelObject.HTTPURI.Contains("?"))
            {
                RequestPath = ChannelObject.HTTPURI.Split("?")[0];
                RequestVariables = ChannelObject.HTTPURI.Split("?")[1];
            }
            else
            {
                RequestPath = ChannelObject.HTTPURI;
            }

        }

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
                    app.Run(async (context) =>
                    {
                        string reqURI=context.Request.Path+context.Request.QueryString;

                        // Print out the request details
                        Console.WriteLine(
                            $"HTTP Endpoint: {context.Connection}\n" +
                            $"Received: \n{reqURI}\n" +
                            $"{context.Request.Body}"
                            );

                        if (
                            // The HTTP request should be presise for the URI, parameters and headers
                            reqURI == ChannelObject.HTTPURI
                            // If HTTP request based headers need to be verified for auth
                            // use context.Request.Headers.Contains and HTTPRequestHeaders
                            )
                        {
                            context.Response.ContentType = "text/html";
                            await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(ChannelObject.ResponseContent));
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

        public override void Receive(string ClientEndPoint, Object obj)
        {


        }
    }
}