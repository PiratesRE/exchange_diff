using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MailboxAssistants_Assistants_ELCTags
	{
		public const int ELCAssistant = 0;

		public const int FolderProvisioner = 1;

		public const int CommonEnforcerOperations = 2;

		public const int ExpirationEnforcer = 3;

		public const int AutoCopyEnforcer = 4;

		public const int PFD = 5;

		public const int TagProvisioner = 6;

		public const int CommonTagEnforcerOperations = 7;

		public const int ExpirationTagEnforcer = 8;

		public const int AutocopyTagEnforcer = 9;

		public const int EventBasedAssistant = 10;

		public const int DeliveryAgent = 11;

		public const int TagExpirationExecutor = 12;

		public const int CommonCleanupEnforcerOperations = 13;

		public const int DumpsterExpirationEnforcer = 14;

		public const int AuditExpirationEnforcer = 15;

		public const int CalendarLogExpirationEnforcer = 16;

		public const int DumpsterQuotaEnforcer = 17;

		public const int SupplementExpirationEnforcer = 18;

		public const int DiscoveryHoldEnforcer = 19;

		public const int ElcReporting = 20;

		public const int HoldCleanupEnforcer = 21;

		public const int EHAHiddenFolderCleanupEnforcer = 22;

		public static Guid guid = new Guid("75989588-FD78-490c-B0DC-EC9E6F6E148B");
	}
}
