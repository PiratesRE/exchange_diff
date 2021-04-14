using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	internal class SharingLog : QuickLog
	{
		public static void LogEntry(MailboxSession session, string entry)
		{
			string entry2 = string.Format("{0}, Mailbox: '{1}', Entry {2}.", DateTime.UtcNow.ToString(), session.MailboxOwner.LegacyDn, entry);
			SharingLog.instance.WriteLogEntry(session, entry2);
		}

		protected override string LogMessageClass
		{
			get
			{
				return "IPM.Microsoft.Sharing.Log";
			}
		}

		public const string MessageClass = "IPM.Microsoft.Sharing.Log";

		private static SharingLog instance = new SharingLog();
	}
}
