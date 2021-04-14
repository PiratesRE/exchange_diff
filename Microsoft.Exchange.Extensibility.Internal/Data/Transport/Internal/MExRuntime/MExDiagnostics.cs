using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MExRuntime;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal static class MExDiagnostics
	{
		public static ExEventLog EventLog
		{
			get
			{
				return MExDiagnostics.logger;
			}
		}

		private static readonly ExEventLog logger = new ExEventLog(ExTraceGlobals.DispatchTracer.Category, "MSExchange Extensibility");
	}
}
