using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CmdletWriteVerboseTracer : ITracer
	{
		public CmdletWriteVerboseTracer(PSCmdlet cmdlet)
		{
			ArgumentValidator.ThrowIfNull("cmdlet", cmdlet);
			this.cmdlet = cmdlet;
		}

		public void TraceDebug<T0>(long id, string formatString, T0 arg0)
		{
			this.WriteToVerboseStream(id, string.Format(formatString, arg0));
		}

		public void TraceDebug<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.WriteToVerboseStream(id, string.Format(formatString, arg0, arg1));
		}

		public void TraceDebug<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.WriteToVerboseStream(id, string.Format(formatString, arg0, arg1, arg2));
		}

		public void TraceDebug(long id, string formatString, params object[] args)
		{
			this.WriteToVerboseStream(id, string.Format(formatString, args));
		}

		public void TraceDebug(long id, string message)
		{
			this.WriteToVerboseStream(id, message);
		}

		public void TraceWarning<T0>(long id, string formatString, T0 arg0)
		{
			this.WriteToVerboseStream(id, string.Format(formatString, arg0));
		}

		public void TraceWarning(long id, string message)
		{
			this.WriteToVerboseStream(id, message);
		}

		public void TraceWarning(long id, string formatString, params object[] args)
		{
			this.WriteToVerboseStream(id, string.Format(formatString, args));
		}

		public void TraceError(long id, string message)
		{
			this.WriteToVerboseStream(id, message);
		}

		public void TraceError(long id, string formatString, params object[] args)
		{
			this.WriteToVerboseStream(id, string.Format(formatString, args));
		}

		public void TraceError<T0>(long id, string formatString, T0 arg0)
		{
			this.WriteToVerboseStream(id, string.Format(formatString, arg0));
		}

		public void TraceError<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.WriteToVerboseStream(id, string.Format(formatString, arg0, arg1));
		}

		public void TraceError<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.WriteToVerboseStream(id, string.Format(formatString, arg0, arg1, arg2));
		}

		public void TracePerformance(long id, string message)
		{
			this.WriteToVerboseStream(id, message);
		}

		public void TracePerformance(long id, string formatString, params object[] args)
		{
			this.WriteToVerboseStream(id, string.Format(formatString, args));
		}

		public void TracePerformance<T0>(long id, string formatString, T0 arg0)
		{
			this.WriteToVerboseStream(id, string.Format(formatString, arg0));
		}

		public void TracePerformance<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.WriteToVerboseStream(id, string.Format(formatString, arg0, arg1));
		}

		public void TracePerformance<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.WriteToVerboseStream(id, string.Format(formatString, arg0, arg1, arg2));
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
			return true;
		}

		private void WriteToVerboseStream(long id, string message)
		{
			this.cmdlet.WriteVerbose(message);
		}

		private readonly PSCmdlet cmdlet;
	}
}
