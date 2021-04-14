using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class AddressListInfo
	{
		private AddressListInfo(string displayName, ADObjectId id, OrganizationId organizationId)
		{
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			this.displayName = displayName;
			this.id = id;
			this.organizationId = organizationId;
		}

		internal AddressListInfo(AddressBookBase addressList)
		{
			if (addressList == null)
			{
				throw new ArgumentNullException("addressList");
			}
			this.DisplayName = addressList.DisplayName;
			this.Id = addressList.Id;
			this.organizationId = addressList.OrganizationId;
		}

		internal static AddressListInfo CreateEmpty(OrganizationId organizationId)
		{
			return new AddressListInfo(null, null, organizationId);
		}

		internal bool IsEmpty
		{
			get
			{
				return this.DisplayName == null && this.Id == null;
			}
		}

		public string ToBase64String()
		{
			if (this.Id == null)
			{
				return "0000";
			}
			return Convert.ToBase64String(this.id.ObjectGuid.ToByteArray());
		}

		public virtual AddressBookBase ToAddressBookBase()
		{
			AddressBookBase addressBookBase = new AddressBookBase();
			addressBookBase.SetId(this.Id);
			addressBookBase.DisplayName = this.DisplayName;
			addressBookBase.OrganizationId = this.OrganizationId;
			return addressBookBase;
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		public ADObjectId Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		private string displayName;

		private ADObjectId id;

		private readonly OrganizationId organizationId;
	}
}
