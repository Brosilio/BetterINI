using System;
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
				IniParamAttribute ipa = info.GetCustomAttribute<IniParamAttribute>();
				
				if (ipa == null)
					continue;

				string name = ipa.Name ?? info.Name;

				if (!ini.IsSet(name))
				{
					if (ipa.Required) throw new MissingIniParam(name);
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
	}
}
