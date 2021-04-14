using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Common.Extensions
{
	internal class Tracer : ITracer
	{
		public Tracer(Trace trace)
		{
			this.trace = trace;
		}

		public bool IsErrorTraceEnabled
		{
			get
			{
				return this.trace.IsTraceEnabled(TraceType.ErrorTrace);
			}
		}

		public bool IsDebugTraceEnabled
		{
			get
			{
				return this.trace.IsTraceEnabled(TraceType.DebugTrace);
			}
		}

		public void TraceError(long id, string formatString, params object[] args)
		{
			this.trace.TraceError(id, formatString, args);
		}

		public void TraceDebug(long id, string formatString, params object[] args)
		{
			this.trace.TraceDebug(id, formatString, args);
		}

		private Trace trace;
	}
}
