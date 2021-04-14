using System;
using System.ComponentModel;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class ExceptionFactory
	{
		internal static LocalizedException LocalizedExceptionFromWin32Error(int error, string message)
		{
			LocalizedException ex = new Win32InteropException(new Win32Exception(error));
			ExceptionFactory.SecurityTracer.TraceDebug<int>((long)ex.GetHashCode(), message, error);
			return ex;
		}

		private static readonly Trace SecurityTracer = ExTraceGlobals.SecurityTracer;
	}
}
