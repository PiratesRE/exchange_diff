using System;

namespace Microsoft.Exchange.Diagnostics
{
	public interface ITraceTestHook
	{
		bool IsTraceEnabled(TraceType traceType);

		void TraceDebug(int lid, long id, string message);

		void TraceDebug(int lid, long id, string formatString, params object[] args);

		void TraceError(int lid, long id, string message);

		void TraceError(int lid, long id, string formatString, params object[] args);

		void TraceFunction(int lid, long id, string message);

		void TraceFunction(int lid, long id, string formatString, params object[] args);

		void TraceInformation(int lid, long id, string message);

		void TraceInformation(int lid, long id, string formatString, params object[] args);

		void TracePerformance(int lid, long id, string message);

		void TracePerformance(int lid, long id, string formatString, params object[] args);

		void TracePfd(int lid, long id, string message);

		void TracePfd(int lid, long id, string formatString, params object[] args);

		void TraceWarning(int lid, long id, string message);

		void TraceWarning(int lid, long id, string formatString, params object[] args);
	}
}
