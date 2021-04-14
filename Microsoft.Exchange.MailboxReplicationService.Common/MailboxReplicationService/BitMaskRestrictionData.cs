using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class BitMaskRestrictionData : RestrictionData
	{
		public BitMaskRestrictionData()
		{
		}

		[DataMember]
		public int RelBmr { get; set; }

		[DataMember]
		public int PropTag { get; set; }

		[DataMember]
		public int Mask { get; set; }

		internal BitMaskRestrictionData(Restriction.BitMaskRestriction r)
		{
			this.RelBmr = (int)r.Bmr;
			this.PropTag = (int)r.Tag;
			this.Mask = r.Mask;
		}

		internal BitMaskRestrictionData(StoreSession storeSession, BitMaskFilter bitMaskFilter)
		{
			this.RelBmr = (bitMaskFilter.IsNonZero ? 1 : 0);
			this.PropTag = base.GetPropTagFromDefinition(storeSession, bitMaskFilter.Property);
			this.Mask = (int)bitMaskFilter.Mask;
		}

		internal override Restriction GetRestriction()
		{
			return new Restriction.BitMaskRestriction((Restriction.RelBmr)this.RelBmr, (PropTag)this.PropTag, this.Mask);
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			return new BitMaskFilter(base.GetPropertyDefinitionFromPropTag(storeSession, this.PropTag), (ulong)((long)this.Mask), this.RelBmr == 1);
		}

		internal override void InternalEnumPropTags(CommonUtils.EnumPropTagDelegate del)
		{
			int propTag = this.PropTag;
			del(ref propTag);
			this.PropTag = propTag;
		}

		internal override string ToStringInternal()
		{
			return string.Format("BITMASK[ptag:{0}, {1}, mask:0x{2:X}]", TraceUtils.DumpPropTag((PropTag)this.PropTag), (Restriction.RelBmr)this.RelBmr, this.Mask);
		}

		internal override int GetApproximateSize()
		{
			return base.GetApproximateSize() + 12;
		}
	}
}
