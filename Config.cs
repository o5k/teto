using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Teto {
    /// <summary>
    /// A configuration, saved as a .cfg file.
    /// </summary>
    public partial class Config {
        /// <summary>
        /// Whether this config exists on disk.
        /// </summary>
        public bool Exists { get; private set; } = false;
        /// <summary>
        /// The path of the config file.
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// The items in the config file.
        /// </summary>
        private Dictionary<string, ConfigItem> items;

        /// <summary>
        /// Construct a config file.
        /// </summary>
        /// <param name="path">The path of the config file to be loaded.</param>
        public Config(string path) {
            Path = path;
            Load();
        }

        /// <summary>
        /// (Re)loads a config file.
        /// </summary>
        public void Load() {
            items = new Dictionary<string, ConfigItem>();

            if (!File.Exists(Path)) {
                Exists = false;
                return;
            }

            Exists = true;
            string[] lines = File.ReadAllLines(Path);

            foreach (string line in lines) {
                if (line.StartsWith("#") || !line.Contains("=")) {
                    continue;
                }

                string[] pcs = line.Split('=');

                items[pcs[0]] = new ConfigItem(string.Join("=", pcs.Skip(1)));
            }
        }

        /// <summary>
        /// Obtain a config item by key.
        /// </summary>
        /// <param name="key">The key of the config item.</param>
        /// <returns>The config item, or null if not found.</returns>
        public ConfigItem this[string key] {
            get {
                if (items.ContainsKey(key)) {
                    return items[key];
                } else {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get an array of keys that are missing or have no value.
        /// </summary>
        /// <param name="keys">The keys that must be in the config.</param>
        /// <returns>The missing keys.</returns>
        public string[] RequireKeys(string[] keys) {
            return keys.Except(items.Keys.Where(o => items[o].StringValue.Trim() != "")).ToArray();
        }
    }
}
