using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TracingLogger : ISyncLogger
	{
		public void Information(Trace tracer, long id, string message)
		{
			tracer.Information(id, message);
		}

		public void Information(Trace tracer, long id, string formatString, params object[] args)
		{
			tracer.Information(id, formatString, args);
		}

		public void Information<T0>(Trace tracer, long id, string formatString, T0 arg0)
		{
			tracer.Information<T0>(id, formatString, arg0);
		}

		public void Information<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1)
		{
			tracer.Information<T0, T1>(id, formatString, arg0, arg1);
		}

		public void Information<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			tracer.Information<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceDebug(Trace tracer, long id, string message)
		{
			tracer.TraceDebug(id, message);
		}

		public void TraceDebug(Trace tracer, int lid, long id, string message)
		{
			tracer.TraceDebug(lid, id, message);
		}

		public void TraceDebug(Trace tracer, long id, string formatString, params object[] args)
		{
			tracer.TraceDebug(id, formatString, args);
		}

		public void TraceDebug<T0>(Trace tracer, long id, string formatString, T0 arg0)
		{
			tracer.TraceDebug<T0>(id, formatString, arg0);
		}

		public void TraceDebug(Trace tracer, int lid, long id, string formatString, params object[] args)
		{
			tracer.TraceDebug(lid, id, formatString, args);
		}

		public void TraceDebug<T0>(Trace tracer, int lid, long id, string formatString, T0 arg0)
		{
			tracer.TraceDebug<T0>(lid, id, formatString, arg0);
		}

		public void TraceDebug<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1)
		{
			tracer.TraceDebug<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TraceDebug<T0, T1>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			tracer.TraceDebug<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TraceDebug<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			tracer.TraceDebug<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceDebug<T0, T1, T2>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			tracer.TraceDebug<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		public void TraceError(Trace tracer, long id, string message)
		{
			tracer.TraceError(id, message);
		}

		public void TraceError(Trace tracer, int lid, long id, string message)
		{
			tracer.TraceError(lid, id, message);
		}

		public void TraceError(Trace tracer, long id, string formatString, params object[] args)
		{
			tracer.TraceError(id, formatString, args);
		}

		public void TraceError<T0>(Trace tracer, long id, string formatString, T0 arg0)
		{
			tracer.TraceError<T0>(id, formatString, arg0);
		}

		public void TraceError(Trace tracer, int lid, long id, string formatString, params object[] args)
		{
			tracer.TraceError(lid, id, formatString, args);
		}

		public void TraceError<T0>(Trace tracer, int lid, long id, string formatString, T0 arg0)
		{
			tracer.TraceError<T0>(lid, id, formatString, arg0);
		}

		public void TraceError<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1)
		{
			tracer.TraceError<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TraceError<T0, T1>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			tracer.TraceError<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TraceError<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			tracer.TraceError<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceError<T0, T1, T2>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			tracer.TraceError<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		public void TraceFunction(Trace tracer, long id, string message)
		{
			tracer.TraceFunction(id, message);
		}

		public void TraceFunction(Trace tracer, int lid, long id, string message)
		{
			tracer.TraceFunction(lid, id, message);
		}

		public void TraceFunction(Trace tracer, long id, string formatString, params object[] args)
		{
			tracer.TraceFunction(id, formatString, args);
		}

		public void TraceFunction<T0>(Trace tracer, long id, string formatString, T0 arg0)
		{
			tracer.TraceFunction<T0>(id, formatString, arg0);
		}

		public void TraceFunction(Trace tracer, int lid, long id, string formatString, params object[] args)
		{
			tracer.TraceFunction(lid, id, formatString, args);
		}

		public void TraceFunction<T0>(Trace tracer, int lid, long id, string formatString, T0 arg0)
		{
			tracer.TraceFunction<T0>(lid, id, formatString, arg0);
		}

		public void TraceFunction<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1)
		{
			tracer.TraceFunction<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TraceFunction<T0, T1>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			tracer.TraceFunction<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TraceFunction<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			tracer.TraceFunction<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceFunction<T0, T1, T2>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			tracer.TraceFunction<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		public void TracePfd(Trace tracer, long id, string message)
		{
			tracer.TracePfd(id, message);
		}

		public void TracePfd(Trace tracer, int lid, long id, string message)
		{
			tracer.TracePfd(lid, id, message);
		}

		public void TracePfd(Trace tracer, long id, string formatString, params object[] args)
		{
			tracer.TracePfd(id, formatString, args);
		}

		public void TracePfd<T0>(Trace tracer, long id, string formatString, T0 arg0)
		{
			tracer.TracePfd<T0>(id, formatString, arg0);
		}

		public void TracePfd(Trace tracer, int lid, long id, string formatString, params object[] args)
		{
			tracer.TracePfd(lid, id, formatString, args);
		}

		public void TracePfd<T0>(Trace tracer, int lid, long id, string formatString, T0 arg0)
		{
			tracer.TracePfd<T0>(lid, id, formatString, arg0);
		}

		public void TracePfd<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1)
		{
			tracer.TracePfd<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TracePfd<T0, T1>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			tracer.TracePfd<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TracePfd<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			tracer.TracePfd<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TracePfd<T0, T1, T2>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			tracer.TracePfd<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		public void TraceWarning(Trace tracer, long id, string message)
		{
			tracer.TraceWarning(id, message);
		}

		public void TraceWarning(Trace tracer, int lid, long id, string message)
		{
			tracer.TraceWarning(lid, id, message);
		}

		public void TraceWarning(Trace tracer, long id, string formatString, params object[] args)
		{
			tracer.TraceWarning(id, formatString, args);
		}

		public void TraceWarning<T0>(Trace tracer, long id, string formatString, T0 arg0)
		{
			tracer.TraceWarning<T0>(id, formatString, arg0);
		}

		public void TraceWarning(Trace tracer, int lid, long id, string formatString, params object[] args)
		{
			tracer.TraceWarning(lid, id, formatString, args);
		}

		public void TraceWarning<T0>(Trace tracer, int lid, long id, string formatString, T0 arg0)
		{
			tracer.TraceWarning<T0>(lid, id, formatString, arg0);
		}

		public void TraceWarning<T0, T1>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1)
		{
			tracer.TraceWarning<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TraceWarning<T0, T1>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			tracer.TraceWarning<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TraceWarning<T0, T1, T2>(Trace tracer, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			tracer.TraceWarning<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceWarning<T0, T1, T2>(Trace tracer, int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			tracer.TraceWarning<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		public static readonly TracingLogger Singleton = new TracingLogger();
	}
}
