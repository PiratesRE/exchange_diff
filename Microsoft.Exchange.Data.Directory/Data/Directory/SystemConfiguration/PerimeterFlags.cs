using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum PerimeterFlags
	{
		Empty = 0,
		SyncToHotmailEnabled = 1,
		RouteOutboundViaEhfEnabled = 2,
		IPSkiplistingEnabled = 4,
		[Obsolete("This flag is deprecated. Use SafelistingUIMode flag instead.")]
		LinkToEhfEnabled = 8,
		EhfConfigSyncDisabled = 16,
		EhfAdminAccountSyncDisabled = 32,
		IPSafelistingSyncEnabled = 64,
		RouteOutboundViaFfoFrontendEnabled = 512,
		MigrationInProgress = 1024,
		EheEnabled = 2048,
		RMSOFwdSyncDisabled = 4096,
		EheDecryptEnabled = 8192,
		All = 15988
	}
}
