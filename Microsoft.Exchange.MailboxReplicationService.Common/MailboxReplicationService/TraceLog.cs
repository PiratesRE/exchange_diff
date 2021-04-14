using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class TraceLog : ObjectLog<TraceLogData>
	{
		private TraceLog() : base(new TraceLog.TraceLogSchema(), new SimpleObjectLogConfiguration("Trace", null, "TraceLogMaxDirSize", "TraceLogMaxFileSize"))
		{
		}

		public static void Write(string tracerName, TraceType traceType, string traceMessage)
		{
			TraceLogData objectToLog = default(TraceLogData);
			objectToLog.TracerName = tracerName;
			objectToLog.TraceType = traceType;
			objectToLog.TraceMessage = traceMessage;
			TraceLog.instance.LogObject(objectToLog);
		}

		private const int MaxDataContextLength = 1000;

		private static TraceLog instance = new TraceLog();

		private class TraceLogSchema : ObjectLogSchema
		{
			public override string Software
			{
				get
				{
					return "Microsoft Exchange Mailbox Replication Service";
				}
			}

			public override string LogType
			{
				get
				{
					return "Trace Log";
				}
			}

			public static readonly ObjectLogSimplePropertyDefinition<TraceLogData> TracerName = new ObjectLogSimplePropertyDefinition<TraceLogData>("Tracer", (TraceLogData d) => d.TracerName);

			public static readonly ObjectLogSimplePropertyDefinition<TraceLogData> TraceType = new ObjectLogSimplePropertyDefinition<TraceLogData>("Type", (TraceLogData d) => d.TraceType.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<TraceLogData> TraceMessage = new ObjectLogSimplePropertyDefinition<TraceLogData>("Message", (TraceLogData d) => d.TraceMessage);
		}
	}
}
