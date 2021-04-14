using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ManagedStore_StoreCommonServicesTags
	{
		public const int Context = 0;

		public const int Mailbox = 1;

		public const int ExtendedProps = 2;

		public const int QueryPlannerSummary = 3;

		public const int QueryPlannerDetail = 4;

		public const int SecurityMailboxOwnerAccess = 5;

		public const int SecurityAdminAccess = 6;

		public const int SecurityServiceAccess = 7;

		public const int SecuritySendAsAccess = 8;

		public const int SecurityContext = 9;

		public const int SecurityDescriptor = 10;

		public const int FullAccountName = 11;

		public const int ExecutionDiagnostics = 12;

		public const int FullTextIndex = 13;

		public const int MailboxQuarantine = 14;

		public const int MailboxSignature = 15;

		public const int TimedEvents = 16;

		public const int Maintenance = 17;

		public const int MailboxLock = 18;

		public const int CriticalBlock = 19;

		public const int FaultInjection = 20;

		public const int Notification = 21;

		public const int StoreDatabase = 22;

		public const int MailboxCounters = 23;

		public const int Chunking = 24;

		public const int ViewTableFindRow = 30;

		public const int SchemaUpgradeService = 31;

		public static Guid guid = new Guid("15744371-ee52-4dfc-97f9-940803cf462f");
	}
}
