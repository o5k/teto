using System.Text;

namespace Teto {
    partial class Webbe {
        /// <summary>
        /// A form parameter to a request.
        /// </summary>
        public class FormParameter : Parameter {
            /// <summary>
            /// The value of the parameter.
            /// </summary>
            private string Value;
            /// <summary>
            /// Construct a new form parameter.
            /// </summary>
            /// <param name="key">The key of the parameter.</param>
            /// <param name="value">The value of the parameter.</param>
            public FormParameter(string key, string value) {
                Key = key;
                Value = value;
            }
            /// <summary>
            /// Return the parameter's value as string.
            /// </summary>
            /// <returns>The parameter's value.</returns>
            public override string String() {
                return Value;
            }
            /// <summary>
            /// Generate the header for this parameter in multipart format.
            /// </summary>
            /// <returns>The generated header.</returns>
            public override string MultipartHeader() {
                return $"Content-Disposition: form-data; name=\"{ Key }\"";
            }
            /// <summary>
            /// Return the parameter's value as bytes.
            /// </summary>
            /// <returns>The parameter's value.</returns>
            public override byte[] ByteValue() {
                return Encoding.UTF8.GetBytes(Value);
            }
        }
    }
}
