using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Wcf
{
	public class WcfTraceListener : TraceListener
	{
		public override void Write(string message)
		{
			ExTraceGlobals.WCFTracer.TraceDebug(0L, message);
		}

		public override void WriteLine(string message)
		{
			ExTraceGlobals.WCFTracer.TraceDebug(0L, message);
		}
	}
}
