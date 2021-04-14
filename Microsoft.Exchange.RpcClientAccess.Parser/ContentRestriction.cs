using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class ContentRestriction : Restriction
	{
		internal ContentRestriction(FuzzyLevel fuzzyLevel, PropertyTag propertyTag, PropertyValue? propertyValue)
		{
			this.fuzzyLevel = fuzzyLevel;
			this.propertyTag = propertyTag;
			this.propertyValue = propertyValue;
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.Content;
			}
		}

		internal new static ContentRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			FuzzyLevel fuzzyLevel = (FuzzyLevel)reader.ReadUInt32();
			PropertyTag propertyTag = reader.ReadPropertyTag();
			return new ContentRestriction(fuzzyLevel, propertyTag, Restriction.ReadNullablePropertyValue(reader, wireFormatStyle));
		}

		public override void Serialize(Writer writer, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			base.Serialize(writer, string8Encoding, wireFormatStyle);
			writer.WriteUInt32((uint)this.fuzzyLevel);
			writer.WritePropertyTag(this.propertyTag);
			Restriction.WriteNullablePropertyValue(writer, this.propertyValue, string8Encoding, wireFormatStyle);
		}

		public PropertyValue? PropertyValue
		{
			get
			{
				return this.propertyValue;
			}
		}

		public PropertyTag PropertyTag
		{
			get
			{
				return this.propertyTag;
			}
		}

		public FuzzyLevel FuzzyLevel
		{
			get
			{
				return this.fuzzyLevel;
			}
		}

		public override ErrorCode Validate()
		{
			if (this.propertyValue == null)
			{
				return (ErrorCode)2147942487U;
			}
			if (this.PropertyValue.Value.PropertyTag.IsMultiValuedProperty)
			{
				return (ErrorCode)2147746071U;
			}
			if (!PropertyTag.HasCompatiblePropertyType(this.propertyTag, this.PropertyValue.Value.PropertyTag))
			{
				return (ErrorCode)2147746071U;
			}
			if (!this.propertyValue.Value.PropertyTag.IsStringProperty && this.propertyValue.Value.PropertyTag.PropertyType != PropertyType.Binary)
			{
				return (ErrorCode)2147746071U;
			}
			return base.Validate();
		}

		internal override void ResolveString8Values(Encoding string8Encoding)
		{
			base.ResolveString8Values(string8Encoding);
			if (this.propertyValue != null)
			{
				this.propertyValue.Value.ResolveString8Values(string8Encoding);
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" [Tag=").Append(this.PropertyTag.ToString());
			stringBuilder.Append(" ").Append(this.PropertyValue);
			stringBuilder.Append(" Fuzzy=").Append(this.FuzzyLevel);
			stringBuilder.Append("]");
		}

		private readonly FuzzyLevel fuzzyLevel;

		private readonly PropertyValue? propertyValue;

		private readonly PropertyTag propertyTag;
	}
}
