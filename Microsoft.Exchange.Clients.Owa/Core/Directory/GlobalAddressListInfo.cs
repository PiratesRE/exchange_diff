using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Core.Directory
{
	public class GlobalAddressListInfo : AddressListInfo
	{
		internal GlobalAddressListInfo(AddressBookBase addressList, GlobalAddressListInfo.GalOrigin origin) : base(addressList)
		{
			this.origin = origin;
		}

		public GlobalAddressListInfo.GalOrigin Origin
		{
			get
			{
				return this.origin;
			}
		}

		public override AddressBookBase ToAddressBookBase()
		{
			if (this.origin == GlobalAddressListInfo.GalOrigin.EmptyGlobalAddressList)
			{
				return new AddressBookBase
				{
					OrganizationId = base.OrganizationId
				};
			}
			return base.ToAddressBookBase();
		}

		private GlobalAddressListInfo.GalOrigin origin;

		public enum GalOrigin
		{
			DefaultGlobalAddressList,
			QueryBaseDNAddressList,
			QueryBaseDNSubTree,
			EmptyGlobalAddressList
		}
	}
}
