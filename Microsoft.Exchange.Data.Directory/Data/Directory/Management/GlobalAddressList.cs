using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public sealed class GlobalAddressList : AddressListBase
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return GlobalAddressList.schema;
			}
		}

		public GlobalAddressList()
		{
		}

		public GlobalAddressList(AddressBookBase dataObject) : base(dataObject)
		{
		}

		internal static GlobalAddressList FromDataObject(AddressBookBase dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new GlobalAddressList(dataObject);
		}

		public bool IsDefaultGlobalAddressList
		{
			get
			{
				return (bool)this[GlobalAddressListSchema.IsDefaultGlobalAddressList];
			}
		}

		private static GlobalAddressListSchema schema = ObjectSchema.GetInstance<GlobalAddressListSchema>();

		public static readonly QueryFilter RecipientFilterForDefaultGal = new AndFilter(new QueryFilter[]
		{
			new ExistsFilter(ADRecipientSchema.Alias),
			new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "user"),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "contact"),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "msExchSystemMailbox"),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "msExchDynamicDistributionList"),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "group"),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "publicFolder")
			})
		});

		public static readonly ADObjectId RdnGalContainerToOrganization = new ADObjectId("CN=All Global Address Lists,CN=Address Lists Container");
	}
}
