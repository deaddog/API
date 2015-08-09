using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace API
{
    public class JsonRequestHandler
    {
        private readonly string rootURL;

        private bool signedIn, signingIn;

        public JsonRequestHandler(string rootURL)
        {
            this.rootURL = rootURL.TrimEnd('/');

            this.signingIn = false;
            this.signedIn = false;
        }

        protected virtual void SignIn()
        {
        }

        protected virtual void SetCredentials(HttpWebRequest request)
        {
        }

        public JToken Request(string url, RequestMethods method, JObject data)
        {
            return Request(url, method, data.ToString());
        }
        public JToken Request(string url, RequestMethods method, string data)
        {
            byte[] response = getReponse(rootURL + url, method, data);

            if (response != null && response.Length > 0)
                return JToken.Parse(Encoding.UTF8.GetString(response));
            else
                return null;
        }
        public JToken Request(string url, RequestMethods method)
        {
            return Request(url, method, (string)null);
        }

        private static string getMethodString(RequestMethods method)
        {
            switch (method)
            {
                case RequestMethods.GET: return "GET";
                case RequestMethods.PUT: return "PUT";
                case RequestMethods.POST: return "POST";
                case RequestMethods.DELETE: return "DELETE";
                default:
                    throw new ArgumentException("Unknown request method.", nameof(method));
            }
        }
        private static string getContentTypeString(ContentTypes type)
        {
            switch (type)
            {
                case ContentTypes.JSON: return "application/json";
                case ContentTypes.URL_Encoded: return "application/x-www-form-urlencoded";
                default:
                    throw new ArgumentException("Unknown content type.", nameof(type));
            }
        }
        private byte[] getReponse(string url, RequestMethods method, string data)
        {
            if (!signedIn && !signingIn)
            {
                signingIn = true;
                SignIn();
                signedIn = true;
                signingIn = false;
            }

            byte[] buffer = data == null ? new byte[0] : Encoding.UTF8.GetBytes(data);
            byte[] responseBuffer = new byte[0];

            HttpWebRequest client = System.Net.HttpWebRequest.CreateHttp(url);
            if (signedIn)
                SetCredentials(client);

            switch (method)
            {
                case RequestMethods.GET:
                    responseBuffer = handleWebResponse(client);
                    break;

                case RequestMethods.PUT:
                case RequestMethods.POST:
                case RequestMethods.DELETE:
                    client.ContentType = "application/json";
                    client.Method = getMethodString(method);

                    var g = client.GetRequestStream();

                    g.Write(buffer, 0, buffer.Length);

                    try
                    {
                        HttpWebResponse response = client.GetResponse() as HttpWebResponse;
                    }
                    catch (WebException e)
                    {
                        var resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                        throw new WebException(resp, e);
                    }

                    responseBuffer = handleWebResponse(client);

                    break;
            }

            return responseBuffer;
        }

        private static byte[] handleWebResponse(HttpWebRequest client)
        {
            byte[] responseBuffer = new byte[0];
            HttpWebResponse response = null;
            try
            {
                response = client.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                throw e;
            }
            var g = response.StatusCode;

            if (g == HttpStatusCode.OK || g == HttpStatusCode.Created)
            {
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                responseBuffer = ms.ToArray();
            }
            else
            {
                throw new WebException((response as HttpWebResponse).StatusDescription);
            }
            response.Dispose();

            return responseBuffer;
        }
    }
}
