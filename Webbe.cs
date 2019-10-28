using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;

namespace Teto {
    /// <summary>
    /// A better WebClient that can do multipart and also doesn't explode when it receives something that isn't a 200
    /// </summary>
    public partial class Webbe {
        /// <summary>
        /// The Random instance (used for multipart boundaries)
        /// </summary>
        private Random rng = new Random();
        /// <summary>
        /// The HTTP headers associated with this client.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Construct a new Webbe client.
        /// </summary>
        public Webbe() {
            Headers["Connection"] = "close";
            Headers["User-Agent"] = "Teto/Webbe";
        }

        /// <summary>
        /// Generate a new random multipart-ready boundary.
        /// </summary>
        /// <returns>The multipart boundary.</returns>
        private string GenerateMultipartBoundary() {
            byte[] bytes = new byte[16];
            rng.NextBytes(bytes);

            string unfiltered = Convert.ToBase64String(bytes);

            return "---------------------" + new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled).Replace(unfiltered, "");
        }

        /// <summary>
        /// Turn Parameters into a form body.
        /// </summary>
        /// <param name="parameters">The form parameters to convert.</param>
        /// <returns>The bytes for the body.</returns>
        private byte[] FormulateFormBody(Parameter[] parameters) {
            string bodyString = "";

            foreach (Parameter p in parameters) {
                bodyString += Uri.EscapeDataString(p.Key) + "=" + Uri.EscapeDataString(p.String()) + "&";
            }
            return Encoding.UTF8.GetBytes(bodyString.Substring(0, bodyString.Length - 1));
        }

        /// <summary>
        /// Turn Parameters into a multipart body.
        /// </summary>
        /// <param name="parameters">The form parameters to convert.</param>
        /// <param name="boundary">The form boundary.</param>
        /// <returns>The bytes for the body.</returns>
        private byte[] FormulateMultipartBody(Parameter[] parameters, string boundary) {
            List<byte> output = new List<byte>();

            foreach (Parameter p in parameters) {
                output.AddRange(Encoding.UTF8.GetBytes("--" + boundary + "\r\n"));
                output.AddRange(Encoding.UTF8.GetBytes(p.MultipartHeader() + "\r\n\r\n"));
                output.AddRange(p.ByteValue());
                output.AddRange(Encoding.UTF8.GetBytes("\r\n"));
            }
            output.AddRange(Encoding.UTF8.GetBytes("--" + boundary + "--\r\n"));

            return output.ToArray();
        }

        /// <summary>
        /// Upload a Form to a server.
        /// </summary>
        /// <param name="method">The HTTP method to use.</param>
        /// <param name="url">The URL to request to.</param>
        /// <param name="parameters">The parameters to send.</param>
        /// <returns>The server's response.</returns>
        public Response UploadForm(string method, string url, Parameter[] parameters) {
            Headers["Content-Type"] = "application/x-www-form-urlencoded";
            byte[] body = FormulateFormBody(parameters);

            return Upload(method, url, body);
        }

        /// <summary>
        /// Upload a Multipart form to a server.
        /// </summary>
        /// <param name="method">The HTTP method to use.</param>
        /// <param name="url">The URL to request to.</param>
        /// <param name="parameters">The parameters to send.</param>
        /// <returns>The server's response.</returns>
        public Response UploadMultipart(string method, string url, Parameter[] parameters) {
            string boundary = GenerateMultipartBoundary();

            Headers["Content-Type"] = "multipart/form-data; boundary=" + boundary;
            byte[] body = FormulateMultipartBody(parameters, boundary);

            return Upload(method, url, body);
        }

        /// <summary>
        /// Upload a byte body to a server.
        /// </summary>
        /// <param name="method">The HTTP method to use.</param>
        /// <param name="url">The URL to request to.</param>
        /// <param name="body">The body data to send.</param>
        /// <returns>The server's response.</returns>
        private Response Upload(string method, string url, byte[] body) {
            Uri uri = new Uri(url);

            Headers["Host"] = uri.Host;
            Headers["Content-Length"] = body.LongLength.ToString();

            using (TcpClient tc = new TcpClient()) {
                tc.Connect(uri.Host, uri.Port);

                using (Stream ss = uri.Scheme == "https" ? new SslStream(tc.GetStream()) : (Stream)tc.GetStream()) {
                    if (uri.Scheme == "https") {
                        ((SslStream)ss).AuthenticateAsClient(uri.Host, null, SslProtocols.Tls12, true);
                    }
                    // Method header
                    ss.WriteAllBytes(Encoding.UTF8.GetBytes(method + " " + uri.AbsolutePath + " HTTP/1.1\r\n"));

                    // Headers
                    foreach (KeyValuePair<string, string> h in Headers) {
                        ss.WriteAllBytes(Encoding.UTF8.GetBytes(h.Key + ": " + h.Value + "\r\n"));
                    }

                    ss.WriteAllBytes(Encoding.UTF8.GetBytes("\r\n"));

                    // Body
                    ss.WriteAllBytes(body);

                    // Response's code header
                    string responseCodeString = ReadStreamLine(ss);
                    int responseCode = int.Parse(responseCodeString.Split(' ')[1]);

                    // Response headers
                    Dictionary<string, string> headers = new Dictionary<string, string>();

                    while (true) {
                        string header = ReadStreamLine(ss);
                        if (header == "") {
                            break;
                        }

                        string[] pcs = header.Split(new string[] { ": " }, StringSplitOptions.None);
                        headers[pcs[0]] = string.Join(": ", pcs.Skip(1));
                    }

                    // Response body
                    List<byte> responseBody = new List<byte>();

                    while (true) {
                        int ib = ss.ReadByte();

                        if (ib == -1) {
                            break;
                        }

                        responseBody.Add((byte)ib);
                    }

                    return new Response(responseCode, responseBody.ToArray(), headers);
                }
            }
        }

        /// <summary>
        /// Read a line from a stream.
        /// </summary>
        /// <param name="s">The stream to read.</param>
        /// <returns>The line read.</returns>
        private string ReadStreamLine(Stream s) {
            List<byte> output = new List<byte>();

            while (true) {
                int ib = s.ReadByte();

                if (ib == -1) {
                    return Encoding.UTF8.GetString(output.ToArray());
                }

                byte b = (byte)ib;

                output.Add(b);

                string str = Encoding.UTF8.GetString(output.ToArray());

                if (str.EndsWith("\r\n")) {
                    return str.Substring(0, str.Length - 2);
                }
            }
        }
    }
}
