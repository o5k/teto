using System;

namespace Teto {
    /// <summary>
    /// A Unix timestamp (in seconds).
    /// </summary>
    public class UnixTimestamp {
        /// <summary>
        /// The value of this timestamp.
        /// </summary>
        public int Value { get; private set; } = 0;
        /// <summary>
        /// Convert a DateTime into a new Unix timestamp.
        /// </summary>
        /// <param name="time"></param>
        public UnixTimestamp(DateTime time) {
            Value = (int)time.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary>
        /// Implicit cast to integer.
        /// </summary>
        /// <param name="u">The timestamp to be cast.</param>
        public static implicit operator int(UnixTimestamp u) => u.Value;
        /// <summary>
        /// Implicit cast to string.
        /// </summary>
        /// <param name="u">The timestamp to be cast.</param>
        public static implicit operator string(UnixTimestamp u) => u.Value.ToString();
    }
}
