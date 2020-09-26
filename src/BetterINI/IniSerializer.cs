using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace BetterINI
{
	public static class IniSerializer
	{
		/// <summary>
		/// Deserialize the provided INI data into a type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize data into.</typeparam>
		/// <param name="data">The raw INI data.</param>
		public static T Deserialize<T>(string data) where T : class, new()
		{
			T template = new T();
			IniFile ini = IniFile.Parse(data);

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
	}
}
