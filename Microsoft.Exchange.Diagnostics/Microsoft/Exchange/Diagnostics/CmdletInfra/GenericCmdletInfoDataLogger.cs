using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GenericCmdletInfoDataLogger : IPerformanceDataLogger
	{
		private GenericCmdletInfoDataLogger()
		{
		}

		public void Log(string marker, string counter, TimeSpan value)
		{
			CmdletLogger.SafeAppendGenericInfo(string.Format("{0}.{1}", marker, counter), ((long)value.TotalMilliseconds).ToString(NumberFormatInfo.InvariantInfo));
		}

		public void Log(string marker, string counter, uint value)
		{
			CmdletLogger.SafeAppendGenericInfo(string.Format("{0}.{1}", marker, counter), value.ToString(NumberFormatInfo.InvariantInfo));
		}

		public void Log(string marker, string counter, string value)
		{
			CmdletLogger.SafeAppendGenericInfo(string.Format("{0}.{1}", marker, counter), value);
		}

		public static readonly IPerformanceDataLogger Instance = new GenericCmdletInfoDataLogger();
	}
}
