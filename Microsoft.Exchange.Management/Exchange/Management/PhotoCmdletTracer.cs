using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoCmdletTracer : ITracer
	{
		public PhotoCmdletTracer(bool tracingEnabled)
		{
			if (!tracingEnabled)
			{
				this.output = NullTracer.Instance;
				return;
			}
			this.output = new InMemoryTracer(ExTraceGlobals.UserPhotosTracer.Category, ExTraceGlobals.UserPhotosTracer.TraceTag);
		}

		public void TraceDebug<T0>(long id, string formatString, T0 arg0)
		{
			this.output.TraceDebug(id, string.Format(formatString, arg0));
		}

		public void TraceDebug<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.output.TraceDebug(id, string.Format(formatString, arg0, arg1));
		}

		public void TraceDebug<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.output.TraceDebug(id, string.Format(formatString, arg0, arg1, arg2));
		}

		public void TraceDebug(long id, string formatString, params object[] args)
		{
			this.output.TraceDebug(id, string.Format(formatString, args));
		}

		public void TraceDebug(long id, string message)
		{
			this.output.TraceDebug(id, message);
		}

		public void TraceWarning<T0>(long id, string formatString, T0 arg0)
		{
			this.output.TraceWarning(id, string.Format(formatString, arg0));
		}

		public void TraceWarning(long id, string message)
		{
			this.output.TraceWarning(id, message);
		}

		public void TraceWarning(long id, string formatString, params object[] args)
		{
			this.output.TraceWarning(id, string.Format(formatString, args));
		}

		public void TraceError(long id, string message)
		{
			this.output.TraceError(id, message);
		}

		public void TraceError(long id, string formatString, params object[] args)
		{
			this.output.TraceError(id, string.Format(formatString, args));
		}

		public void TraceError<T0>(long id, string formatString, T0 arg0)
		{
			this.output.TraceError(id, string.Format(formatString, arg0));
		}

		public void TraceError<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.output.TraceError(id, string.Format(formatString, arg0, arg1));
		}

		public void TraceError<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.output.TraceError(id, string.Format(formatString, arg0, arg1, arg2));
		}

		public void TracePerformance(long id, string message)
		{
			this.output.TracePerformance(id, message);
		}

		public void TracePerformance(long id, string formatString, params object[] args)
		{
			this.output.TracePerformance(id, string.Format(formatString, args));
		}

		public void TracePerformance<T0>(long id, string formatString, T0 arg0)
		{
			this.output.TracePerformance(id, string.Format(formatString, arg0));
		}

		public void TracePerformance<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.output.TracePerformance(id, string.Format(formatString, arg0, arg1));
		}

		public void TracePerformance<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.output.TracePerformance(id, string.Format(formatString, arg0, arg1, arg2));
		}

		public void Dump(TextWriter writer, bool addHeader, bool verbose)
		{
			throw new NotSupportedException();
		}

		public ITracer Compose(ITracer other)
		{
			return new CompositeTracer(this, other);
		}

		public bool IsTraceEnabled(TraceType traceType)
		{
			return this.output.IsTraceEnabled(traceType);
		}

		public void Dump(ITraceEntryWriter writer)
		{
			ArgumentValidator.ThrowIfNull("writer", writer);
			if (NullTracer.Instance.Equals(this.output))
			{
				return;
			}
			((InMemoryTracer)this.output).Dump(writer);
		}

		private readonly ITracer output;
	}
}
