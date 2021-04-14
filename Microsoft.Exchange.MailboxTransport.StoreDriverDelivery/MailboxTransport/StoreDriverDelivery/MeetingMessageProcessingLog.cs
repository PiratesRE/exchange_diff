using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class MeetingMessageProcessingLog : QuickLog
	{
		public static void LogEntry(MailboxSession session, string entry, params object[] args)
		{
			MeetingMessageProcessingLog.instance.AppendFormatLogEntry(session, entry, args);
		}

		public static void LogEntry(MailboxSession session, Exception e, bool logWatsonReport, string entry, params object[] args)
		{
			MeetingMessageProcessingLog.instance.AppendFormatLogEntry(session, e, logWatsonReport, entry, args);
		}

		protected override string LogMessageClass
		{
			get
			{
				return "IPM.Microsoft.MeetingMessageProcessingAgent.Log";
			}
		}

		public const string MessageClass = "IPM.Microsoft.MeetingMessageProcessingAgent.Log";

		private static MeetingMessageProcessingLog instance = new MeetingMessageProcessingLog();
	}
}
