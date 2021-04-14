using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal class MfnLog : QuickLog
	{
		protected override string LogMessageClass
		{
			get
			{
				return "IPM.Microsoft.MFN.Log";
			}
		}

		public static void LogEntry(MailboxSession session, string info)
		{
			MfnLog.Logger.WriteLogEntry(session, string.Format("{0} : {1}", DateTime.UtcNow, info));
		}

		private static MfnLog Logger = new MfnLog();
	}
}
