using System;

namespace Microsoft.Exchange.Data
{
	public enum ClientAccessProtocol
	{
		[LocDescription(DataStrings.IDs.ClientAccessProtocolEWS)]
		ExchangeWebServices,
		[LocDescription(DataStrings.IDs.ClientAccessProtocolRPS)]
		RemotePowerShell,
		[LocDescription(DataStrings.IDs.ClientAccessProtocolOA)]
		OutlookAnywhere,
		[LocDescription(DataStrings.IDs.ClientAccessProtocolPOP3)]
		POP3,
		[LocDescription(DataStrings.IDs.ClientAccessProtocolIMAP4)]
		IMAP4,
		[LocDescription(DataStrings.IDs.ClientAccessProtocolOWA)]
		OutlookWebApp,
		[LocDescription(DataStrings.IDs.ClientAccessProtocolEAC)]
		ExchangeAdminCenter,
		[LocDescription(DataStrings.IDs.ClientAccessProtocolEAS)]
		ExchangeActiveSync,
		[LocDescription(DataStrings.IDs.ClientAccessProtocolOAB)]
		OfflineAddressBook,
		[LocDescription(DataStrings.IDs.ClientAccessProtocolPSWS)]
		PowerShellWebServices
	}
}
