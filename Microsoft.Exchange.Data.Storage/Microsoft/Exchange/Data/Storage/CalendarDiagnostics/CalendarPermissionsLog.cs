using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarPermissionsLog : QuickLog
	{
		public CalendarPermissionsLog() : base(200)
		{
		}

		public static void LogEntry(MailboxSession session, string entry, params object[] args)
		{
			CalendarPermissionsLog.instance.AppendFormatLogEntry(session, entry, args);
		}

		public static void LogEntry(MailboxSession session, Exception e, bool logWatsonReport, string entry, params object[] args)
		{
			CalendarPermissionsLog.instance.AppendFormatLogEntry(session, e, logWatsonReport, entry, args);
		}

		protected override string LogMessageClass
		{
			get
			{
				return "IPM.Microsoft.CalendarPermissions.Log";
			}
		}

		public const string MessageClass = "IPM.Microsoft.CalendarPermissions.Log";

		private const int MaxLogEntries = 200;

		private static CalendarPermissionsLog instance = new CalendarPermissionsLog();
	}
}
