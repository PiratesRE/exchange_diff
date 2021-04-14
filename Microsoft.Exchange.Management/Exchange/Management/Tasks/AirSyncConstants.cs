using System;

namespace Microsoft.Exchange.Management.Tasks
{
	internal sealed class AirSyncConstants
	{
		private AirSyncConstants()
		{
		}

		public const string AirSyncBackendVDirRelativePath = "ClientAccess\\sync";

		public const string AirSyncFrontendVDirRelativePath = "FrontEnd\\HttpProxy\\sync";

		public const string AirSyncVDirName = "Microsoft-Server-ActiveSync";

		public const string AirSyncAppPoolName = "MSExchangeSyncAppPool";

		public const string AirSyncFilterName = "Microsoft Exchange Server ActiveSync Filter";

		public const string AirSync = "AirSync";

		public const string AirSyncAssemblyRelativePath = "ClientAccess\\sync\\bin\\Microsoft.Exchange.AirSync.dll";

		public const string GetActiveSyncDeviceCmdLetName = "Get-ActiveSyncDevice";

		public const string GetMobileDeviceCmdLetName = "Get-MobileDevice";

		public const string GetActiveSyncDeviceStatisticsCmdLetName = "Get-ActiveSyncDeviceStatistics";

		public const string GetMobileDeviceStatisticsCmdLetName = "Get-MobileDeviceStatistics";

		public const string RemoveActiveSyncDeviceCmdLetName = "Remove-ActiveSyncDevice";

		public const string RemoveMobileDeviceCmdLetName = "Remove-MobileDevice";

		public const string ClearActiveSyncDeviceCmdLetName = "Clear-ActiveSyncDevice";

		public const string ClearMobileDeviceCmdLetName = "Clear-MobileDevice";

		public const string GetActiveSyncMailboxPolicyCmdLetName = "Get-ActiveSyncMailboxPolicy";

		public const string GetMobileDeviceMailboxPolicyCmdLetName = "Get-MobileDeviceMailboxPolicy";

		public const string SetActiveSyncMailboxPolicyCmdLetName = "Set-ActiveSyncMailboxPolicy";

		public const string SetMobileDeviceMailboxPolicyCmdLetName = "Set-MobileDeviceMailboxPolicy";

		public const string NewActiveSyncMailboxPolicyCmdLetName = "New-ActiveSyncMailboxPolicy";

		public const string NewMobileDeviceMailboxPolicyCmdLetName = "New-MobileDeviceMailboxPolicy";

		public const string RemoveActiveSyncMailboxPolicyCmdLetName = "Remove-ActiveSyncMailboxPolicy";

		public const string RemoveMobileDeviceMailboxPolicyCmdLetName = "Remove-MobileMailboxPolicy";
	}
}
