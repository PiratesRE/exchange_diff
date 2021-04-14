using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal sealed class GlobalAddressListSchema : AddressListBaseSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<AddressBookBaseSchema>();
		}

		public static readonly ADPropertyDefinition IsDefaultGlobalAddressList = AddressBookBaseSchema.IsDefaultGlobalAddressList;

		public new static readonly ADPropertyDefinition Name = AddressListBaseSchema.Name;
	}
}
