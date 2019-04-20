using System.Collections.Generic;

namespace Sleepingmat
{
    /// <summary>
    /// The Block class is a block.
    /// </summary>
    public class Block
    {
        /// <summary>
        /// The name of this block.
        /// </summary>
        public string name;

        /// <summary>
        /// All the key/value pairs located inside this block.
        /// </summary>
        public Dictionary<string, string> settings = new Dictionary<string, string>();

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public void SetValue(object key, object value)
        {
            if (settings.ContainsKey(key.ToString()))
                settings[key.ToString()] = value.ToString();
            else
                settings.Add(key.ToString(), value.ToString());
        }

        /// <summary>
        /// Gets the specified value
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The value you want, or an empty string if it didn't exist</returns>
        public string GetValue(string key)
        {
            if (settings.ContainsKey(key))
                return settings[key];
            else
                return string.Empty;
        }
    }
}