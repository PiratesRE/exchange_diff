using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct TraceEntry
	{
		public TraceEntry(TraceType traceType, Guid componentGuid, int traceTag, long id, string formatString, int startIndex, int nativeThreadId)
		{
			this.traceType = traceType;
			this.traceTag = traceTag;
			this.componentGuid = componentGuid;
			this.formatString = formatString;
			this.startIndex = startIndex;
			this.id = id;
			this.length = 0;
			this.timeStamp = DateTime.UtcNow.Ticks;
			this.nativeThreadId = nativeThreadId;
		}

		public Guid ComponentGuid
		{
			get
			{
				return this.componentGuid;
			}
		}

		public string FormatString
		{
			get
			{
				return this.formatString;
			}
			internal set
			{
				this.formatString = value;
			}
		}

		public int StartIndex
		{
			get
			{
				return this.startIndex;
			}
			set
			{
				this.startIndex = value;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
			set
			{
				this.length = value;
			}
		}

		public DateTime Timestamp
		{
			get
			{
				return new DateTime(this.timeStamp, DateTimeKind.Utc);
			}
		}

		public TraceType TraceType
		{
			get
			{
				return this.traceType;
			}
		}

		public int TraceTag
		{
			get
			{
				return this.traceTag;
			}
		}

		public long Id
		{
			get
			{
				return this.id;
			}
		}

		public int NativeThreadId
		{
			get
			{
				return this.nativeThreadId;
			}
		}

		public void Clear()
		{
			this.formatString = null;
			this.startIndex = 0;
			this.length = 0;
		}

		private string formatString;

		private int startIndex;

		private int length;

		private long timeStamp;

		private Guid componentGuid;

		private TraceType traceType;

		private int traceTag;

		private long id;

		private int nativeThreadId;
	}
}
