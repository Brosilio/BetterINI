﻿using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace BetterINI
{
	/// <summary>
	/// Provides access to INI file serialization functionality.
	/// </summary>
	public static class IniSerializer
	{
		/// <summary>
		/// Deserialize the provided INI file into a type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize data into.</typeparam>
		/// <param name="ini">An input <see cref="IniFile"/> from which to suck data out of.</param>
		public static T Deserialize<T>(IniFile ini) where T : class, new()
		{
			T template = new T();

			foreach (FieldInfo info in typeof(T).GetFields())
			{
				IniIgnoreAttribute ignore = info.GetCustomAttribute<IniIgnoreAttribute>();
				if (ignore != null) continue;

				IniParamAttribute ipa = info.GetCustomAttribute<IniParamAttribute>();
				if (ipa == null) continue;

				string name = ipa.Name ?? info.Name;

				if (!ini.KeyExists(name))
				{
					if (ipa.Required && !ini.IsSet(name)) throw new MissingIniParamException(name);

					if (ipa.Default != null)
						info.SetValue(template, ipa.Default);

					continue;
				}

				info.SetValue(template, TypeDescriptor.GetConverter(info.FieldType).ConvertFromInvariantString(ini[name]));

			}

			return template;
		}

		/// <summary>
		/// Deserialize the provided INI-file-style string into a type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize data into.</typeparam>
		/// <param name="data">The data.</param>
		public static T Deserialize<T>(string data) where T : class, new()
		{
			return Deserialize<T>(IniFile.Parse(data));
		}

		/// <summary>
		/// Deserialize the provided stream into a type. Will return when the stream reaches its end.
		/// </summary>
		/// <typeparam name="T">The type to deserialize the data into.</typeparam>
		/// <param name="stream">A stream from which to read the INI data.</param>
		public static T Deserialize<T>(Stream stream) where T : class, new()
		{
			return DeserializeAsync<T>(stream).GetAwaiter().GetResult();
		}

		/// <summary>
		/// Deserialize the provided stream into a type. Will return when the stream reaches its end.
		/// </summary>
		/// <typeparam name="T">The type to deserialize the data into.</typeparam>
		/// <param name="stream">A stream from which to read the INI data.</param>
		public static async Task<T> DeserializeAsync<T>(Stream stream) where T : class, new()
		{
			return Deserialize<T>(await IniFile.ParseAsync(stream));
		}

		/// <summary>
		/// Serialize an object with public fields into an IniFile. Use <see cref="IniIgnoreAttribute"/> to ignore specific fields.
		/// </summary>
		/// <typeparam name="T">The type of the object to serialize.</typeparam>
		/// <param name="obj">The instance of the object to serialize.</param>
		/// <returns>An IniFile containing the serialized data.</returns>
		public static IniFile Serialize<T>(T obj) where T : class
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));

			IniFile ini = new IniFile();

			foreach (FieldInfo info in typeof(T).GetFields())
			{
				IniIgnoreAttribute ignore = info.GetCustomAttribute<IniIgnoreAttribute>();
				if (ignore != null) continue;

				IniParamAttribute ipa = info.GetCustomAttribute<IniParamAttribute>();

				string name = info.Name;
				bool required = false;
				string fallback = "";

				if (ipa != null)
				{
					required = ipa.Required;
					if (ipa.Name != null) name = ipa.Name;
					if (ipa.Default != null) fallback = ipa.Default.ToString();
				}

				object val = info.IsStatic ? info.GetValue(null) : info.GetValue(obj);

				if (val != null)
					ini.Add(name, val.ToString());
				else if (val == null && required)
					ini.Add(name, fallback);
			}

			return ini;
		}
	}
}
