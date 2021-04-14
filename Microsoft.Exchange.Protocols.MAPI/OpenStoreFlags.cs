using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	[Flags]
	public enum OpenStoreFlags : uint
	{
		None = 0U,
		UseAdminPrivilege = 1U,
		Public = 2U,
		HomeLogon = 4U,
		TakeOwnership = 8U,
		OverrideHomeMdb = 16U,
		Transport = 32U,
		RemoteTransport = 64U,
		InternetAnonymous = 128U,
		AlternateServer = 256U,
		IgnoreHomeMdb = 512U,
		NoMail = 1024U,
		OverrideLastModifier = 2048U,
		CallbackLogon = 4096U,
		Local = 8192U,
		FailIfNoMailbox = 16384U,
		CacheExchange = 32768U,
		CliWithNamedpropFix = 65536U,
		EnableLazyLogging = 131072U,
		CliWithReplidGuidMappingFix = 262144U,
		NoLocalization = 524288U,
		RestoreDatabase = 1048576U,
		XforestMove = 2097152U,
		MailboxGuid = 4194304U,
		ExchangeTransport = 8388608U,
		UsePerMdbReplidMapping = 16777216U,
		DeliverNormalMessage = 33554432U,
		DeliverSpecialMessage = 67108864U,
		DeliverQuotaMessage = 134217728U,
		NoExtendedFlags = 268435456U,
		SupportProgress = 536870912U,
		DisconnectedMailbox = 1073741824U,
		ApplicationIdOnly = 2147483648U
	}
}
