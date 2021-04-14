using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AnonymousSharingLog : QuickLog
	{
		public static void LogEntries(MailboxSession session, List<LocalizedString> entries)
		{
			foreach (LocalizedString value in entries)
			{
				AnonymousSharingLog.instance.AppendFormatLogEntry(session, value, new object[0]);
			}
		}

		protected override string LogMessageClass
		{
			get
			{
				return "IPM.Microsoft.AnonymousSharing.Log";
			}
		}

		private const string MessageClass = "IPM.Microsoft.AnonymousSharing.Log";

		private static AnonymousSharingLog instance = new AnonymousSharingLog();
	}
}
