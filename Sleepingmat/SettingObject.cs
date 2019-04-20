using System.Collections.Generic;

namespace Sleepingmat
{
    public class SettingObject
    {
        public string name;

        public Dictionary<string, string> settings = new Dictionary<string, string>();

        public void SetValue(object key, object value)
        {
            if (settings.ContainsKey(key.ToString()))
                settings[key.ToString()] = value.ToString();
            else
                settings.Add(key.ToString(), value.ToString());
        }

        public string GetValue(string key)
        {
            if (settings.ContainsKey(key))
                return settings[key];
            else
                return string.Empty;
        }
    }
}