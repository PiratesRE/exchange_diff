using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class OrRestrictionData : HierRestrictionData
	{
		public OrRestrictionData()
		{
		}

		internal OrRestrictionData(Restriction.OrRestriction r)
		{
			base.ParseRestrictions(r.Restrictions);
		}

		internal OrRestrictionData(StoreSession storeSession, OrFilter orFilter)
		{
			base.ParseQueryFilters(storeSession, orFilter.Filters);
		}

		internal override Restriction GetRestriction()
		{
			return Restriction.Or(base.GetRestrictions());
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			return new OrFilter(base.GetQueryFilters(storeSession));
		}

		internal override string ToStringInternal()
		{
			return base.ConcatSubRestrictions("OR");
		}
	}
}
