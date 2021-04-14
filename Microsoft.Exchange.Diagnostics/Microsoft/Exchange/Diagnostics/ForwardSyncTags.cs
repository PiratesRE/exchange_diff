using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ForwardSyncTags
	{
		public const int ForwardSyncService = 0;

		public const int FaultInjection = 1;

		public const int MainStream = 2;

		public const int FullSyncStream = 3;

		public const int MsoSyncService = 4;

		public const int PowerShell = 5;

		public const int JobProcessor = 6;

		public const int RecipientWorkflow = 7;

		public const int OrganizationWorkflow = 8;

		public const int ProvisioningLicense = 9;

		public const int UnifiedGroup = 10;

		public static Guid guid = new Guid("8FAC856B-D0D4-4f7d-BBE9-B713EDFCBAAD");
	}
}
