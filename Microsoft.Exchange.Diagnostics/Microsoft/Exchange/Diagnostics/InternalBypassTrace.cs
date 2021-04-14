using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal sealed class InternalBypassTrace
	{
		private InternalBypassTrace(Guid guid, int traceTag)
		{
			this.category = guid;
			this.traceTag = traceTag;
		}

		public void TraceDebug(int lid, long id, string formatString, params object[] args)
		{
			if (ETWTrace.IsInternalTraceEnabled)
			{
				ETWTrace.InternalWrite(lid, TraceType.DebugTrace, this.category, this.traceTag, id, string.Format(formatString, args));
			}
		}

		public void TraceError(int lid, long id, string formatString, params object[] args)
		{
			if (ETWTrace.IsInternalTraceEnabled)
			{
				ETWTrace.InternalWrite(lid, TraceType.ErrorTrace, this.category, this.traceTag, id, string.Format(formatString, args));
			}
		}

		internal static readonly InternalBypassTrace TracingConfigurationTracer = new InternalBypassTrace(CommonTags.guid, 9);

		internal static readonly InternalBypassTrace FaultInjectionConfigurationTracer = new InternalBypassTrace(CommonTags.guid, 10);

		private readonly Guid category;

		private readonly int traceTag;
	}
}
