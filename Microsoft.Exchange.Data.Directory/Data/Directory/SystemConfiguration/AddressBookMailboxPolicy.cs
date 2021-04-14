using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class AddressBookMailboxPolicy : MailboxPolicy
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return AddressBookMailboxPolicy.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AddressBookMailboxPolicy.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return AddressBookMailboxPolicy.parentPath;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override bool CheckForAssociatedUsers()
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.DistinguishedName, base.Id.DistinguishedName),
				new ExistsFilter(AddressBookMailboxPolicySchema.AssociatedUsers)
			});
			if (base.Session != null)
			{
				AddressBookMailboxPolicy[] array = base.Session.Find<AddressBookMailboxPolicy>(null, QueryScope.SubTree, filter, null, 1);
				return array != null && array.Length > 0;
			}
			return true;
		}

		public MultiValuedProperty<ADObjectId> AddressLists
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[AddressBookMailboxPolicySchema.AddressLists];
			}
			set
			{
				this[AddressBookMailboxPolicySchema.AddressLists] = value;
			}
		}

		public ADObjectId GlobalAddressList
		{
			get
			{
				return (ADObjectId)this[AddressBookMailboxPolicySchema.GlobalAddressList];
			}
			set
			{
				this[AddressBookMailboxPolicySchema.GlobalAddressList] = value;
			}
		}

		public ADObjectId RoomList
		{
			get
			{
				return (ADObjectId)this[AddressBookMailboxPolicySchema.RoomList];
			}
			set
			{
				this[AddressBookMailboxPolicySchema.RoomList] = value;
			}
		}

		public ADObjectId OfflineAddressBook
		{
			get
			{
				return (ADObjectId)this[AddressBookMailboxPolicySchema.OfflineAddressBook];
			}
			set
			{
				this[AddressBookMailboxPolicySchema.OfflineAddressBook] = value;
			}
		}

		internal ValidationError AddAddressListToPolicy(IConfigurationSession session, AddressBookBase addressListToAdd)
		{
			this.AddressLists.Add(addressListToAdd.Id);
			return null;
		}

		private static AddressBookMailboxPolicySchema schema = ObjectSchema.GetInstance<AddressBookMailboxPolicySchema>();

		private static string mostDerivedClass = "msExchAddressBookMailboxPolicy";

		private static ADObjectId parentPath = new ADObjectId("CN=AddressBook Mailbox Policies");
	}
}
