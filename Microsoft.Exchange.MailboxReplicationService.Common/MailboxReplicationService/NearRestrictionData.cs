using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class NearRestrictionData : RestrictionData
	{
		public NearRestrictionData()
		{
		}

		[DataMember]
		public int Distance { get; set; }

		[DataMember]
		public bool Ordered { get; set; }

		[DataMember]
		public AndRestrictionData RestrictionData { get; set; }

		internal NearRestrictionData(Restriction.NearRestriction r)
		{
			if (r == null || r.Restriction == null)
			{
				MrsTracer.Common.Error("Null near restriction in near restriction data constructor", new object[0]);
				throw new CorruptRestrictionDataException();
			}
			this.Distance = r.Distance;
			this.Ordered = r.Ordered;
			this.RestrictionData = new AndRestrictionData(r.Restriction);
		}

		internal NearRestrictionData(StoreSession storeSession, NearFilter nearFilter)
		{
			if (nearFilter == null || nearFilter.Filter == null)
			{
				MrsTracer.Common.Error("Null near filter in near restriction data constructor", new object[0]);
				throw new CorruptRestrictionDataException();
			}
			this.Distance = (int)nearFilter.Distance;
			this.Ordered = nearFilter.Ordered;
			this.RestrictionData = new AndRestrictionData(storeSession, nearFilter.Filter);
		}

		internal override Restriction GetRestriction()
		{
			return Restriction.Near(this.Distance, this.Ordered, (Restriction.AndRestriction)this.RestrictionData.GetRestriction());
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			return new NearFilter((uint)this.Distance, this.Ordered, (AndFilter)this.RestrictionData.GetQueryFilter(storeSession));
		}

		internal override string ToStringInternal()
		{
			return string.Format("Near[distance:{0}, ordered:{1}, {2}]", this.Distance, this.Ordered, (this.RestrictionData == null) ? string.Empty : this.RestrictionData.ToStringInternal());
		}

		internal override int GetApproximateSize()
		{
			return base.GetApproximateSize() + 5 + this.RestrictionData.GetApproximateSize();
		}
	}
}
