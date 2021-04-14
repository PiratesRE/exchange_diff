using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class RestrictionTextProperty : Restriction
	{
		public RestrictionTextProperty(StorePropTag propertyTag, object value, RestrictionTextFuzzyLevel fuzzyLevel, RestrictionTextFullness fullness)
		{
			this.propertyTag = propertyTag;
			this.value = value;
			this.fullness = fullness;
			this.fuzzyLevel = fuzzyLevel;
		}

		public RestrictionTextProperty(Context context, byte[] byteForm, ref int position, int posMax, Mailbox mailbox, ObjectType objectType)
		{
			ParseSerialize.GetByte(byteForm, ref position, posMax);
			this.fullness = (RestrictionTextFullness)ParseSerialize.GetWord(byteForm, ref position, posMax);
			this.fuzzyLevel = (RestrictionTextFuzzyLevel)ParseSerialize.GetWord(byteForm, ref position, posMax);
			this.propertyTag = Mailbox.GetStorePropTag(context, mailbox, ParseSerialize.GetDword(byteForm, ref position, posMax), objectType);
			StorePropTag storePropTag = Mailbox.GetStorePropTag(context, mailbox, ParseSerialize.GetDword(byteForm, ref position, posMax), objectType);
			this.value = Restriction.DeserializeValue(byteForm, ref position, storePropTag);
			if (!Restriction.FSamePropType(storePropTag.PropType, this.propertyTag.PropType))
			{
				throw new StoreException((LID)38904U, ErrorCodeValue.TooComplex);
			}
			if (storePropTag.PropType != PropertyType.Unicode)
			{
				DiagnosticContext.TraceDword((LID)37736U, storePropTag.PropTag);
				throw new StoreException((LID)55288U, ErrorCodeValue.TooComplex);
			}
		}

		public StorePropTag PropertyTag
		{
			get
			{
				return this.propertyTag;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public RestrictionTextFuzzyLevel FuzzyLevel
		{
			get
			{
				return this.fuzzyLevel;
			}
		}

		public RestrictionTextFullness Fullness
		{
			get
			{
				return this.fullness;
			}
		}

		internal override void AppendToString(StringBuilder sb)
		{
			sb.Append("TEXT([");
			this.propertyTag.AppendToString(sb);
			sb.Append("], ");
			sb.AppendAsString(this.value);
			sb.Append(", ");
			sb.Append(this.fullness.ToString());
			sb.Append(", ");
			sb.Append(this.fuzzyLevel.ToString());
			sb.Append(')');
		}

		public override void Serialize(byte[] byteForm, ref int position)
		{
			ParseSerialize.SetByte(byteForm, ref position, 3);
			ParseSerialize.SetWord(byteForm, ref position, (ushort)this.fullness);
			ParseSerialize.SetWord(byteForm, ref position, (ushort)this.fuzzyLevel);
			ParseSerialize.SetDword(byteForm, ref position, this.propertyTag.ExternalTag);
			StorePropTag propTag = this.propertyTag;
			if (propTag.IsMultiValued && !(this.value is string[]))
			{
				propTag = StorePropTag.CreateWithoutInfo(this.propertyTag.PropId, this.propertyTag.PropType & (PropertyType)61439, this.propertyTag.BaseObjectType);
			}
			ParseSerialize.SetDword(byteForm, ref position, propTag.ExternalTag);
			Restriction.SerializeValue(byteForm, ref position, propTag, this.value);
		}

		public override SearchCriteria ToSearchCriteria(StoreDatabase database, ObjectType objectType)
		{
			Column lhs = PropertySchema.MapToColumn(database, objectType, this.propertyTag);
			Column rhs = Factory.CreateConstantColumn(this.value);
			SearchCriteriaText.SearchTextFullness searchTextFullness = RestrictionTextProperty.GetSearchTextFullness(this.fullness);
			SearchCriteriaText.SearchTextFuzzyLevel searchTextFuzzyLevel = RestrictionTextProperty.GetSearchTextFuzzyLevel(this.fuzzyLevel);
			return Factory.CreateSearchCriteriaText(lhs, searchTextFullness, searchTextFuzzyLevel, rhs);
		}

		public override bool RefersToProperty(StorePropTag propTag)
		{
			return this.propertyTag == propTag;
		}

		private static SearchCriteriaText.SearchTextFullness GetSearchTextFullness(RestrictionTextFullness fullness)
		{
			return (SearchCriteriaText.SearchTextFullness)fullness;
		}

		private static SearchCriteriaText.SearchTextFuzzyLevel GetSearchTextFuzzyLevel(RestrictionTextFuzzyLevel fuzzyLevel)
		{
			return (SearchCriteriaText.SearchTextFuzzyLevel)fuzzyLevel;
		}

		private readonly StorePropTag propertyTag;

		private readonly object value;

		private readonly RestrictionTextFuzzyLevel fuzzyLevel;

		private readonly RestrictionTextFullness fullness;
	}
}
