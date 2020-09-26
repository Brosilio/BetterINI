using System;
using System.Collections.Generic;
using System.Text;

namespace BetterINI
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class IniParamAttribute : Attribute
	{
		public string Name { get; }
		public bool Required { get; set; }

		public IniParamAttribute() { }

		public IniParamAttribute(string name)
		{
			this.Name = name;
		}
	}
}
