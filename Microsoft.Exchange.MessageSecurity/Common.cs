using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessageSecurity;

namespace Microsoft.Exchange.MessageSecurity
{
	internal static class Common
	{
		public static readonly ExEventLog EventLogger = new ExEventLog(ExTraceGlobals.EdgeCredentialServiceTracer.Category, "MSExchange Message Security");
	}
}
