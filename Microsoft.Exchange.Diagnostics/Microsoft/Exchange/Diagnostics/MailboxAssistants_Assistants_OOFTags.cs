using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MailboxAssistants_Assistants_OOFTags
	{
		public const int MailboxData = 0;

		public const int OofSettingsHandler = 1;

		public const int LegacyOofReplyTemplateHandler = 2;

		public const int LegacyOofStateHandler = 3;

		public const int OofScheduler = 4;

		public const int OofScheduledAction = 5;

		public const int PFD = 6;

		public const int OofScheduleStore = 7;

		public static Guid guid = new Guid("EF72E07E-3E86-4250-9083-C18CD673631D");
	}
}
