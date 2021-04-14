using System;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Exchange.Diagnostics
{
	internal sealed class ExTraceListener : TraceListener
	{
		private Trace ExchangeTracer
		{
			get
			{
				if (this.tracer == null)
				{
					lock (this)
					{
						if (this.tracer == null)
						{
							this.tracer = new Trace(SystemLoggingTags.guid, this.traceTag);
						}
					}
				}
				return this.tracer;
			}
		}

		public ExTraceListener(int traceTag, string source) : base(source)
		{
			this.line = new StringBuilder(1024);
			this.traceData = new StringBuilder(1024);
			this.traceTag = traceTag;
			base.TraceOutputOptions = TraceOptions.None;
		}

		public override void Write(string message)
		{
			if (ExTraceInternal.AreAnyTraceProvidersEnabled)
			{
				this.line.Append(message);
			}
		}

		public override void WriteLine(string message)
		{
			if (ExTraceInternal.AreAnyTraceProvidersEnabled)
			{
				this.line.Append(message);
				this.ExchangeTracer.TraceDebug<StringBuilder>((long)this.GetHashCode(), "{0}", this.line);
				this.line.Length = 0;
			}
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
		{
			if (data == null)
			{
				return;
			}
			if (base.Filter != null && !base.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data))
			{
				return;
			}
			for (int i = 0; i < data.Length; i++)
			{
				if (i != 0)
				{
					this.traceData.Append(", ");
				}
				if (data[i] != null)
				{
					this.traceData.Append(data[i].ToString());
				}
			}
			this.TraceEvent(eventCache, source, eventType, id, this.traceData.ToString());
			this.traceData.Length = 0;
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			if (data == null)
			{
				return;
			}
			this.TraceEvent(eventCache, source, eventType, id, data.ToString());
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			if (base.Filter != null && !base.Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null))
			{
				return;
			}
			long id2 = (id == 0) ? ((long)this.GetHashCode()) : ((long)id);
			switch (eventType)
			{
			case TraceEventType.Critical:
			case TraceEventType.Error:
				this.ExchangeTracer.TraceError(id2, message);
				return;
			case (TraceEventType)3:
				break;
			case TraceEventType.Warning:
				this.ExchangeTracer.TraceWarning(id2, message);
				return;
			default:
				if (eventType == TraceEventType.Information)
				{
					this.ExchangeTracer.TraceInformation(0, id2, message);
					return;
				}
				break;
			}
			this.ExchangeTracer.TraceDebug(id2, message);
		}

		private int traceTag;

		private Trace tracer;

		private StringBuilder line;

		private StringBuilder traceData;
	}
}
