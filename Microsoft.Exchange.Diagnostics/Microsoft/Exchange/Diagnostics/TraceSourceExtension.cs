using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Diagnostics
{
	internal static class TraceSourceExtension
	{
		public static void SetMaxDataSize(this TraceSource traceSource, int value)
		{
			traceSource.Attributes["maxdatasize"] = value.ToString();
		}
	}
}
