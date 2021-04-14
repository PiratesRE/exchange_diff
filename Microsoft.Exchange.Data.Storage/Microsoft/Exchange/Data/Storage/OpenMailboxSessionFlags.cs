using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum OpenMailboxSessionFlags
	{
		None = 0,
		RequestAdminAccess = 1,
		RequestCachedConnection = 2,
		RequestTransportAccess = 4,
		OpenForQuotaMessageDelivery = 8,
		OpenForNormalMessageDelivery = 16,
		OpenForSpecialMessageDelivery = 32,
		RequestLocalRpcConnection = 64,
		RequestExchangeRpcServer = 128,
		OverrideHomeMdb = 256,
		InitDefaultFolders = 512,
		InitUserConfigurationManager = 1024,
		InitCopyOnWrite = 2048,
		InitDeadSessionChecking = 4096,
		InitCheckPrivateItemsAccess = 8192,
		SuppressFolderIdPrefetch = 16384,
		UseNamedProperties = 32768,
		DeferDefaultFolderIdInitialization = 65536,
		UseRecoveryDatabase = 131072,
		NonInteractiveSession = 262144,
		DisconnectedMailbox = 524288,
		XForestMove = 1048576,
		MoveUser = 2097152,
		IgnoreForcedFolderInit = 4194304,
		ContentIndexing = 8388608,
		AllowAdminLocalization = 16777216,
		ReadOnly = 33554432,
		OlcSync = 67108864
	}
}
