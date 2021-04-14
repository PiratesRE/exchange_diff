using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public sealed class ExchangeConfigurationContainerWithAddressLists : ExchangeConfigurationContainer
	{
		public MultiValuedProperty<ADObjectId> AddressBookRoots
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ExchangeConfigurationContainerSchemaWithAddressLists.AddressBookRoots];
			}
		}

		public MultiValuedProperty<ADObjectId> DefaultGlobalAddressList
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ExchangeConfigurationContainerSchemaWithAddressLists.DefaultGlobalAddressList];
			}
		}

		public MultiValuedProperty<ADObjectId> AddressBookRoots2
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ExchangeConfigurationContainerSchemaWithAddressLists.AddressBookRoots2];
			}
		}

		public MultiValuedProperty<ADObjectId> DefaultGlobalAddressList2
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ExchangeConfigurationContainerSchemaWithAddressLists.DefaultGlobalAddressList2];
			}
		}

		internal AddressBookBase GetDefaultGlobalAddressList()
		{
			MultiValuedProperty<ADObjectId> defaultGlobalAddressList = this.DefaultGlobalAddressList;
			AddressBookBase result = null;
			ADObjectId adobjectId = null;
			foreach (ADObjectId adobjectId2 in defaultGlobalAddressList)
			{
				adobjectId = adobjectId2;
				if (adobjectId2.IsDescendantOf(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest()))
				{
					break;
				}
			}
			if (adobjectId != null)
			{
				result = base.Session.Read<AddressBookBase>(adobjectId);
			}
			return result;
		}

		internal bool LinkedAddressBookRootAttributesPresent()
		{
			ADSchemaAttributeObject[] array = base.Session.Find<ADSchemaAttributeObject>(base.Session.SchemaNamingContext, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADSchemaAttributeSchema.LdapDisplayName, ExchangeConfigurationContainerSchemaWithAddressLists.DefaultGlobalAddressList2.LdapDisplayName), null, 1);
			return array.Length > 0;
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ExchangeConfigurationContainerWithAddressLists.schema;
			}
		}

		private static ExchangeConfigurationContainerSchemaWithAddressLists schema = ObjectSchema.GetInstance<ExchangeConfigurationContainerSchemaWithAddressLists>();
	}
}
