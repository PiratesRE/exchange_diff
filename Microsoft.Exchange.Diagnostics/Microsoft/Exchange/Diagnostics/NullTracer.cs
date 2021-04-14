using System;
using System.IO;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NullTracer : ITracer
	{
		private NullTracer()
		{
		}

		public void TraceDebug<T0>(long id, string formatString, T0 arg0)
		{
		}

		public void TraceDebug<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
		}

		public void TraceDebug<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
		}

		public void TraceDebug(long id, string formatString, params object[] args)
		{
		}

		public void TraceDebug(long id, string message)
		{
		}

		public void TraceWarning<T0>(long id, string formatString, T0 arg0)
		{
		}

		public void TraceWarning(long id, string message)
		{
		}

		public void TraceWarning(long id, string formatString, params object[] args)
		{
		}

		public void TraceError(long id, string message)
		{
		}

		public void TraceError<T0>(long id, string formatString, T0 arg0)
		{
		}

		public void TraceError<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
		}

		public void TraceError<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
		}

		public void TraceError(long id, string formatString, params object[] args)
		{
		}

		public void TracePerformance(long id, string message)
		{
		}

		public void TracePerformance<T0>(long id, string formatString, T0 arg0)
		{
		}

		public void TracePerformance<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
		}

		public void TracePerformance<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
		}

		public void TracePerformance(long id, string formatString, params object[] args)
		{
		}

		public void Dump(TextWriter writer, bool addHeader, bool verbose)
		{
		}

		public ITracer Compose(ITracer other)
		{
			return other;
		}

		public bool IsTraceEnabled(TraceType traceType)
		{
			return false;
		}

		public static readonly NullTracer Instance = new NullTracer();
	}
}
