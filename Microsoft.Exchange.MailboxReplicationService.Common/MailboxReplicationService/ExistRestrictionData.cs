using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class ExistRestrictionData : RestrictionData
	{
		public ExistRestrictionData()
		{
		}

		[DataMember]
		public int PropTag { get; set; }

		internal ExistRestrictionData(Restriction.ExistRestriction r)
		{
			this.PropTag = (int)r.Tag;
		}

		internal ExistRestrictionData(StoreSession storeSession, ExistsFilter filter)
		{
			this.PropTag = base.GetPropTagFromDefinition(storeSession, filter.Property);
		}

		internal override Restriction GetRestriction()
		{
			return Restriction.Exist((PropTag)this.PropTag);
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			return new ExistsFilter(base.GetPropertyDefinitionFromPropTag(storeSession, this.PropTag));
		}

		internal override void InternalEnumPropTags(CommonUtils.EnumPropTagDelegate del)
		{
			int propTag = this.PropTag;
			del(ref propTag);
			this.PropTag = propTag;
		}

		internal override string ToStringInternal()
		{
			return string.Format("EXIST[ptag:{0}]", TraceUtils.DumpPropTag((PropTag)this.PropTag));
		}

		internal override int GetApproximateSize()
		{
			return base.GetApproximateSize() + 4;
		}
	}
}
