using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class PropertyRestrictionData : RestrictionData
	{
		public PropertyRestrictionData()
		{
		}

		[DataMember]
		public int RelOp { get; set; }

		[DataMember]
		public int PropTag { get; set; }

		[DataMember]
		public bool MultiValued { get; set; }

		[DataMember]
		public PropValueData Value { get; set; }

		internal PropertyRestrictionData(Restriction.PropertyRestriction r)
		{
			this.RelOp = (int)r.Op;
			this.PropTag = (int)r.PropTag;
			this.MultiValued = r.MultiValued;
			this.Value = DataConverter<PropValueConverter, PropValue, PropValueData>.GetData(r.PropValue);
		}

		internal PropertyRestrictionData(StoreSession storeSession, ComparisonFilter comparisonFilter)
		{
			this.RelOp = base.GetRelOpFromComparisonOperator(comparisonFilter.ComparisonOperator);
			this.PropTag = base.GetPropTagFromDefinition(storeSession, comparisonFilter.Property);
			this.MultiValued = (comparisonFilter is MultivaluedInstanceComparisonFilter);
			this.Value = new PropValueData((PropTag)this.PropTag, comparisonFilter.PropertyValue);
		}

		internal override Restriction GetRestriction()
		{
			return new Restriction.PropertyRestriction((Restriction.RelOp)this.RelOp, (PropTag)this.PropTag, this.MultiValued, DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(this.Value).Value);
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			return new ComparisonFilter(base.GetComparisonOperatorFromRelOp(this.RelOp), base.GetPropertyDefinitionFromPropTag(storeSession, this.PropTag), this.Value.Value);
		}

		internal override void InternalEnumPropTags(CommonUtils.EnumPropTagDelegate del)
		{
			int propTag = this.PropTag;
			del(ref propTag);
			this.PropTag = propTag;
		}

		internal override void InternalEnumPropValues(CommonUtils.EnumPropValueDelegate del)
		{
			del(this.Value);
		}

		internal override string ToStringInternal()
		{
			return string.Format("PROPERTY[ptag:{0}, {1}{2}, val:{3}]", new object[]
			{
				TraceUtils.DumpPropTag((PropTag)this.PropTag),
				(Restriction.RelOp)this.RelOp,
				this.MultiValued ? " (mv)" : string.Empty,
				this.Value
			});
		}

		internal override int GetApproximateSize()
		{
			return base.GetApproximateSize() + 9 + this.Value.GetApproximateSize();
		}
	}
}
