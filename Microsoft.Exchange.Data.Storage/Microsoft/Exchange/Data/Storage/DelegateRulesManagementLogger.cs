using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DelegateRulesManagementLogger : QuickLog
	{
		public DelegateRulesManagementLogger() : base(200)
		{
		}

		protected override string LogMessageClass
		{
			get
			{
				return "IPM.Microsoft.DelegateRulesManagement.Log";
			}
		}

		public static void LogEntry(MailboxSession session, string info)
		{
			DelegateRulesManagementLogger.Logger.WriteLogEntry(session, string.Format("Timestamp: {0}   {1} ", DateTime.UtcNow, info));
		}

		private const int MaxLogEntries = 200;

		private const string RulesManagementLoggerString = "IPM.Microsoft.DelegateRulesManagement.Log";

		private static DelegateRulesManagementLogger Logger = new DelegateRulesManagementLogger();
	}
}
