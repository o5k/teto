using System;

namespace Teto {
    public partial class Config {
        /// <summary>
        /// A configuration item.
        /// </summary>
        public class ConfigItem {
            /// <summary>
            /// The original value, as in the .cfg file.
            /// </summary>
            public string StringValue { get; private set; }

            /// <summary>
            /// Construct a config item.
            /// </summary>
            /// <param name="value">The string representation of the value.</param>
            public ConfigItem(string value) {
                StringValue = value;
            }

            /// <summary>
            /// Get a config item as object of type T.
            /// </summary>
            /// <typeparam name="T">The type of object to return.</typeparam>
            /// <returns>The object as type T.</returns>
            public T Value<T>() {
                if (typeof(T) == typeof(int)) {
                    return (T)(object)int.Parse(StringValue);
                }
                if (typeof(T) == typeof(long)) {
                    return (T)(object)long.Parse(StringValue);
                }
                if (typeof(T) == typeof(bool)) {
                    return (T)(object)(StringValue.ToLower() == "true");
                }
                if (typeof(T) == typeof(string)) {
                    return (T)(object)StringValue;
                }

                throw new ArgumentException("The requested type is not valid.");
            }
        }
    }
}
