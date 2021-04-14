using System;

namespace System.Diagnostics
{
	[Serializable]
	internal enum LoggingLevels
	{
		TraceLevel0,
		TraceLevel1,
		TraceLevel2,
		TraceLevel3,
		TraceLevel4,
		StatusLevel0 = 20,
		StatusLevel1,
		StatusLevel2,
		StatusLevel3,
		StatusLevel4,
		WarningLevel = 40,
		ErrorLevel = 50,
		PanicLevel = 100
	}
}
