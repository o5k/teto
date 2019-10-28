using System.Collections.Generic;

namespace Teto {
    public partial class Webbe {
        /// <summary>
        /// A parameter to a request.
        /// </summary>
        public abstract class Parameter {
            /// <summary>
            /// The key of the parameter.
            /// </summary>
            public string Key { get; protected set; }
            /// <summary>
            /// Get the parameter's value as string.
            /// </summary>
            /// <returns>The parameter's value as string.</returns>
            public abstract string String();
            /// <summary>
            /// Generate the header for this parameter in multipart format.
            /// </summary>
            /// <returns>The generated header.</returns>
            public abstract string MultipartHeader();
            /// <summary>
            /// Get the parameter's value as bytes.
            /// </summary>
            /// <returns>The parameter's value.</returns>
            public abstract byte[] ByteValue();

            /// <summary>
            /// Convert a list of parameters to a string-string dictionary. Do not use if file parameters are involved.
            /// </summary>
            /// <param name="pa">The parameters to convert.</param>
            /// <returns>A string-string dictionary.</returns>
            public static Dictionary<string, string> ToDictionary(Parameter[] pa) {
                Dictionary<string, string> dict = new Dictionary<string, string>();

                foreach (Parameter p in pa) {
                    dict[p.Key] = p.String();
                }

                return dict;
            }
        }
    }
}
