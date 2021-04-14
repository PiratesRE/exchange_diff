using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum OpenStoreFlag : long
	{
		None = 0L,
		UseAdminPrivilege = 1L,
		Public = 2L,
		HomeLogon = 4L,
		TakeOwnership = 8L,
		OverrideHomeMdb = 16L,
		Transport = 32L,
		RemoteTransport = 64L,
		InternetAnonymous = 128L,
		AlternateServer = 256L,
		IgnoreHomeMdb = 512L,
		NoMail = 1024L,
		OverrideLastModifier = 2048L,
		CallbackLogon = 4096L,
		Local = 8192L,
		FailIfNoMailbox = 16384L,
		CacheExchange = 32768L,
		CliWithNamedPropFix = 65536L,
		EnableLazyLogging = 131072L,
		CliWithReplidGuidMappingFix = 262144L,
		NoLocalization = 524288L,
		RestoreDatabase = 1048576L,
		XForestMove = 2097152L,
		MailboxGuid = 4194304L,
		CliWithPerMdbFix = 16777216L,
		DeliverNormalMessage = 33554432L,
		DeliverSpecialMessage = 67108864L,
		DeliverQuotaMessage = 134217728L,
		NoExtendedFlags = 268435456L,
		SupportsProgress = 536870912L,
		DisconnectedMailbox = 1073741824L,
		ApplicationIdOnly = 2147483648L,
		ShowAllFIDCs = 4294967296L,
		PublicFolderSubsystem = 8589934592L
	}
}
