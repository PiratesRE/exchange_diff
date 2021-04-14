using System;
using System.IO;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class InMemoryTracer : ITracer
	{
		public InMemoryTracer(Guid component, int tag) : this(component, tag, 1024, 16384)
		{
		}

		public InMemoryTracer(Guid component, int tag, int maxEntries, int maxBufferSize)
		{
			this.component = component;
			this.tag = tag;
			this.builder = new MemoryTraceBuilder(maxEntries, maxBufferSize);
		}

		public void TraceDebug<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.AppendFormat<T0, T1, T2>(id, TraceType.DebugTrace, formatString, arg0, arg1, arg2);
		}

		public void TraceDebug<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.AppendFormat<T0, T1>(id, TraceType.DebugTrace, formatString, arg0, arg1);
		}

		public void TraceDebug<T0>(long id, string formatString, T0 arg0)
		{
			this.AppendFormat<T0>(id, TraceType.DebugTrace, formatString, arg0);
		}

		public void TraceDebug(long id, string message)
		{
			this.Append(id, TraceType.DebugTrace, message);
		}

		public void TraceDebug(long id, string formatString, params object[] args)
		{
			this.AppendFormat(id, TraceType.DebugTrace, formatString, args);
		}

		public void TraceWarning<T0>(long id, string formatString, T0 arg0)
		{
			this.AppendFormat<T0>(id, TraceType.WarningTrace, formatString, arg0);
		}

		public void TraceWarning(long id, string message)
		{
			this.Append(id, TraceType.WarningTrace, message);
		}

		public void TraceWarning(long id, string formatString, params object[] args)
		{
			this.AppendFormat(id, TraceType.WarningTrace, formatString, args);
		}

		public void TraceError<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.AppendFormat<T0, T1, T2>(id, TraceType.ErrorTrace, formatString, arg0, arg1, arg2);
		}

		public void TraceError<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.AppendFormat<T0, T1>(id, TraceType.ErrorTrace, formatString, arg0, arg1);
		}

		public void TraceError<T0>(long id, string formatString, T0 arg0)
		{
			this.AppendFormat<T0>(id, TraceType.ErrorTrace, formatString, arg0);
		}

		public void TraceError(long id, string message)
		{
			this.Append(id, TraceType.ErrorTrace, message);
		}

		public void TraceError(long id, string formatString, params object[] args)
		{
			this.AppendFormat(id, TraceType.ErrorTrace, formatString, args);
		}

		public void TracePerformance<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.AppendFormat<T0, T1, T2>(id, TraceType.PerformanceTrace, formatString, arg0, arg1, arg2);
		}

		public void TracePerformance<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			this.AppendFormat<T0, T1>(id, TraceType.PerformanceTrace, formatString, arg0, arg1);
		}

		public void TracePerformance<T0>(long id, string formatString, T0 arg0)
		{
			this.AppendFormat<T0>(id, TraceType.PerformanceTrace, formatString, arg0);
		}

		public void TracePerformance(long id, string message)
		{
			this.Append(id, TraceType.PerformanceTrace, message);
		}

		public void TracePerformance(long id, string formatString, params object[] args)
		{
			this.AppendFormat(id, TraceType.PerformanceTrace, formatString, args);
		}

		public void Dump(TextWriter writer, bool addHeader, bool verbose)
		{
			this.builder.Dump(writer, addHeader, verbose);
		}

		public void Dump(ITraceEntryWriter writer)
		{
			ArgumentValidator.ThrowIfNull("writer", writer);
			foreach (TraceEntry entry in this.builder.GetTraces())
			{
				writer.Write(entry);
			}
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
			return true;
		}

		private void Append(long id, TraceType type, string message)
		{
			this.builder.BeginEntry(type, this.component, this.tag, id, message);
			this.builder.EndEntry();
		}

		private void AppendFormat(long id, TraceType type, string formatString, params object[] args)
		{
			if (args == null)
			{
				return;
			}
			this.builder.BeginEntry(type, this.component, this.tag, id, formatString);
			for (int i = 0; i < args.Length; i++)
			{
				this.builder.AddArgument<object>(args[i]);
			}
			this.builder.EndEntry();
		}

		private void AppendFormat<T0>(long id, TraceType type, string formatString, T0 arg0)
		{
			this.builder.BeginEntry(type, this.component, this.tag, id, formatString);
			this.builder.AddArgument<T0>(arg0);
			this.builder.EndEntry();
		}

		private void AppendFormat<T0, T1>(long id, TraceType type, string formatString, T0 arg0, T1 arg1)
		{
			this.builder.BeginEntry(type, this.component, this.tag, id, formatString);
			this.builder.AddArgument<T0>(arg0);
			this.builder.AddArgument<T1>(arg1);
			this.builder.EndEntry();
		}

		private void AppendFormat<T0, T1, T2>(long id, TraceType type, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			this.builder.BeginEntry(type, this.component, this.tag, id, formatString);
			this.builder.AddArgument<T0>(arg0);
			this.builder.AddArgument<T1>(arg1);
			this.builder.AddArgument<T2>(arg2);
			this.builder.EndEntry();
		}

		private const int DefaultMaxEntries = 1024;

		private const int DefaultMaxBufferSize = 16384;

		private readonly Guid component;

		private readonly int tag;

		private readonly MemoryTraceBuilder builder;
	}
}
