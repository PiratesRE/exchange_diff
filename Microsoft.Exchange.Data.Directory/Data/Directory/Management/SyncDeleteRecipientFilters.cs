using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class SyncDeleteRecipientFilters
	{
		public static QueryFilter GetFilter(SyncRecipientType recipientType)
		{
			return SyncDeleteRecipientFilters.Filters[recipientType];
		}

		private static readonly QueryFilter Mailbox = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADUser.MostDerivedClass),
			new ExistsFilter(IADMailStorageSchema.ServerLegacyDN)
		});

		private static readonly QueryFilter MailContact = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADContact.MostDerivedClass);

		private static readonly QueryFilter MailUser = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADUser.MostDerivedClass),
			new NotFilter(new ExistsFilter(IADMailStorageSchema.ServerLegacyDN))
		});

		private static readonly QueryFilter DistributionGroup = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADGroup.MostDerivedClass);

		private static readonly QueryFilter DynamicDistributionGroup = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADDynamicGroup.MostDerivedClass)
		});

		private static readonly QueryFilter All = new ExistsFilter(ADObjectSchema.ObjectClass);

		private static readonly IDictionary<SyncRecipientType, QueryFilter> Filters = new Dictionary<SyncRecipientType, QueryFilter>
		{
			{
				SyncRecipientType.Mailbox,
				SyncDeleteRecipientFilters.Mailbox
			},
			{
				SyncRecipientType.MailContact,
				SyncDeleteRecipientFilters.MailContact
			},
			{
				SyncRecipientType.MailUser,
				SyncDeleteRecipientFilters.MailUser
			},
			{
				SyncRecipientType.DistributionGroup,
				SyncDeleteRecipientFilters.DistributionGroup
			},
			{
				SyncRecipientType.DynamicDistributionGroup,
				SyncDeleteRecipientFilters.DynamicDistributionGroup
			},
			{
				SyncRecipientType.All,
				SyncDeleteRecipientFilters.All
			}
		};
	}
}
