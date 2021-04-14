using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal sealed class AddressListSchema : AddressListBaseSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<AddressBookBaseSchema>();
		}

		public static readonly ADPropertyDefinition Container = AddressBookBaseSchema.Container;

		public static readonly ADPropertyDefinition Path = AddressBookBaseSchema.Path;

		public static readonly ADPropertyDefinition DisplayName = AddressBookBaseSchema.DisplayName;

		public new static readonly ADPropertyDefinition Name = AddressListBaseSchema.Name;
	}
}
