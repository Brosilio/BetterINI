using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterINI
{
	/// <summary>
	/// Represents a Key=Value style configuration file in memory.
	/// <para />
	/// Use <see cref="Parse"/> or similar to obtain one.
	/// </summary>
	public class IniFile
	{
		private readonly Dictionary<string, string> data;
		public string this[string key] => KeyExists(key) ? data[key] : throw new KeyNotFoundException($"Key {key} not found in IniFile");

		private IniFile()
		{
			this.data = new Dictionary<string, string>();
		}

		/// <summary>
		/// Read an INI file from disk and parse it.
		/// </summary>
		/// <param name="fileName">The full path to the ini file.</param>
		/// <returns>An ini file object.</returns>
		public static IniFile ParseFromDisk(string fileName)
		{
			return Parse(File.ReadAllText(fileName));
		}

		/// <summary>
		/// Parse INI key=value pairs from the provided string.
		/// </summary>
		/// <param name="data">The raw data.</param>
		/// <exception cref="ArgumentNullException" />
		public static IniFile Parse(string data)
		{
			IniFile f = new IniFile();

			StringReader sr = new StringReader(data);

			while (true)
			{
				string line = sr.ReadLine();
				if (line == null) break;

				if (ParseLine(line, out string key, out string value))
					f.Add(key, value);
			}

			return f;
		}

		/// <summary>
		/// Parse an INI file out of a stream. Will return when the end of stream is reached. The stream will not be disposed.
		/// </summary>
		/// <param name="stream">A stream to read the INI data from.</param>
		/// <returns></returns>
		public static async Task<IniFile> ParseAsync(Stream stream)
		{
			StreamReader reader = new StreamReader(stream);
			IniFile ini = new IniFile();

			while (true)
			{
				string line = await reader.ReadLineAsync();
				if (line == null) break;

				if (ParseLine(line, out string key, out string value))
					ini.Add(key, value);
			}

			return ini;
		}

		/// <summary>
		/// Parse an INI file out of a stream. Will return when the end of stream is reached. The stream will not be disposed.
		/// </summary>
		/// <param name="stream">A stream to read the INI data from.</param>
		public static IniFile Parse(Stream stream)
		{
			return ParseAsync(stream).GetAwaiter().GetResult();
		}

		/// <summary>
		/// Parse a single line of INI garbage into a key=value pair.
		/// </summary>
		/// <param name="line">The source line.</param>
		/// <param name="outKey">The parsed key name.</param>
		/// <param name="outValue">The parsed value.</param>
		/// <returns><see langword="true"/> if the line was valid, <see langword="false"/> if it was not.</returns>
		private static bool ParseLine(string line, out string outKey, out string outValue)
		{
			outKey = null;
			outValue = null;

			if (line == null)
				return false;

			line = line.Trim();
			if (line.StartsWith("#") || !line.Contains('=') || line.StartsWith("="))
				return false;

			outKey = line.Substring(0, line.IndexOf('=')).Trim();
			outValue = line.IndexOf('=') + 1 >= line.Length ? string.Empty : line.Substring(line.IndexOf('=') + 1).Trim();
			return true;
		}

		/// <summary>
		/// Get an array of strings from a key=value pair by splitting the value by the specified character.
		/// </summary>
		/// <param name="key">The name of the key.</param>
		/// <param name="split">The character to split the value by.</param>
		/// <exception cref="KeyNotFoundException" />
		public string[] GetArray(string key, char split)
		{
			return this[key].Split(split).Select(x => x.Trim()).ToArray();
		}

		/// <summary>
		/// Like <see cref="GetArray(string, char)"/>, but will return an empty string array if the key does not exist, instead of throwing an exception.
		/// </summary>
		/// <param name="key">The name of the key.</param>
		/// <param name="split">The character to split the value by.</param>
		public string[] GetArraySafe(string key, char split)
		{
			if (!IsSet(key))
				return Array.Empty<string>();

			return GetArray(key, split);
		}

		/// <summary>
		/// Appends or overwrites a key/value pair in the internal dictionary.
		/// </summary>
		/// <param name="key">The name of the key. This is case-sensitive.</param>
		/// <param name="value">The value.</param>
		private void Add(string key, string value)
		{
			if (data.ContainsKey(key))
				data[key] = value;
			else
				data.Add(key, value);
		}

		/// <summary>
		/// Get an integer value from the configuration data. Will throw <see cref="KeyNotFoundException"/> if the key does not exist.
		/// </summary>
		/// <param name="key">The name of the value.</param>
		/// <exception cref="KeyNotFoundException" />
		public int GetInt(string key)
		{
			if (!IsSet(key))
				throw new KeyNotFoundException($"Key {key} not found in IniFile");

			return int.Parse(this[key]);
		}

		/// <summary>
		/// Try to get a value. Returns null if not found.
		/// </summary>
		/// <param name="key">The key.</param>
		public string SafeGet(string key)
		{
			if (IsSet(key))
				return this[key];

			return null;
		}

		/// <summary>
		/// Parse an Enum from the configuration data. Will return null on failure.
		/// </summary>
		/// <param name="enumType">The type of the enum to parse.</param>
		/// <param name="key">The name of the value in the configuration file.</param>
		public bool TryGetEnum<TEnum>(string key, out TEnum? result) where TEnum : struct
		{
			if (IsSet(key))
			{
				if (Enum.TryParse<TEnum>(this[key], out TEnum value))
				{
					result = value;
					return true;
				}
			}

			result = null;
			return false;
		}


		/// <summary>
		/// Get a boolean value. Returns <paramref name="defaultValue"/> if the key is not found.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value to return if the key is not found.</param>
		/// <returns></returns>
		public bool GetBoolSafe(string key, bool defaultValue = false)
		{
			if (!IsSet(key))
				return defaultValue;

			return GetBool(key);
		}

		/// <summary>
		/// Get a bool. Will throw <see cref="KeyNotFoundException"/> if the key is not found.
		/// </summary>
		/// <param name="key">The key.</param>
		public bool GetBool(string key)
		{
			if (!IsSet(key))
				throw new KeyNotFoundException($"Key {key} not found in IniFile");

			return this[key].ToLower() == "true";
		}

		/// <summary>
		/// Return true if all the specified key names exist in this configuration file.
		/// </summary>
		/// <param name="keys">A list of key names.</param>
		public bool DoAllExist(params string[] keys)
		{
			foreach (string k in keys)
			{
				if (!IsSet(k))
					return false;
			}

			return true;
		}

		public override string ToString()
		{
			return ToString(false);
		}

		public string ToString(bool align = false)
		{
			StringBuilder sb = new StringBuilder();

			int longest = -1;

			// i hate myself for this line of code
			if (align) foreach (string key in data.Keys) if (key.Length > longest) longest = key.Length;

			foreach (string key in data.Keys)
			{
				string k = align ? key.PadRight(longest) : key;
				sb.AppendLine($"{k} = {data[key]}");
			}

			return sb.ToString();
		}

		/// <summary>
		/// Returns <see langword="true"/> if the specified key exists. This is case-sensitive.
		/// </summary>
		/// <param name="key">The name of the key. This is case-sensitive.</param>
		/// <returns>Returns true if the key exists.</returns>
		public bool KeyExists(string key) => data != null && data.ContainsKey(key);

		/// <summary>
		/// Returns true if the specified key exists and its associated value is not null, empty, or whitespace.
		/// </summary>
		/// <param name="key">The name of the key.</param>
		public bool IsSet(string key) => KeyExists(key) && !string.IsNullOrWhiteSpace(this[key]);

		/// <summary>
		/// Throw an exception if the specified key is not set, null, empty, or whitespace.
		/// </summary>
		/// <param name="key">The key</param>
		public void Assert(string key)
		{
			if (!IsSet(key))
				throw new Exception($"The key \"{key}\" was not set.");
		}
	}
}
