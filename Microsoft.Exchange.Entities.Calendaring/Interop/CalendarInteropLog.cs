using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.Interop
{
	internal class CalendarInteropLog : QuickLog, ICalendarInteropLog
	{
		public static CalendarInteropLog Default
		{
			get
			{
				return CalendarInteropLog.DefaultInstance;
			}
		}

		protected override string LogMessageClass
		{
			get
			{
				return "IPM.Microsoft.CalendarInterop.Log";
			}
		}

		public void LogEntry(IStoreSession session, string entry, params object[] args)
		{
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession != null)
			{
				this.LogMessageEntry(mailboxSession, entry, args);
			}
		}

		public void LogEntry(IStoreSession session, Exception e, bool logWatsonReport, string entry, params object[] args)
		{
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession != null)
			{
				this.LogExceptionEntry(mailboxSession, e, logWatsonReport, entry, args);
			}
		}

		public void SafeLogEntry(IStoreSession session, Exception exceptionToReport, bool logWatsonReport, string entry, params object[] args)
		{
			try
			{
				this.LogEntry(session, exceptionToReport, logWatsonReport, entry, args);
			}
			catch (Exception arg)
			{
				ExTraceGlobals.CalendarInteropTracer.TraceError<Exception>(0L, "Error writing CalendarInteropLog: {0}", arg);
			}
		}

		protected virtual void LogMessageEntry(MailboxSession mailboxSession, string entry, params object[] args)
		{
			base.AppendFormatLogEntry(mailboxSession, entry, args);
		}

		protected virtual void LogExceptionEntry(MailboxSession mailboxSession, Exception e, bool logWatsonReport, string entry, params object[] args)
		{
			base.AppendFormatLogEntry(mailboxSession, e, logWatsonReport, entry, args);
		}

		public const string MessageClass = "IPM.Microsoft.CalendarInterop.Log";

		private static readonly CalendarInteropLog DefaultInstance = new CalendarInteropLog();
	}
}
