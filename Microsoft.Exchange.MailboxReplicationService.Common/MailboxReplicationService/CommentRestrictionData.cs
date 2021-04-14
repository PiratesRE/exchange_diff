using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class CommentRestrictionData : HierRestrictionData
	{
		public CommentRestrictionData()
		{
		}

		[DataMember]
		public PropValueData[] PropValues { get; set; }

		internal CommentRestrictionData(Restriction.CommentRestriction r)
		{
			base.ParseRestrictions(new Restriction[]
			{
				r.Restriction
			});
			this.PropValues = new PropValueData[r.Values.Length];
			for (int i = 0; i < r.Values.Length; i++)
			{
				this.PropValues[i] = DataConverter<PropValueConverter, PropValue, PropValueData>.GetData(r.Values[i]);
			}
		}

		internal CommentRestrictionData(StoreSession storeSession, CommentFilter filter)
		{
			base.ParseQueryFilter(storeSession, filter.Filter);
			this.PropValues = new PropValueData[filter.Values.Length];
			for (int i = 0; i < filter.Values.Length; i++)
			{
				this.PropValues[i] = new PropValueData((PropTag)base.GetPropTagFromDefinition(storeSession, filter.Properties[i]), filter.Values[i]);
			}
		}

		internal override Restriction GetRestriction()
		{
			PropValue[] array = new PropValue[this.PropValues.Length];
			for (int i = 0; i < this.PropValues.Length; i++)
			{
				array[i] = DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(this.PropValues[i]);
			}
			return Restriction.Comment(base.GetRestrictions()[0], array);
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			NativeStorePropertyDefinition[] array = new NativeStorePropertyDefinition[this.PropValues.Length];
			object[] array2 = new object[this.PropValues.Length];
			for (int i = 0; i < this.PropValues.Length; i++)
			{
				array[i] = base.GetPropertyDefinitionFromPropTag(storeSession, this.PropValues[i].PropTag);
				array2[i] = this.PropValues[i].Value;
			}
			return new CommentFilter(array, array2, base.GetQueryFilters(storeSession)[0]);
		}

		internal override void InternalEnumPropTags(CommonUtils.EnumPropTagDelegate del)
		{
			foreach (PropValueData propValueData in this.PropValues)
			{
				int propTag = propValueData.PropTag;
				del(ref propTag);
				propValueData.PropTag = propTag;
			}
		}

		internal override void InternalEnumPropValues(CommonUtils.EnumPropValueDelegate del)
		{
			foreach (PropValueData pval in this.PropValues)
			{
				del(pval);
			}
		}

		internal override string ToStringInternal()
		{
			return string.Format("COMMENT[{0}, {1}]", CommonUtils.ConcatEntries<PropValueData>(this.PropValues, null), base.Restrictions[0].ToStringInternal());
		}

		internal override int GetApproximateSize()
		{
			int num = base.GetApproximateSize();
			foreach (PropValueData propValueData in this.PropValues)
			{
				num += propValueData.GetApproximateSize();
			}
			return num;
		}
	}
}
