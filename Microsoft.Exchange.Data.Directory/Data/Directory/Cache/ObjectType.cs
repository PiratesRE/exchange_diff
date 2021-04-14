using System;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	[Flags]
	internal enum ObjectType
	{
		Unknown = 0,
		ExchangeConfigurationUnit = 1,
		Recipient = 2,
		AcceptedDomain = 4,
		FederatedOrganizationId = 8,
		MiniRecipient = 16,
		TransportMiniRecipient = 32,
		OWAMiniRecipient = 64,
		ActiveSyncMiniRecipient = 128,
		ADRawEntry = 256,
		StorageMiniRecipient = 512,
		LoadBalancingMiniRecipient = 1024,
		MiniRecipientWithTokenGroups = 2048,
		FrontEndMiniRecipient = 4096
	}
}
