using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ServiceType
	{
		Invalid,
		WebServices,
		OutlookWebAccess,
		MobileSync,
		RpcHttp,
		OfflineAddressBook,
		AvailabilityForeignConnector,
		ExchangeControlPanel,
		UnifiedMessaging,
		Pop3,
		Imap4,
		Smtp,
		MapiHttp
	}
}
