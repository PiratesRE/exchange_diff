using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISyncLogger
	{
		void Information(Trace tracer, long id, string message);

		void Information(Trace tracer, long id, string formatString, params object[] args);

		void Information<T0>(Trace tracer, long id, string formatString, T0 arg0);

		void Information<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1);

		void Information<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TraceDebug(Trace tracer, long id, string message);

		void TraceDebug(Trace tracer, int lid, long id, string message);

		void TraceDebug(Trace tracer, long id, string formatString, params object[] args);

		void TraceDebug<T0>(Trace tracer, long id, string formatString, T0 arg0);

		void TraceDebug(Trace tracer, int lid, long id, string formatString, params object[] args);

		void TraceDebug<T0>(Trace tracer, int lid, long id, string formatString, T0 arg0);

		void TraceDebug<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1);

		void TraceDebug<T0, T1>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1);

		void TraceDebug<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TraceDebug<T0, T1, T2>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TraceError(Trace tracer, long id, string message);

		void TraceError(Trace tracer, int lid, long id, string message);

		void TraceError(Trace tracer, long id, string formatString, params object[] args);

		void TraceError<T0>(Trace tracer, long id, string formatString, T0 arg0);

		void TraceError(Trace tracer, int lid, long id, string formatString, params object[] args);

		void TraceError<T0>(Trace tracer, int lid, long id, string formatString, T0 arg0);

		void TraceError<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1);

		void TraceError<T0, T1>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1);

		void TraceError<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TraceError<T0, T1, T2>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TraceFunction(Trace tracer, long id, string message);

		void TraceFunction(Trace tracer, int lid, long id, string message);

		void TraceFunction(Trace tracer, long id, string formatString, params object[] args);

		void TraceFunction<T0>(Trace tracer, long id, string formatString, T0 arg0);

		void TraceFunction(Trace tracer, int lid, long id, string formatString, params object[] args);

		void TraceFunction<T0>(Trace tracer, int lid, long id, string formatString, T0 arg0);

		void TraceFunction<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1);

		void TraceFunction<T0, T1>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1);

		void TraceFunction<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TraceFunction<T0, T1, T2>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TracePfd(Trace tracer, long id, string message);

		void TracePfd(Trace tracer, int lid, long id, string message);

		void TracePfd(Trace tracer, long id, string formatString, params object[] args);

		void TracePfd<T0>(Trace tracer, long id, string formatString, T0 arg0);

		void TracePfd(Trace tracer, int lid, long id, string formatString, params object[] args);

		void TracePfd<T0>(Trace tracer, int lid, long id, string formatString, T0 arg0);

		void TracePfd<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1);

		void TracePfd<T0, T1>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1);

		void TracePfd<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TracePfd<T0, T1, T2>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TraceWarning(Trace tracer, long id, string message);

		void TraceWarning(Trace tracer, int lid, long id, string message);

		void TraceWarning(Trace tracer, long id, string formatString, params object[] args);

		void TraceWarning<T0>(Trace tracer, long id, string formatString, T0 arg0);

		void TraceWarning(Trace tracer, int lid, long id, string formatString, params object[] args);

		void TraceWarning<T0>(Trace tracer, int lid, long id, string formatString, T0 arg0);

		void TraceWarning<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1);

		void TraceWarning<T0, T1>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1);

		void TraceWarning<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TraceWarning<T0, T1, T2>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2);
	}
}
