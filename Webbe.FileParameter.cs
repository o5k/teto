using System;
using System.IO;

namespace Teto {
    partial class Webbe {
        /// <summary>
        /// A file parameter to a request.
        /// </summary>
        public class FileParameter : Parameter {
            /// <summary>
            /// The file name of the file to send.
            /// </summary>
            public string Filename { get; private set; }
            /// <summary>
            /// The bytes in the file.
            /// </summary>
            private byte[] Value;
            /// <summary>
            /// Construct a new file parameter.
            /// </summary>
            /// <param name="key">The key of the parameter.</param>
            /// <param name="filename">The filename to attach.</param>
            /// <param name="value">The content of the file.</param>
            public FileParameter(string key, string filename, byte[] value) {
                Key = key;
                Filename = filename;
                Value = value;
            }
            /// <summary>
            /// Throw an exception, as you can't turn a file into a string. Please use Multipart for files, thank you
            /// </summary>
            /// <returns>Fucking nothing</returns>
            public override string String() {
                throw new NotSupportedException("Cannot cast a string into a file.");
            }
            /// <summary>
            /// Generate the header for this parameter in multipart format.
            /// </summary>
            /// <returns>The generated header.</returns>
            public override string MultipartHeader() {
                return $"Content-Disposition: form-data; name=\"{ Key }\"; filename=\"{ Filename }\"\r\nContent-Type: { (MimeMapping.ContainsKey(Path.GetExtension(Filename)) ? MimeMapping[Path.GetExtension(Filename)] : "application/octet-stream") }";
            }
            /// <summary>
            /// Get the value of the parameter as bytes.
            /// </summary>
            /// <returns>The file.</returns>
            public override byte[] ByteValue() {
                return Value;
            }
        }
    }
}
