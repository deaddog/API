using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace API
{
    public class RequestHandler
    {
        private readonly string rootURL;
        private Encoding encoding;
        private ContentTypes contentType;

        private bool signedIn, signingIn;

        public RequestHandler(string rootURL)
            : this(rootURL, Encoding.UTF8)
        {
        }
        public RequestHandler(string rootURL, Encoding encoding)
        {
            if (rootURL == null)
                throw new ArgumentNullException(nameof(rootURL));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            this.rootURL = rootURL.TrimEnd('/');
            this.encoding = encoding;
            this.contentType = ContentTypes.Undefined;

            this.signingIn = false;
            this.signedIn = false;
        }

        public Encoding Encoding
        {
            get { return encoding; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                encoding = value;
            }
        }
        public ContentTypes DefaultContentType
        {
            get { return contentType; }
            set { contentType = value; }
        }

        protected virtual void SignIn()
        {
        }

        protected virtual void SetCredentials(HttpWebRequest request)
        {
        }

        public async Task<T> Get<T>(string url, XDocument data, ContentTypes contentType = ContentTypes.XML) where T : class
        {
            return await Request<T>(url, RequestMethods.GET, data, contentType);
        }
        public async Task<T> Get<T>(string url, JToken data, ContentTypes contentType = ContentTypes.JSON) where T : class
        {
            return await Request<T>(url, RequestMethods.GET, data, contentType);
        }
        public async Task<T> Get<T>(string url, object data, ContentTypes contentType) where T : class
        {
            return await Request<T>(url, RequestMethods.GET, data, contentType);
        }
        public async Task<T> Get<T>(string url, string data, ContentTypes contentType) where T : class
        {
            return await Request<T>(url, RequestMethods.GET, data, contentType);
        }
        public async Task<T> Get<T>(string url, byte[] data, ContentTypes contentType) where T : class
        {
            return await Request<T>(url, RequestMethods.GET, data, contentType);
        }
        public async Task<T> Get<T>(string url) where T : class
        {
            return await Request<T>(url, RequestMethods.GET);
        }

        public async Task<T> Put<T>(string url, XDocument data, ContentTypes contentType = ContentTypes.XML) where T : class
        {
            return await Request<T>(url, RequestMethods.PUT, data, contentType);
        }
        public async Task<T> Put<T>(string url, JToken data, ContentTypes contentType = ContentTypes.JSON) where T : class
        {
            return await Request<T>(url, RequestMethods.PUT, data, contentType);
        }
        public async Task<T> Put<T>(string url, object data, ContentTypes contentType) where T : class
        {
            return await Request<T>(url, RequestMethods.PUT, data, contentType);
        }
        public async Task<T> Put<T>(string url, string data, ContentTypes contentType) where T : class
        {
            return await Request<T>(url, RequestMethods.PUT, data, contentType);
        }
        public async Task<T> Put<T>(string url, byte[] data, ContentTypes contentType) where T : class
        {
            return await Request<T>(url, RequestMethods.PUT, data, contentType);
        }
        public async Task<T> Put<T>(string url) where T : class
        {
            return await Request<T>(url, RequestMethods.PUT);
        }

        public async Task<T> Post<T>(string url, XDocument data, ContentTypes contentType = ContentTypes.XML) where T : class
        {
            return await Request<T>(url, RequestMethods.POST, data, contentType);
        }
        public async Task<T> Post<T>(string url, JToken data, ContentTypes contentType = ContentTypes.JSON) where T : class
        {
            return await Request<T>(url, RequestMethods.POST, data, contentType);
        }
        public async Task<T> Post<T>(string url, object data, ContentTypes contentType) where T : class
        {
            return await Request<T>(url, RequestMethods.POST, data, contentType);
        }
        public async Task<T> Post<T>(string url, string data, ContentTypes contentType) where T : class
        {
            return await Request<T>(url, RequestMethods.POST, data, contentType);
        }
        public async Task<T> Post<T>(string url, byte[] data, ContentTypes contentType) where T : class
        {
            return await Request<T>(url, RequestMethods.POST, data, contentType);
        }
        public async Task<T> Post<T>(string url) where T : class
        {
            return await Request<T>(url, RequestMethods.POST);
        }

        public async Task<T> Delete<T>(string url, XDocument data, ContentTypes contentType = ContentTypes.XML) where T : class
        {
            return await Request<T>(url, RequestMethods.DELETE, data, contentType);
        }
        public async Task<T> Delete<T>(string url, JToken data, ContentTypes contentType = ContentTypes.JSON) where T : class
        {
            return await Request<T>(url, RequestMethods.DELETE, data, contentType);
        }
        public async Task<T> Delete<T>(string url, object data, ContentTypes contentType) where T : class
        {
            return await Request<T>(url, RequestMethods.DELETE, data, contentType);
        }
        public async Task<T> Delete<T>(string url, string data, ContentTypes contentType) where T : class
        {
            return await Request<T>(url, RequestMethods.DELETE, data, contentType);
        }
        public async Task<T> Delete<T>(string url, byte[] data, ContentTypes contentType) where T : class
        {
            return await Request<T>(url, RequestMethods.DELETE, data, contentType);
        }
        public async Task<T> Delete<T>(string url) where T : class
        {
            return await Request<T>(url, RequestMethods.DELETE);
        }

        public async Task<T> Request<T>(string url, RequestMethods method, XDocument data, ContentTypes contentType = ContentTypes.XML) where T : class
        {
            return await Request<T>(url, method, data.ToString(SaveOptions.DisableFormatting), contentType);
        }
        public async Task<T> Request<T>(string url, RequestMethods method, JToken data, ContentTypes contentType = ContentTypes.JSON) where T : class
        {
            return await Request<T>(url, method, data.ToString(Newtonsoft.Json.Formatting.None), contentType);
        }
        public async Task<T> Request<T>(string url, RequestMethods method, object data, ContentTypes contentType) where T : class
        {
            return await Request<T>(url, method, data?.ToString(), contentType);
        }
        public async Task<T> Request<T>(string url, RequestMethods method, string data, ContentTypes contentType) where T : class
        {
            return await Request<T>(url, method, data == null ? null : encoding.GetBytes(data), contentType);
        }
        public async Task<T> Request<T>(string url, RequestMethods method, byte[] data, ContentTypes contentType) where T : class
        {
            byte[] response = await getReponse(url, method, contentType, data);
            string response_str = (response == null || response.Length == 0) ? null : encoding.GetString(response);

            if (typeof(T) == typeof(string))
                return response_str as T;
            else if (typeof(T) == typeof(JToken))
            {
                if (response_str != null)
                    return JToken.Parse(response_str) as T;
                else
                    return null;
            }
            else if (typeof(T) == typeof(JArray))
            {
                if (response_str != null)
                    return JArray.Parse(response_str) as T;
                else
                    return null;
            }
            else if (typeof(T) == typeof(JObject))
            {
                if (response_str != null)
                    return JObject.Parse(response_str) as T;
                else
                    return null;
            }
            else if (typeof(T) == typeof(XDocument))
            {
                if (response_str != null)
                    return XDocument.Parse(response_str) as T;
                else
                    return null;
            }
            else
                throw new InvalidOperationException($"{nameof(Request)} does not support objects of type {typeof(T).Name}.");
        }
        public async Task<T> Request<T>(string url, RequestMethods method) where T : class
        {
            return await Request<T>(url, method, new byte[0], ContentTypes.Undefined);
        }


        public HttpWebRequest CreateRequest(string url, RequestMethods method)
        {
            HttpWebRequest request = CreateRequest(url);
            request.Method = request.Method = getMethodString(method);
            return request;
        }
        public HttpWebRequest CreateRequest(string url)
        {
            if (!signedIn && !signingIn)
            {
                signingIn = true;
                SignIn();
                signedIn = true;
                signingIn = false;
            }

            HttpWebRequest request = HttpWebRequest.CreateHttp(rootURL + url);
            if (signedIn)
                SetCredentials(request);

            return request;
        }

        protected string RequestString(string url, RequestMethods method, ContentTypes content, string data, out Dictionary<string, string[]> headers)
        {
            Dictionary<string, string[]> temp = null;
            byte[] response = getReponse(url, method, content, encoding.GetBytes(data), x => temp = headersToDictionary(x)).Result;
            headers = temp;

            return (response == null || response.Length == 0) ? null : encoding.GetString(response);
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
                case ContentTypes.XML: return "application/xml";
                default:
                    throw new ArgumentException("Unknown content type.", nameof(type));
            }
        }

        private static Dictionary<string, string[]> headersToDictionary(WebHeaderCollection headers)
        {
            Dictionary<string, string[]> dict = new Dictionary<string, string[]>();
            for (int i = 0; i < headers.Count; i++)
                dict.Add(headers.Keys[i], headers.GetValues(i));
            return dict;
        }

        private async Task<byte[]> getReponse(string url, RequestMethods method, ContentTypes content, byte[] data, Action<WebHeaderCollection> headerReader = null)
        {
            HttpWebRequest client = CreateRequest(url);

            byte[] buffer = data == null ? new byte[0] : data;
            byte[] responseBuffer = new byte[0];
            
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

                    using (var g = await client.GetRequestStreamAsync())
                        g.Write(buffer, 0, buffer.Length);

                    try
                    {
                        HttpWebResponse response = await client.GetResponseAsync() as HttpWebResponse;
                        headerReader?.Invoke(response.Headers);
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
                using (MemoryStream ms = new MemoryStream())
                {
                    response.GetResponseStream().CopyTo(ms);
                    responseBuffer = ms.ToArray();
                }
            }
            else
                throw new WebException((response as HttpWebResponse).StatusDescription);

            response.Dispose();

            return responseBuffer;
        }
    }
}
