using System;
using System.Globalization;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Tracking;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class TraceWrapper
	{
		private TraceWrapper()
		{
		}

		public static TraceWrapper SearchLibraryTracer
		{
			get
			{
				return TraceWrapper.searchLibraryTracer;
			}
		}

		public void Register(TraceWrapper.ITraceWriter traceWriter)
		{
			TraceWrapper.traceWriter = traceWriter;
			Interlocked.Increment(ref this.enabledCount);
		}

		public void Unregister()
		{
			Interlocked.Decrement(ref this.enabledCount);
			TraceWrapper.traceWriter = null;
		}

		public void TraceDebug<T0>(int id, string formatString, T0 arg0)
		{
			if (this.enabledCount > 0 && TraceWrapper.traceWriter != null)
			{
				this.RedirectTrace("Debug", id, formatString, new object[]
				{
					arg0
				});
			}
			ExTraceGlobals.SearchLibraryTracer.TraceDebug<T0>((long)id, formatString, arg0);
		}

		public void TraceDebug<T0, T1>(int id, string formatString, T0 arg0, T1 arg1)
		{
			if (this.enabledCount > 0 && TraceWrapper.traceWriter != null)
			{
				this.RedirectTrace("Debug", id, formatString, new object[]
				{
					arg0,
					arg1
				});
			}
			ExTraceGlobals.SearchLibraryTracer.TraceDebug<T0, T1>((long)id, formatString, arg0, arg1);
		}

		public void TraceDebug<T0, T1, T2>(int id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (this.enabledCount > 0 && TraceWrapper.traceWriter != null)
			{
				this.RedirectTrace("Debug", id, formatString, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
			ExTraceGlobals.SearchLibraryTracer.TraceDebug<T0, T1, T2>((long)id, formatString, arg0, arg1, arg2);
		}

		public void TraceDebug(int id, string formatString, params object[] args)
		{
			if (this.enabledCount > 0 && TraceWrapper.traceWriter != null)
			{
				this.RedirectTrace("Debug", id, formatString, args);
			}
			ExTraceGlobals.SearchLibraryTracer.TraceDebug((long)id, formatString, args);
		}

		public void TraceError<T0>(int id, string formatString, T0 arg0)
		{
			if (this.enabledCount > 0 && TraceWrapper.traceWriter != null)
			{
				this.RedirectTrace("Error", id, formatString, new object[]
				{
					arg0
				});
			}
			ExTraceGlobals.SearchLibraryTracer.TraceError<T0>((long)id, formatString, arg0);
		}

		public void TraceError<T0, T1>(int id, string formatString, T0 arg0, T1 arg1)
		{
			if (this.enabledCount > 0 && TraceWrapper.traceWriter != null)
			{
				this.RedirectTrace("Error", id, formatString, new object[]
				{
					arg0,
					arg1
				});
			}
			ExTraceGlobals.SearchLibraryTracer.TraceError<T0, T1>((long)id, formatString, arg0, arg1);
		}

		public void TraceError<T0, T1, T2>(int id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (this.enabledCount > 0 && TraceWrapper.traceWriter != null)
			{
				this.RedirectTrace("Error", id, formatString, new object[]
				{
					arg0,
					arg1,
					arg2
				});
			}
			ExTraceGlobals.SearchLibraryTracer.TraceError<T0, T1, T2>((long)id, formatString, arg0, arg1, arg2);
		}

		public void TraceError(int id, string formatString, params object[] args)
		{
			if (this.enabledCount > 0 && TraceWrapper.traceWriter != null)
			{
				this.RedirectTrace("Error", id, formatString, args);
			}
			ExTraceGlobals.SearchLibraryTracer.TraceError((long)id, formatString, args);
		}

		private void RedirectTrace(string traceTypePrefix, int id, string formatString, params object[] args)
		{
			string value = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
			int capacity = traceTypePrefix.Length + formatString.Length + args.Length * 10;
			StringBuilder stringBuilder = new StringBuilder(capacity);
			stringBuilder.Append(traceTypePrefix);
			stringBuilder.Append(", ");
			stringBuilder.Append(value);
			stringBuilder.Append(", ");
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, formatString, args);
			TraceWrapper.traceWriter.Write(stringBuilder.ToString());
		}

		private const string DebugPrefix = "Debug";

		private const string ErrorPrefix = "Error";

		private static readonly TraceWrapper searchLibraryTracer = new TraceWrapper();

		[ThreadStatic]
		private static TraceWrapper.ITraceWriter traceWriter;

		private int enabledCount;

		internal interface ITraceWriter
		{
			void Write(string message);
		}
	}
}
