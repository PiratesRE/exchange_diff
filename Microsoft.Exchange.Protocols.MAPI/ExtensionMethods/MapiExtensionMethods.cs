using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI.ExtensionMethods
{
	public static class MapiExtensionMethods
	{
		public static byte[] UserSearchKey(this AddressInfo addressInfo)
		{
			return AddressBookEID.MakeSearchKey("EX", addressInfo.LegacyExchangeDN);
		}

		public static byte[] UserEntryId(this AddressInfo addressInfo)
		{
			return AddressBookEID.MakeAddressBookEntryID(addressInfo.LegacyExchangeDN, addressInfo.IsDistributionList);
		}

		public static int UserFlags(this AddressInfo addressInfo)
		{
			return 0;
		}
	}
}
