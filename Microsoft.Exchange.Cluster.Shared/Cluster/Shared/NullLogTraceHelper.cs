using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal class NullLogTraceHelper : ILogTraceHelper
	{
		public static ILogTraceHelper GetNullLogger()
		{
			return NullLogTraceHelper.s_nullLogger;
		}

		public void AppendLogMessage(LocalizedString locMessage)
		{
			string arg = DateTime.UtcNow.ToString("s");
			ExTraceGlobals.ClusterTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[{0}] {1}", arg, locMessage.ToString());
		}

		public void AppendLogMessage(string englishMessage, params object[] args)
		{
			ExTraceGlobals.ClusterTracer.TraceDebug((long)this.GetHashCode(), englishMessage, args);
		}

		public void WriteVerbose(LocalizedString locString)
		{
			this.AppendLogMessage(locString);
		}

		private static NullLogTraceHelper s_nullLogger = new NullLogTraceHelper();
	}
}
