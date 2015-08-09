using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace API
{
    public class RequestHandler
    {
        private readonly string rootURL;

        private bool signedIn, signingIn;

        public RequestHandler(string rootURL)
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

        public T Request<T>(string url, RequestMethods method, JObject data) where T : class
        {
            return Request<T>(url, method, ContentTypes.JSON, data.ToString());
        }
        public T Request<T>(string url, RequestMethods method, ContentTypes content, string data) where T : class
        {
            byte[] response = getReponse(rootURL + url, method, content, data);
            string response_str = (response == null || response.Length == 0) ? null : Encoding.UTF8.GetString(response);

            if (typeof(T) == typeof(string))
                return response_str as T;
            else if (typeof(T) == typeof(JToken))
            {
                if (response_str != null)
                    return JToken.Parse(response_str) as T;
                else
                    return null;
            }
            else if(typeof(T) == typeof(XDocument))
            {
                if (response_str != null)
                    return XDocument.Parse(response_str) as T;
                else
                    return null;
            }
            else
                throw new InvalidOperationException($"{nameof(Request)} does not support objects of type {typeof(T).Name}.");
        }
        public T Request<T>(string url, RequestMethods method) where T : class
        {
            return Request<T>(url, method, ContentTypes.Undefined, (string)null);
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
                case ContentTypes.Undefined: return null;

                case ContentTypes.JSON: return "application/json";
                case ContentTypes.URL_Encoded: return "application/x-www-form-urlencoded";
                default:
                    throw new ArgumentException("Unknown content type.", nameof(type));
            }
        }

        private byte[] getReponse(string url, RequestMethods method, ContentTypes content, string data)
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
                    client.ContentType = getContentTypeString(content);
                    client.Method = getMethodString(method);

                    using (var g = client.GetRequestStream())
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
