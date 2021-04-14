using System;

namespace Microsoft.Exchange.Assistants.Logging
{
	internal class MailboxAssistantsDatabaseSlaLogFileSettings : MailboxAssistantsSlaReportLogFileSettings
	{
		protected override string LogSubFolderName
		{
			get
			{
				return "MailboxAssistantsDatabaseSlaLog";
			}
		}

		protected override string LogTypeName
		{
			get
			{
				return "MailboxAssistantsDatabaseSlaLog";
			}
		}

		internal const string DatabaseSlaLogName = "MailboxAssistantsDatabaseSlaLog";
	}
}
