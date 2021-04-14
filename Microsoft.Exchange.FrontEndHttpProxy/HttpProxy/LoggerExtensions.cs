using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class LoggerExtensions
	{
		internal static void SafeSet(this RequestDetailsLogger logger, Enum key, object value)
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(logger, key, value);
		}
	}
}
