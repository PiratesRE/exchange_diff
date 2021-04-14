using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class SizeRestrictionData : RestrictionData
	{
		public SizeRestrictionData()
		{
		}

		[DataMember]
		public int RelOp { get; set; }

		[DataMember]
		public int PropTag { get; set; }

		[DataMember]
		public int Size { get; set; }

		internal SizeRestrictionData(Restriction.SizeRestriction r)
		{
			this.RelOp = (int)r.Op;
			this.PropTag = (int)r.Tag;
			this.Size = r.Size;
		}

		internal SizeRestrictionData(StoreSession storeSession, SizeFilter filter)
		{
			this.RelOp = base.GetRelOpFromComparisonOperator(filter.ComparisonOperator);
			this.PropTag = base.GetPropTagFromDefinition(storeSession, filter.Property);
			this.Size = (int)filter.PropertySize;
		}

		internal override Restriction GetRestriction()
		{
			return new Restriction.SizeRestriction((Restriction.RelOp)this.RelOp, (PropTag)this.PropTag, this.Size);
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			return new SizeFilter(base.GetComparisonOperatorFromRelOp(this.RelOp), base.GetPropertyDefinitionFromPropTag(storeSession, this.PropTag), (uint)this.Size);
		}

		internal override void InternalEnumPropTags(CommonUtils.EnumPropTagDelegate del)
		{
			int propTag = this.PropTag;
			del(ref propTag);
			this.PropTag = propTag;
		}

		internal override string ToStringInternal()
		{
			return string.Format("SIZE[ptag:{0}, {1}, size:{2}]", TraceUtils.DumpPropTag((PropTag)this.PropTag), (Restriction.RelOp)this.RelOp, this.Size);
		}

		internal override int GetApproximateSize()
		{
			return base.GetApproximateSize() + 12;
		}
	}
}
