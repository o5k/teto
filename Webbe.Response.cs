using System.Collections.Generic;
using System.Text;

namespace Teto
{
    public partial class Webbe
    {
        /// <summary>
        /// A server's response to an HTTP request.
        /// </summary>
		public class Response {
            /// <summary>
            /// The response code (e.g. 200).
            /// </summary>
            public int Code { get; private set; }
            /// <summary>
            /// The binary body returned.
            /// </summary>
            public byte[] Data { get; private set; }
            /// <summary>
            /// The encoding of this response.
            /// </summary>
            public Encoding Encoding { get; private set; } = Encoding.UTF8;
            /// <summary>
            /// The headers received.
            /// </summary>
            public Dictionary<string, string> Headers { get; private set; }
            /// <summary>
            /// The binary body as a string.
            /// </summary>
            public string DataString {
				get {
                    return Encoding.GetString(Data);
                }
            }

            /// <summary>
            /// Construct a response.
            /// </summary>
            /// <param name="code">The response code.</param>
            /// <param name="data">The body data.</param>
            /// <param name="headers">The headers received.</param>
			public Response(int code, byte[] data, Dictionary<string, string> headers) {
                Code = code;
                Data = data;
                Headers = headers;
            }
        }
    }
}
