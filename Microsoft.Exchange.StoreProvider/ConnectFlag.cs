using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum ConnectFlag
	{
		None = 0,
		UseAdminPrivilege = 1,
		NoRpcEncryption = 2,
		UseSeparateConnection = 4,
		NoUnderCoverConnection = 8,
		AnonymousAccess = 16,
		NoNotifications = 32,
		NoTableNotifications = 32,
		NoAddressResolution = 64,
		RestoreDatabase = 128,
		UseDelegatedAuthPrivilege = 256,
		UseLegacyUdpNotifications = 512,
		UseTransportPrivilege = 1024,
		UseReadOnlyPrivilege = 2048,
		UseReadWritePrivilege = 4096,
		LocalRpcOnly = 8192,
		LowMemoryFootprint = 16384,
		UseHTTPS = 65536,
		UseNTLM = 131072,
		UseRpcUniqueBinding = 262144,
		ConnectToExchangeRpcServerOnly = 524288,
		UseRpcContextPool = 1048576,
		UseResiliency = 2097152,
		RemoteSystemService = 4194304,
		PublicFolderMigration = 8388608,
		IsPreExchange15 = 16777216,
		AllowLegacyStore = 33554432,
		MonitoringMailbox = 67108864,
		UseAnonymousInnerAuth = 134217728,
		IgnoreServerCertificate = 268435456
	}
}
