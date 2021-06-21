using System;
using System.Collections.Generic;

namespace Tehsat
{
    public class Channel
    {
        // Generic channel options
        public int Port { get; set; }
        public string Host { get; set; }
        public string ProfileId { get; set; }
        public string Protocol { get; set; }  // "HTTP", "HTTP Websocket", "TCP", "UDP" 
        public string RequestContent { get; set; }
        public bool Binary { get; set; }
        public string ResponseContent { get; set; }
        public string ResponseError { get; set; }
        public bool TLS { get; set; }
        public string TLSCert { get; set; }
        public string TLSCertPassword { get; set; }
        public int Interval { get; set; }
        public int Jitter { get; set; }

        // HTTP specific channel options
        public string HTTPUserAgent { get; set; }
        public string HTTPRequestMethod { get; set; }
        public string HTTPURI { get; set; }
        private Dictionary<String, String> cookies = new Dictionary<String, String>();
        private Dictionary<String, String> httpRequestHeaders = new Dictionary<String, String>();
        private Dictionary<String, String> httpResponseHeaders = new Dictionary<String, String>();

        // Get URI
        public String GetUri()
        {
            // convert the protocols to URI heads
            String p = Protocol.ToLower().Replace("http websocket", "ws");
            // add an s if TLS is on
            if (TLS) { p = p + "s"; }
            // combine all for URI
            String uri = p + "://" + Host + ":" + Port + HTTPURI;
            return uri;
        }

        // Cloning
        public Channel Clone()
        {
            Channel newchannel = new Channel()
            {
                Port = this.Port,
                ProfileId = this.ProfileId,
                Host = this.Host,
                Binary = this.Binary,
                Protocol = this.Protocol,
                RequestContent = this.RequestContent,
                ResponseContent = this.ResponseContent,
                ResponseError = this.ResponseError,
                Interval = this.Interval,
                Jitter = this.Jitter,
                TLS = this.TLS,
                TLSCert = this.TLSCert,
                TLSCertPassword = this.TLSCertPassword,
                HTTPUserAgent = this.HTTPUserAgent,
                HTTPRequestMethod = this.HTTPRequestMethod,
                HTTPURI = this.HTTPURI,
                httpRequestHeaders = this.httpRequestHeaders,
                httpResponseHeaders = this.httpResponseHeaders,

            };

            return newchannel;
        }
        public Dictionary<String, String> Cookies
        {
            get
            {
                return this.cookies;
            }
            set
            {
                this.cookies = value;
            }
        }
        public Dictionary<String, String> HTTPRequestHeaders
        {
            get
            {
                return this.httpRequestHeaders;
            }
            set
            {
                this.httpRequestHeaders = value;
            }
        }
        public Dictionary<String, String> HTTPResponseHeaders
        {
            get
            {
                return this.httpResponseHeaders;
            }
            set
            {
                this.httpResponseHeaders = value;
            }
        }

        public Boolean AddCookie(string cname, string ccontent)
        {
            if (Cookies.ContainsKey(cname))
            {
                return false;
            }
            else
            {
                Cookies.Add(cname, ccontent);
                return true;
            }
        }

        public Boolean AddHTTPRequestHeader(string hname, string hcontent)
        {
            if (HTTPRequestHeaders.ContainsKey(hname))
            {
                return false;
            }
            else
            {
                HTTPRequestHeaders.Add(hname, hcontent);
                return true;
            }
        }

        public Boolean AddHTTPResponseHeader(string hname, string hcontent)
        {
            if (HTTPResponseHeaders.ContainsKey(hname))
            {
                return false;
            }
            else
            {
                HTTPResponseHeaders.Add(hname, hcontent);
                return true;
            }
        }

        public Boolean EditCookie(string cname, string ccontent)
        {
            if (Cookies.ContainsKey(cname))
            {
                Cookies[cname] = ccontent;
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean EditHTTPRequestHeader(string hname, string hcontent)
        {
            if (HTTPRequestHeaders.ContainsKey(hname)) {
                HTTPRequestHeaders[hname] = hcontent;
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean EditHTTPResponseHeader(string hname, string hcontent)
        {
            if (HTTPResponseHeaders.ContainsKey(hname))
            {
                HTTPResponseHeaders[hname] = hcontent;
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean DeleteCookie(string cname, string ccontent)
        {
            if (HTTPRequestHeaders.ContainsKey(cname))
            {
                HTTPRequestHeaders.Remove(cname);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean DeleteHTTPRequestHeader(string hname, string hcontent)
        {
            if (HTTPRequestHeaders.ContainsKey(hname))
            {
                HTTPRequestHeaders.Remove(hname);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean DeleteHTTPResponseHeader(string hname, string hcontent)
        {
            if (HTTPResponseHeaders.ContainsKey(hname))
            {
                HTTPResponseHeaders.Remove(hname);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
