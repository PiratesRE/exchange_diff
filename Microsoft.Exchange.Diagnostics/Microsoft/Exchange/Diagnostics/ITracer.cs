using System;
using System.IO;

namespace Microsoft.Exchange.Diagnostics
{
	public interface ITracer
	{
		void TraceDebug<T0>(long id, string formatString, T0 arg0);

		void TraceDebug<T0, T1>(long id, string formatString, T0 arg0, T1 arg1);

		void TraceDebug<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TraceDebug(long id, string formatString, params object[] args);

		void TraceDebug(long id, string message);

		void TraceWarning<T0>(long id, string formatString, T0 arg0);

		void TraceWarning(long id, string message);

		void TraceWarning(long id, string formatString, params object[] args);

		void TraceError(long id, string message);

		void TraceError<T0>(long id, string formatString, T0 arg0);

		void TraceError<T0, T1>(long id, string formatString, T0 arg0, T1 arg1);

		void TraceError<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TraceError(long id, string formatString, params object[] args);

		void TracePerformance(long id, string message);

		void TracePerformance<T0>(long id, string formatString, T0 arg0);

		void TracePerformance<T0, T1>(long id, string formatString, T0 arg0, T1 arg1);

		void TracePerformance<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TracePerformance(long id, string formatString, params object[] args);

		void Dump(TextWriter writer, bool addHeader, bool verbose);

		ITracer Compose(ITracer other);

		bool IsTraceEnabled(TraceType traceType);
	}
}
