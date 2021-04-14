using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	internal class InternetCalendarLog : QuickLog
	{
		private InternetCalendarLog() : base(100)
		{
		}

		public static void LogEntry(MailboxSession session, string entry)
		{
			string entry2 = string.Format("{0}, Mailbox: '{1}', Entry {2}.", DateTime.UtcNow.ToString(), session.DisplayName, entry);
			InternetCalendarLog.instance.WriteLogEntry(session, entry2);
		}

		public static void LogEntry(MailboxSession session, PublishingSubscriptionData subscriptionData, HttpWebRequest request, List<LocalizedString> errors)
		{
			StringBuilder stringBuilder = new StringBuilder(InternetCalendarLog.MaxErrorBufferSize);
			if (errors != null && errors.Count > 0)
			{
				string arg;
				if (request != null && request.Proxy is WebProxy)
				{
					arg = ((WebProxy)request.Proxy).Address.ToString();
				}
				else
				{
					arg = "<unknown>";
				}
				stringBuilder.AppendFormat("Errors while synchronizing calendar. Subscription data: {0}, proxy is: {1}. ", subscriptionData.ToString(), arg);
				foreach (LocalizedString value in errors)
				{
					if (stringBuilder.Length > InternetCalendarLog.MaxErrorBufferSize)
					{
						InternetCalendarLog.instance.WriteLogEntry(session, stringBuilder.ToString());
						stringBuilder.Length = 0;
					}
					stringBuilder.Append(value);
					stringBuilder.Append(';');
				}
				if (stringBuilder.Length > 0)
				{
					InternetCalendarLog.LogEntry(session, stringBuilder.ToString());
				}
			}
		}

		protected override string LogMessageClass
		{
			get
			{
				return "IPM.Microsoft.InternetCalendar.Log";
			}
		}

		private const int DefaultMaxLogEntries = 100;

		private const string UnknownProxy = "<unknown>";

		public const string MessageClass = "IPM.Microsoft.InternetCalendar.Log";

		protected static readonly int MaxErrorBufferSize = 1024;

		private static InternetCalendarLog instance = new InternetCalendarLog();
	}
}
