using System;
using System.Text;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class PropertyRestriction : Restriction
	{
		internal PropertyRestriction(RelationOperator relop, PropertyTag propertyTag, PropertyValue? propertyValue)
		{
			this.relop = relop;
			this.propertyTag = propertyTag;
			this.propertyValue = propertyValue;
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.Property;
			}
		}

		internal new static PropertyRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			RelationOperator relationOperator = (RelationOperator)reader.ReadByte();
			PropertyTag propertyTag = reader.ReadPropertyTag();
			return new PropertyRestriction(relationOperator, propertyTag, Restriction.ReadNullablePropertyValue(reader, wireFormatStyle));
		}

		public override void Serialize(Writer writer, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			base.Serialize(writer, string8Encoding, wireFormatStyle);
			writer.WriteByte((byte)this.relop);
			writer.WriteUInt32(this.propertyTag);
			Restriction.WriteNullablePropertyValue(writer, this.propertyValue, string8Encoding, wireFormatStyle);
		}

		public RelationOperator RelationOperator
		{
			get
			{
				return this.relop;
			}
		}

		public PropertyTag PropertyTag
		{
			get
			{
				return this.propertyTag;
			}
		}

		public PropertyValue? PropertyValue
		{
			get
			{
				return this.propertyValue;
			}
		}

		public override ErrorCode Validate()
		{
			if (this.propertyValue == null)
			{
				return (ErrorCode)2147942487U;
			}
			if (!EnumValidator.IsValidValue<RelationOperator>(this.relop))
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
			stringBuilder.Append(" [RelOp=").Append(this.RelationOperator);
			stringBuilder.Append(" Tag=").Append(this.PropertyTag.ToString());
			if (this.propertyValue != null)
			{
				stringBuilder.Append(" ").Append(this.propertyValue.Value);
			}
			else
			{
				stringBuilder.Append(" (null)");
			}
			stringBuilder.Append("]");
		}

		private readonly RelationOperator relop;

		private readonly PropertyValue? propertyValue;

		private readonly PropertyTag propertyTag;
	}
}
