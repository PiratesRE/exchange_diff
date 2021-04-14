using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class SharingPermissionLogger : QuickLog
	{
		public SharingPermissionLogger() : base(100)
		{
		}

		protected override string LogMessageClass
		{
			get
			{
				return "IPM.Microsoft.SharingPermissionManagement.Log";
			}
		}

		public static void LogEntry(MailboxSession session, string info, string ContextIdentifier)
		{
			SharingPermissionLogger.Logger.WriteLogEntry(session, string.Format("Timestamp: {0} Logging Id: {1} : {2}", DateTime.UtcNow, ContextIdentifier, info));
		}

		private const int MaxLogEntries = 100;

		public const string SharingPermissionLoggerString = "IPM.Microsoft.SharingPermissionManagement.Log";

		private static SharingPermissionLogger Logger = new SharingPermissionLogger();
	}
}
