using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class CountRestrictionData : HierRestrictionData
	{
		public CountRestrictionData()
		{
		}

		[DataMember]
		public int Count { get; set; }

		internal CountRestrictionData(Restriction.CountRestriction r)
		{
			this.Count = r.Count;
			base.ParseRestrictions(new Restriction[]
			{
				r.Restriction
			});
		}

		internal CountRestrictionData(StoreSession storeSession, CountFilter countFilter)
		{
			int count = (int)countFilter.Count;
			if (countFilter.Count > 2147483647U)
			{
				count = int.MaxValue;
			}
			this.Count = count;
			base.ParseQueryFilter(storeSession, countFilter.Filter);
		}

		internal override Restriction GetRestriction()
		{
			return Restriction.Count(this.Count, base.GetRestrictions()[0]);
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			return new CountFilter((uint)this.Count, base.GetQueryFilters(storeSession)[0]);
		}

		internal override string ToStringInternal()
		{
			return string.Format("COUNT[{0}, {1}]", this.Count, base.Restrictions[0].ToStringInternal());
		}

		internal override int GetApproximateSize()
		{
			return base.GetApproximateSize() + 4;
		}
	}
}
