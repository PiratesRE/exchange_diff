using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoRequestLogWriter : ITraceEntryWriter
	{
		public PhotoRequestLogWriter(PhotoRequestLog log, string requestId)
		{
			ArgumentValidator.ThrowIfNull("log", log);
			ArgumentValidator.ThrowIfNullOrEmpty("requestId", requestId);
			this.log = log;
			this.requestId = requestId;
		}

		public void Write(TraceEntry entry)
		{
			switch (entry.TraceType)
			{
			case TraceType.WarningTrace:
				this.Log(entry, "WARNING");
				return;
			case TraceType.ErrorTrace:
				this.Log(entry, "ERROR");
				return;
			case TraceType.PerformanceTrace:
				this.Log(entry, "PERFORMANCE");
				return;
			}
			this.Log(entry, "DEBUG");
		}

		private void Log(TraceEntry entry, string eventType)
		{
			this.log.Log(entry.Timestamp, this.requestId, eventType, entry.FormatString);
		}

		private readonly PhotoRequestLog log;

		private readonly string requestId;

		private static class EventTypes
		{
			public const string Debug = "DEBUG";

			public const string Error = "ERROR";

			public const string Warning = "WARNING";

			public const string Performance = "PERFORMANCE";
		}
	}
}
