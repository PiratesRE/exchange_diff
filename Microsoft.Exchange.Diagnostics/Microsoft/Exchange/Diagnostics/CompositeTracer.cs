using System;
using System.IO;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CompositeTracer : ITracer
	{
		public CompositeTracer(ITracer first, ITracer second)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (second == null)
			{
				throw new ArgumentNullException("second");
			}
			this.first = first;
			this.second = second;
		}

		public void TraceDebug<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.first.TraceDebug<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
			this.second.TraceDebug<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceDebug<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.first.TraceDebug<T0, T1>(id, formatString, arg0, arg1);
			this.second.TraceDebug<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TraceDebug<T0>(long id, string formatString, T0 arg0)
		{
			this.first.TraceDebug<T0>(id, formatString, arg0);
			this.second.TraceDebug<T0>(id, formatString, arg0);
		}

		public void TraceDebug(long id, string message)
		{
			this.first.TraceDebug(id, message);
			this.second.TraceDebug(id, message);
		}

		public void TraceDebug(long id, string formatString, params object[] args)
		{
			this.first.TraceDebug(id, formatString, args);
			this.second.TraceDebug(id, formatString, args);
		}

		public void TraceWarning<T0>(long id, string formatString, T0 arg0)
		{
			this.first.TraceWarning<T0>(id, formatString, arg0);
			this.second.TraceWarning<T0>(id, formatString, arg0);
		}

		public void TraceWarning(long id, string message)
		{
			this.first.TraceWarning(id, message);
			this.second.TraceWarning(id, message);
		}

		public void TraceWarning(long id, string formatString, params object[] args)
		{
			this.first.TraceWarning(id, formatString, args);
			this.second.TraceWarning(id, formatString, args);
		}

		public void TraceError(long id, string message)
		{
			this.first.TraceError(id, message);
			this.second.TraceError(id, message);
		}

		public void TraceError<T0>(long id, string formatString, T0 arg0)
		{
			this.first.TraceError<T0>(id, formatString, arg0);
			this.second.TraceError<T0>(id, formatString, arg0);
		}

		public void TraceError<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.first.TraceError<T0, T1>(id, formatString, arg0, arg1);
			this.second.TraceError<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TraceError<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.first.TraceError<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
			this.second.TraceError<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TraceError(long id, string formatString, params object[] args)
		{
			this.first.TraceError(id, formatString, args);
			this.second.TraceError(id, formatString, args);
		}

		public void TracePerformance(long id, string message)
		{
			this.first.TracePerformance(id, message);
			this.second.TracePerformance(id, message);
		}

		public void TracePerformance<T0>(long id, string formatString, T0 arg0)
		{
			this.first.TracePerformance<T0>(id, formatString, arg0);
			this.second.TracePerformance<T0>(id, formatString, arg0);
		}

		public void TracePerformance<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.first.TracePerformance<T0, T1>(id, formatString, arg0, arg1);
			this.second.TracePerformance<T0, T1>(id, formatString, arg0, arg1);
		}

		public void TracePerformance<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.first.TracePerformance<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
			this.second.TracePerformance<T0, T1, T2>(id, formatString, arg0, arg1, arg2);
		}

		public void TracePerformance(long id, string formatString, params object[] args)
		{
			this.first.TracePerformance(id, formatString, args);
			this.second.TracePerformance(id, formatString, args);
		}

		public void Dump(TextWriter writer, bool addHeader, bool verbose)
		{
			this.first.Dump(writer, addHeader, verbose);
			this.second.Dump(writer, addHeader, verbose);
		}

		public ITracer Compose(ITracer other)
		{
			if (other == null || NullTracer.Instance.Equals(other))
			{
				return this;
			}
			return new CompositeTracer(this, other);
		}

		public bool IsTraceEnabled(TraceType traceType)
		{
			return this.first.IsTraceEnabled(traceType) || this.second.IsTraceEnabled(traceType);
		}

		private ITracer first;

		private ITracer second;
	}
}
