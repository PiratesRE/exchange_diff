using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UserAddressBatch : ConfigurablePropertyBag
	{
		public UserAddressBatch(Guid organizationalUnitRoot)
		{
			this[UserAddressBatchSchema.OrganizationalUnitRootProperty] = organizationalUnitRoot;
			this[UserAddressBatchSchema.BatchAddressesProperty] = this.batchAddresses;
		}

		public override ObjectId Identity
		{
			get
			{
				return new MessageTraceObjectId((Guid)this[UserAddressBatchSchema.OrganizationalUnitRootProperty], Guid.Empty);
			}
		}

		public int FssCopyId
		{
			get
			{
				return (int)this[UserAddressBatchSchema.FssCopyIdProp];
			}
			set
			{
				this[UserAddressBatchSchema.FssCopyIdProp] = value;
			}
		}

		public void Add(UserAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			this.batchAddresses.AddPropertyValue(address.UserAddressId, UserAddressSchema.EmailDomainProperty, address.EmailDomain);
			this.batchAddresses.AddPropertyValue(address.UserAddressId, UserAddressSchema.EmailPrefixProperty, address.EmailPrefix);
			if (address.DigestFrequency != null)
			{
				this.batchAddresses.AddPropertyValue(address.UserAddressId, UserAddressSchema.DigestFrequencyProperty, address.DigestFrequency);
			}
			if (address.LastNotified != null)
			{
				this.batchAddresses.AddPropertyValue(address.UserAddressId, UserAddressSchema.LastNotifiedProperty, address.LastNotified);
			}
			if (address.BlockEsn != null)
			{
				this.batchAddresses.AddPropertyValue(address.UserAddressId, UserAddressSchema.BlockEsnProperty, address.BlockEsn);
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(UserAddressBatchSchema);
		}

		private BatchPropertyTable batchAddresses = new BatchPropertyTable();
	}
}
