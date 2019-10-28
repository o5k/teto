using System.IO;

namespace Teto {
    /// <summary>
    /// Useful extensions to the System.IO.Stream class.
    /// </summary>
    public static class StreamExtensions {
        /// <summary>
        /// Write a buffer to a stream.
        /// </summary>
        /// <param name="stream">The Stream to write to.</param>
        /// <param name="buffer">The bytes to the written to the stream.</param>
        public static void WriteAllBytes(this Stream stream, byte[] buffer) {
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
