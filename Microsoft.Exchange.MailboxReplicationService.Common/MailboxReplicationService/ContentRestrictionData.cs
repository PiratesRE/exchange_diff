using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class ContentRestrictionData : RestrictionData
	{
		public ContentRestrictionData()
		{
		}

		[DataMember]
		public int Flags { get; set; }

		[DataMember]
		public int PropTag { get; set; }

		[DataMember]
		public bool MultiValued { get; set; }

		[DataMember]
		public PropValueData Value { get; set; }

		internal ContentRestrictionData(Restriction.ContentRestriction r)
		{
			this.Flags = (int)r.Flags;
			this.PropTag = (int)r.PropTag;
			this.MultiValued = r.MultiValued;
			this.Value = DataConverter<PropValueConverter, PropValue, PropValueData>.GetData(r.PropValue);
		}

		internal ContentRestrictionData(StoreSession storeSession, ContentFilter contentFilter)
		{
			this.Flags = (int)base.GetContentFlags(contentFilter.MatchFlags, contentFilter.MatchOptions);
			this.PropTag = base.GetPropTagFromDefinition(storeSession, contentFilter.Property);
			this.MultiValued = ((PropTag)this.PropTag).IsMultiValued();
			if (contentFilter is TextFilter)
			{
				TextFilter textFilter = (TextFilter)contentFilter;
				this.Value = new PropValueData(((PropTag)this.PropTag).ChangePropType(PropType.String), textFilter.Text);
				return;
			}
			if (contentFilter is BinaryFilter)
			{
				BinaryFilter binaryFilter = (BinaryFilter)contentFilter;
				this.Value = new PropValueData(((PropTag)this.PropTag).ChangePropType(PropType.Binary), binaryFilter.BinaryData);
				return;
			}
			MrsTracer.Common.Error("Unknown content filter type '{0}' in content restriction data constructor", new object[]
			{
				contentFilter.GetType()
			});
			throw new CorruptRestrictionDataException();
		}

		internal override Restriction GetRestriction()
		{
			return new Restriction.ContentRestriction((PropTag)this.PropTag, this.MultiValued, DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(this.Value).Value, (ContentFlags)this.Flags);
		}

		internal override QueryFilter GetQueryFilter(StoreSession storeSession)
		{
			PropertyDefinition propertyDefinitionFromPropTag = base.GetPropertyDefinitionFromPropTag(storeSession, this.PropTag);
			MatchFlags matchFlags;
			MatchOptions matchOptions;
			base.GetMatchFlagsAndOptions((ContentFlags)this.Flags, out matchFlags, out matchOptions);
			if (this.Value.Value is string)
			{
				return new TextFilter(propertyDefinitionFromPropTag, (string)this.Value.Value, matchOptions, matchFlags);
			}
			if (this.Value.Value is byte[])
			{
				return new BinaryFilter(propertyDefinitionFromPropTag, (byte[])this.Value.Value, matchOptions, matchFlags);
			}
			MrsTracer.Common.Error("Unknown value type '{0}' in content filter.", new object[]
			{
				this.Value.Value.GetType()
			});
			throw new CorruptRestrictionDataException();
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
			return string.Format("CONTENT[ptag:{0}, {1}{2}, val:{3}]", new object[]
			{
				TraceUtils.DumpPropTag((PropTag)this.PropTag),
				(ContentFlags)this.Flags,
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
