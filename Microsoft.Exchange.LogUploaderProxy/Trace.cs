using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public class Trace
	{
		internal Trace(Trace traceImpl)
		{
			ArgumentValidator.ThrowIfNull("traceImpl", traceImpl);
			this.traceImpl = traceImpl;
		}

		public void Dump(TextWriter writer, bool addHeader, bool verbose)
		{
			this.traceImpl.Dump(writer, addHeader, verbose);
		}

		public bool IsTraceEnabled(TraceType traceType)
		{
			return this.traceImpl.IsTraceEnabled((TraceType)traceType);
		}

		public void Information(long id, string message)
		{
			this.traceImpl.Information(id, message);
		}

		public void Information(long id, string formatString, params object[] args)
		{
			this.traceImpl.Information(id, formatString, args);
		}

		public void Information<T0>(long id, string formatString, T0 arg0)
		{
			this.traceImpl.Information<T0>(id, formatString, arg0);
		}

		public void Information<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.traceImpl.Information<T0, T1>(id, formatString, arg0, arg1);
		}

		public void Information<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.traceImpl.Information<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceDebug(long id, string message)
		{
			this.traceImpl.TraceDebug(id, message);
		}

		public void TraceDebug(int lid, long id, string message)
		{
			this.traceImpl.TraceDebug(lid, id, message);
		}

		public void TraceDebug(long id, string formatString, params object[] args)
		{
			this.traceImpl.TraceDebug(id, formatString, args);
		}

		public void TraceDebug<T0>(long id, string formatString, T0 arg0)
		{
			this.traceImpl.TraceDebug<T0>(id, formatString, arg0);
		}

		public void TraceDebug(int lid, long id, string formatString, params object[] args)
		{
			this.traceImpl.TraceDebug(lid, id, formatString, args);
		}

		public void TraceDebug<T0>(int lid, long id, string formatString, T0 arg0)
		{
			this.traceImpl.TraceDebug<T0>(lid, id, formatString, arg0);
		}

		public void TraceDebug<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.traceImpl.TraceDebug<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TraceDebug<T0, T1>(int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			this.traceImpl.TraceDebug<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TraceDebug<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.traceImpl.TraceDebug<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceDebug<T0, T1, T2>(int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.traceImpl.TraceDebug<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		public void TraceError(long id, string message)
		{
			this.traceImpl.TraceError(id, message);
		}

		public void TraceError(int lid, long id, string message)
		{
			this.traceImpl.TraceError(lid, id, message);
		}

		public void TraceError(long id, string formatString, params object[] args)
		{
			this.traceImpl.TraceError(id, formatString, args);
		}

		public void TraceError<T0>(long id, string formatString, T0 arg0)
		{
			this.traceImpl.TraceError<T0>(id, formatString, arg0);
		}

		public void TraceError(int lid, long id, string formatString, params object[] args)
		{
			this.traceImpl.TraceError(lid, id, formatString, args);
		}

		public void TraceError<T0>(int lid, long id, string formatString, T0 arg0)
		{
			this.traceImpl.TraceError<T0>(lid, id, formatString, arg0);
		}

		public void TraceError<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.traceImpl.TraceError<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TraceError<T0, T1>(int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			this.traceImpl.TraceError<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TraceError<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.traceImpl.TraceError<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceError<T0, T1, T2>(int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.traceImpl.TraceError<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		public void TraceFunction(long id, string message)
		{
			this.traceImpl.TraceFunction(id, message);
		}

		public void TraceFunction(int lid, long id, string message)
		{
			this.traceImpl.TraceFunction(lid, id, message);
		}

		public void TraceFunction(long id, string formatString, params object[] args)
		{
			this.traceImpl.TraceFunction(id, formatString, args);
		}

		public void TraceFunction<T0>(long id, string formatString, T0 arg0)
		{
			this.traceImpl.TraceFunction<T0>(id, formatString, arg0);
		}

		public void TraceFunction(int lid, long id, string formatString, params object[] args)
		{
			this.traceImpl.TraceFunction(lid, id, formatString, args);
		}

		public void TraceFunction<T0>(int lid, long id, string formatString, T0 arg0)
		{
			this.traceImpl.TraceFunction<T0>(lid, id, formatString, arg0);
		}

		public void TraceFunction<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.traceImpl.TraceFunction<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TraceFunction<T0, T1>(int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			this.traceImpl.TraceFunction<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TraceFunction<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.traceImpl.TraceFunction<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceFunction<T0, T1, T2>(int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.traceImpl.TraceFunction<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		public void TraceInformation(int lid, long id, string message)
		{
			this.traceImpl.TraceInformation(lid, id, message);
		}

		public void TraceInformation(int lid, long id, string formatString, params object[] args)
		{
			this.traceImpl.TraceInformation(lid, id, formatString, args);
		}

		public void TraceInformation<T0>(int lid, long id, string formatString, T0 arg0)
		{
			this.traceImpl.TraceInformation<T0>(lid, id, formatString, arg0);
		}

		public void TraceInformation<T0, T1>(int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			this.traceImpl.TraceInformation<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TraceInformation<T0, T1, T2>(int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.traceImpl.TraceInformation<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		public void TracePerformance(long id, string message)
		{
			this.traceImpl.TracePerformance(id, message);
		}

		public void TracePerformance(int lid, long id, string message)
		{
			this.traceImpl.TracePerformance(lid, id, message);
		}

		public void TracePerformance(long id, string formatString, params object[] args)
		{
			this.traceImpl.TracePerformance(id, formatString, args);
		}

		public void TracePerformance<T0>(long id, string formatString, T0 arg0)
		{
			this.traceImpl.TracePerformance<T0>(id, formatString, arg0);
		}

		public void TracePerformance(int lid, long id, string formatString, params object[] args)
		{
			this.traceImpl.TracePerformance(lid, id, formatString, args);
		}

		public void TracePerformance<T0>(int lid, long id, string formatString, T0 arg0)
		{
			this.traceImpl.TracePerformance<T0>(lid, id, formatString, arg0);
		}

		public void TracePerformance<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.traceImpl.TracePerformance<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TracePerformance<T0, T1>(int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			this.traceImpl.TracePerformance<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TracePerformance<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.traceImpl.TracePerformance<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TracePerformance<T0, T1, T2>(int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.traceImpl.TracePerformance<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		public void TracePfd(long id, string message)
		{
			this.traceImpl.TracePfd(id, message);
		}

		public void TracePfd(int lid, long id, string message)
		{
			this.traceImpl.TracePfd(lid, id, message);
		}

		public void TracePfd(long id, string formatString, params object[] args)
		{
			this.traceImpl.TracePfd(id, formatString, args);
		}

		public void TracePfd<T0>(long id, string formatString, T0 arg0)
		{
			this.traceImpl.TracePfd<T0>(id, formatString, arg0);
		}

		public void TracePfd(int lid, long id, string formatString, params object[] args)
		{
			this.traceImpl.TracePfd(lid, id, formatString, args);
		}

		public void TracePfd<T0>(int lid, long id, string formatString, T0 arg0)
		{
			this.traceImpl.TracePfd<T0>(lid, id, formatString, arg0);
		}

		public void TracePfd<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.traceImpl.TracePfd<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TracePfd<T0, T1>(int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			this.traceImpl.TracePfd<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TracePfd<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.traceImpl.TracePfd<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TracePfd<T0, T1, T2>(int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.traceImpl.TracePfd<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		public void TraceWarning(long id, string message)
		{
			this.traceImpl.TraceWarning(id, message);
		}

		public void TraceWarning(int lid, long id, string message)
		{
			this.traceImpl.TraceWarning(lid, id, message);
		}

		public void TraceWarning(long id, string formatString, params object[] args)
		{
			this.traceImpl.TraceWarning(id, formatString, args);
		}

		public void TraceWarning<T0>(long id, string formatString, T0 arg0)
		{
			this.traceImpl.TraceWarning<T0>(id, formatString, arg0);
		}

		public void TraceWarning(int lid, long id, string formatString, params object[] args)
		{
			this.traceImpl.TraceWarning(lid, id, formatString, args);
		}

		public void TraceWarning<T0>(int lid, long id, string formatString, T0 arg0)
		{
			this.traceImpl.TraceWarning<T0>(lid, id, formatString, arg0);
		}

		public void TraceWarning<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.traceImpl.TraceWarning<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TraceWarning<T0, T1>(int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			this.traceImpl.TraceWarning<T0, T1>(lid, id, formatString, arg0, arg1);
		}

		public void TraceWarning<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.traceImpl.TraceWarning<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceWarning<T0, T1, T2>(int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.traceImpl.TraceWarning<T0, T1, T2>(lid, id, formatString, arg0, arg1, arg2);
		}

		private Trace traceImpl;
	}
}
