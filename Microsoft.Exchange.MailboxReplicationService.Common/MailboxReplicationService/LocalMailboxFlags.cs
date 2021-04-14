using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum LocalMailboxFlags
	{
		None = 0,
		StripLargeRulesForDownlevelTargets = 1,
		UseHomeMDB = 2,
		PureMAPI = 4,
		CredentialIsNotAdmin = 8,
		UseNTLMAuth = 16,
		ConnectToMoMT = 32,
		LegacyPublicFolders = 64,
		Restore = 128,
		LocalMachineMapiOnly = 256,
		UseMapiProvider = 512,
		Move = 1024,
		PublicFolderMove = 2048,
		WordBreak = 4096,
		PstImport = 8192,
		EasSync = 16384,
		AggregatedMailbox = 32768,
		CreateNewPartition = 65536,
		InvalidateContentIndexAnnotations = 131072,
		FolderMove = 262144,
		OlcSync = 524288,
		PstExport = 1048576,
		ParallelPublicFolderMigration = 4194304
	}
}
