using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OwaServerTraceLogger : ExtensibleLogger
	{
		public OwaServerTraceLogger() : base(new OwaServerTraceLogConfiguration())
		{
			ActivityContext.RegisterMetadata(typeof(OwaServerLogger.LoggerData));
		}

		public static void Initialize()
		{
			if (OwaServerTraceLogger.instance == null)
			{
				OwaServerTraceLogger.instance = new OwaServerTraceLogger();
			}
		}

		public static void AppendToLog(ILogEvent logEvent)
		{
			if (OwaServerTraceLogger.instance != null)
			{
				OwaServerTraceLogger.instance.LogEvent(logEvent);
			}
		}

		public static void SaveTraces()
		{
			Globals.TroubleshootingContext.TraceOperationCompletedAndUpdateContext();
			RequestDetailsLogger requestDetailsLogger = RequestDetailsLogger.Current;
			if (requestDetailsLogger != null && OwaServerTraceLogger.IsInterestingFailure(requestDetailsLogger.ActivityScope) && Globals.LogErrorTraces)
			{
				OwaServerTraceLogger.InternalSaveTraces(requestDetailsLogger.ActivityScope, Globals.TroubleshootingContext);
			}
		}

		public static bool IsOverPerfThreshold(double totalMilliSeconds)
		{
			return totalMilliSeconds > (OwaServerTraceLogger.instance.Configuration as OwaServerTraceLogConfiguration).OwaTraceLoggingThreshold;
		}

		private static bool IsInterestingFailure(IActivityScope activityScope)
		{
			bool flag = OwaServerTraceLogger.IsOverPerfThreshold(activityScope.TotalMilliseconds);
			string property = activityScope.GetProperty(ServiceCommonMetadata.ErrorCode);
			string property2 = activityScope.GetProperty(ServiceCommonMetadata.GenericErrors);
			if (string.IsNullOrEmpty(property) && string.IsNullOrEmpty(property2))
			{
				return flag;
			}
			return property == null || !property.Contains("NotFound") || flag;
		}

		private static void InternalSaveTraces(IActivityScope activityScope, TroubleshootingContext troubleshootingContext)
		{
			IEnumerable<TraceEntry> traces = troubleshootingContext.GetTraces();
			string eventId = activityScope.GetProperty(ExtensibleLoggerMetadata.EventId) + "_Trace";
			foreach (TraceEntry entry in traces)
			{
				OwaServerTraceLogger.TraceLogEvent logEvent = new OwaServerTraceLogger.TraceLogEvent(activityScope, entry, eventId);
				OwaServerTraceLogger.instance.LogEvent(logEvent);
			}
		}

		private static OwaServerTraceLogger instance;

		private class TraceLogEvent : ILogEvent
		{
			public TraceLogEvent(IActivityScope activityScope, TraceEntry entry, string eventId)
			{
				this.activityScope = activityScope;
				this.entry = entry;
				this.eventId = eventId;
			}

			public string EventId
			{
				get
				{
					return this.eventId;
				}
			}

			public virtual ICollection<KeyValuePair<string, object>> GetEventData()
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				ExtensibleLogger.CopyProperty(this.activityScope, dictionary, OwaServerLogger.LoggerData.UserContext, UserContextCookie.UserContextCookiePrefix);
				ExtensibleLogger.CopyPIIProperty(this.activityScope, dictionary, OwaServerLogger.LoggerData.PrimarySmtpAddress, "PSA");
				dictionary.Add("ActivityID", this.activityScope.ActivityId.ToString());
				dictionary.Add("Component", this.entry.ComponentGuid.ToString());
				dictionary.Add("Type", this.entry.TraceType.ToString());
				dictionary.Add("Tag", this.entry.TraceTag.ToString());
				dictionary.Add("Time", this.entry.Timestamp);
				dictionary.Add("NativeThreadId", this.entry.NativeThreadId);
				dictionary.Add("Trace", this.entry.FormatString);
				return dictionary;
			}

			private readonly IActivityScope activityScope;

			private readonly TraceEntry entry;

			private readonly string eventId;
		}
	}
}
