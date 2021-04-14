using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal sealed class SystemAddressListSchema : AddressListBaseSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<AddressBookBaseSchema>();
		}

		public new static readonly ADPropertyDefinition Name = AddressListBaseSchema.Name;
	}
}
