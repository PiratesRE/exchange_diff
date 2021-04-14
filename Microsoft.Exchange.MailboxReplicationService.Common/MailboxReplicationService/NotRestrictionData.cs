using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class NotRestrictionData : HierRestrictionData
	{
		public NotRestrictionData()
		{
		}

		internal NotRestrictionData(Restriction.NotRestriction r)
		{
			base.ParseRestrictions(new Restriction[]
			{
				r.Restriction
			});
		}

		internal NotRestrictionData(StoreSession storeSession, NotFilter notFilter)
		{
			base.ParseQueryFilter(storeSession, notFilter.Filter);
		}

		internal override Restriction GetRestriction()
		{
			return Restriction.Not(base.GetRestrictions()[0]);
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			return new NotFilter(base.GetQueryFilters(storeSession)[0]);
		}

		internal override string ToStringInternal()
		{
			return string.Format("NOT[{0}]", base.Restrictions[0].ToStringInternal());
		}
	}
}
