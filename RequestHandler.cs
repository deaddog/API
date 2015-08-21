using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

        protected virtual Task SignIn()
        {
            return Task.Delay(0);
        }

        protected virtual void SetCredentials(HttpWebRequest request)
        {
        }
        protected virtual void SetCredentials(QueryValues query)
        {
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
            HttpWebRequest request = await CreateRequest(url, method, data, contentType);
            return await GetResponse<T>(request);
        }
        public async Task<T> Request<T>(string url, RequestMethods method) where T : class
        {
            return await Request<T>(url, method, new byte[0], ContentTypes.Undefined);
        }

        public async Task<HttpWebRequest> CreateRequest(string url, RequestMethods method, XDocument data, ContentTypes contentType = ContentTypes.XML)
        {
            return await CreateRequest(url, method, data.ToString(SaveOptions.DisableFormatting), contentType);
        }
        public async Task<HttpWebRequest> CreateRequest(string url, RequestMethods method, JToken data, ContentTypes contentType = ContentTypes.JSON)
        {
            return await CreateRequest(url, method, data.ToString(Newtonsoft.Json.Formatting.None), contentType);
        }
        public async Task<HttpWebRequest> CreateRequest(string url, RequestMethods method, object data, ContentTypes contentType)
        {
            return await CreateRequest(url, method, data?.ToString(), contentType);
        }
        public async Task<HttpWebRequest> CreateRequest(string url, RequestMethods method, string data, ContentTypes contentType)
        {
            return await CreateRequest(url, method, data == null ? null : encoding.GetBytes(data), contentType);
        }
        public async Task<HttpWebRequest> CreateRequest(string url, RequestMethods method, byte[] data, ContentTypes contentType)
        {
            HttpWebRequest request = await CreateRequest(url, method);
            request.ContentType = getContentTypeString(contentType);

            if (data != null && data.Length > 0)
            {
                if (method == RequestMethods.GET)
                    throw new ArgumentException($"Data cannot be transferred using the {nameof(RequestMethods.GET)} method. Embed data as query string.");

                using (var stream = await request.GetRequestStreamAsync())
                    stream.Write(data, 0, data.Length);
            }

            return request;
        }
        public async Task<HttpWebRequest> CreateRequest(string url, RequestMethods method)
        {
            HttpWebRequest request = await CreateRequest(url);
            request.Method = request.Method = getMethodString(method);
            return request;
        }
        public async Task<HttpWebRequest> CreateRequest(string url)
        {
            if (!signedIn && !signingIn)
            {
                signingIn = true;
                await SignIn();
                signedIn = true;
                signingIn = false;
            }

            if (signedIn)
                url = applyCredentialsQuery(url);

            HttpWebRequest request = HttpWebRequest.CreateHttp(rootURL + url);
            if (signedIn)
                SetCredentials(request);

            return request;
        }

        private string applyCredentialsQuery(string url)
        {
            QueryValues values = new QueryValues();
            SetCredentials(values);
            return applyQuery(url, values);
        }
        private string applyQuery(string url, QueryValues values)
        {
            if (values.Count == 0)
                return url;

            url += url.Contains("?") ? "&" : "?";
            url += $"{values[0].Key}={values[0].Value}";
            for (int i = 1; i < values.Count; i++)
                url += $"&{values[i].Key}={values[1].Value}";

            return url;
        }

        public async Task<T> GetResponse<T>(HttpWebRequest request) where T : class
        {
            using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
                return await GetResponse<T>(response);
        }
        public async Task<T> GetResponse<T>(HttpWebResponse response) where T : class
        {
            byte[] data = await readWebResponse(response);

            if (typeof(T) == typeof(byte[]))
                return data as T;

            string response_str = (data == null || data.Length == 0) ? null : encoding.GetString(data);

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
                throw new InvalidOperationException($"{nameof(GetResponse)} does not support objects of type {typeof(T).Name}.");
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

        private static async Task<byte[]> readWebResponse(HttpWebResponse response)
        {
            byte[] responseBuffer = new byte[0];

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                using (MemoryStream ms = new MemoryStream())
                {
                    await response.GetResponseStream().CopyToAsync(ms);
                    responseBuffer = ms.ToArray();
                }
            else
                throw new WebException((response as HttpWebResponse).StatusDescription);

            return responseBuffer;
        }

        public class QueryValues
        {
            private List<KeyValuePair<string, string>> list;

            public QueryValues()
            {
                list = new List<KeyValuePair<string, string>>();
            }

            public void Add(string key, string value, bool encode = true)
            {
                if (encode)
                {
                    key = HttpUtility.UrlEncode(key);
                    value = HttpUtility.UrlEncode(value);
                }

                list.Add(new KeyValuePair<string, string>(key, value));
            }

            public int Count
            {
                get { return list.Count; }
            }
            public KeyValuePair<string, string> this[int index]
            {
                get { return list[index]; }
            }
        }
    }
}
