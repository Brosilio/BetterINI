using System;
using System.Runtime.Serialization;

namespace BetterINI
{
	[Serializable]
	internal class MissingIniParam : Exception
	{
		public string ParamName { get; }

		public MissingIniParam()
		{
		}

		public MissingIniParam(string iniParamName) : base($"INI file parameter '{iniParamName}' not found")
		{
			ParamName = iniParamName;
		}

		public MissingIniParam(string iniParamName, Exception innerException) : base($"INI file parameter '{iniParamName}' not found", innerException)
		{
			ParamName = iniParamName;
		}

		protected MissingIniParam(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}