using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class RecipientRestrictionData : HierRestrictionData
	{
		public RecipientRestrictionData()
		{
		}

		internal RecipientRestrictionData(Restriction.RecipientRestriction r)
		{
			base.ParseRestrictions(new Restriction[]
			{
				r.Restriction
			});
		}

		internal RecipientRestrictionData(StoreSession storeSession, SubFilter filter)
		{
			base.ParseQueryFilter(storeSession, filter.Filter);
		}

		internal override Restriction GetRestriction()
		{
			return Restriction.Sub(PropTag.MessageRecipients, base.GetRestrictions()[0]);
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			return new SubFilter(SubFilterProperties.Recipients, base.GetQueryFilters(storeSession)[0]);
		}

		internal override string ToStringInternal()
		{
			return string.Format("RECIPIENT[{0}]", base.Restrictions[0].ToStringInternal());
		}
	}
}
