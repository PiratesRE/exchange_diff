using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class AndRestrictionData : HierRestrictionData
	{
		public AndRestrictionData()
		{
		}

		internal AndRestrictionData(Restriction.AndRestriction r)
		{
			base.ParseRestrictions(r.Restrictions);
		}

		internal AndRestrictionData(StoreSession storeSession, AndFilter queryFilter)
		{
			base.ParseQueryFilters(storeSession, queryFilter.Filters);
		}

		internal override Restriction GetRestriction()
		{
			return Restriction.And(base.GetRestrictions());
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			return new AndFilter(base.GetQueryFilters(storeSession));
		}

		internal override string ToStringInternal()
		{
			return base.ConcatSubRestrictions("AND");
		}
	}
}
