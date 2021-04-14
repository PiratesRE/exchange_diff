using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class ComparePropertyRestrictionData : RestrictionData
	{
		public ComparePropertyRestrictionData()
		{
		}

		[DataMember]
		public int RelOp { get; set; }

		[DataMember]
		public int TagLeft { get; set; }

		[DataMember]
		public int TagRight { get; set; }

		internal ComparePropertyRestrictionData(Restriction.ComparePropertyRestriction r)
		{
			this.RelOp = (int)r.Op;
			this.TagLeft = (int)r.TagLeft;
			this.TagRight = (int)r.TagRight;
		}

		internal ComparePropertyRestrictionData(StoreSession storeSession, PropertyComparisonFilter filter)
		{
			this.RelOp = base.GetRelOpFromComparisonOperator(filter.ComparisonOperator);
			this.TagLeft = base.GetPropTagFromDefinition(storeSession, filter.Property1);
			this.TagRight = base.GetPropTagFromDefinition(storeSession, filter.Property2);
		}

		internal override Restriction GetRestriction()
		{
			return new Restriction.ComparePropertyRestriction((Restriction.RelOp)this.RelOp, (PropTag)this.TagLeft, (PropTag)this.TagRight);
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			return new PropertyComparisonFilter(base.GetComparisonOperatorFromRelOp(this.RelOp), base.GetPropertyDefinitionFromPropTag(storeSession, this.TagLeft), base.GetPropertyDefinitionFromPropTag(storeSession, this.TagRight));
		}

		internal override void InternalEnumPropTags(CommonUtils.EnumPropTagDelegate del)
		{
			int tagLeft = this.TagLeft;
			int tagRight = this.TagRight;
			del(ref tagLeft);
			this.TagLeft = tagLeft;
			del(ref tagRight);
			this.TagRight = tagRight;
		}

		internal override string ToStringInternal()
		{
			return string.Format("COMPARE[ptag:{0}, {1}, ptag:{2}]", TraceUtils.DumpPropTag((PropTag)this.TagLeft), (Restriction.RelOp)this.RelOp, TraceUtils.DumpPropTag((PropTag)this.TagLeft));
		}

		internal override int GetApproximateSize()
		{
			return base.GetApproximateSize() + 12;
		}
	}
}
