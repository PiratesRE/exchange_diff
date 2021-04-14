using System;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager
{
	public enum AddressListType
	{
		[LocDescription(Strings.IDs.GlobalAddressListTitle)]
		GlobalAddressList,
		[LocDescription(Strings.IDs.AddressListTitle)]
		AddressList
	}
}
