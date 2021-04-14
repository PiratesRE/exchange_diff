using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class ComparePropsRestriction : Restriction
	{
		internal ComparePropsRestriction(RelationOperator relop, PropertyTag property1, PropertyTag property2)
		{
			this.relop = relop;
			this.property1 = property1;
			this.property2 = property2;
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.CompareProps;
			}
		}

		public RelationOperator RelationOperator
		{
			get
			{
				return this.relop;
			}
		}

		public PropertyTag Property1
		{
			get
			{
				return this.property1;
			}
		}

		public PropertyTag Property2
		{
			get
			{
				return this.property2;
			}
		}

		internal new static ComparePropsRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			RelationOperator relationOperator = (RelationOperator)reader.ReadByte();
			PropertyTag propertyTag = reader.ReadPropertyTag();
			PropertyTag propertyTag2 = reader.ReadPropertyTag();
			return new ComparePropsRestriction(relationOperator, propertyTag, propertyTag2);
		}

		public override void Serialize(Writer writer, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			base.Serialize(writer, string8Encoding, wireFormatStyle);
			writer.WriteByte((byte)this.relop);
			writer.WritePropertyTag(this.property1);
			writer.WritePropertyTag(this.property2);
		}

		public override ErrorCode Validate()
		{
			if ((byte)this.relop > 255)
			{
				return (ErrorCode)2147746050U;
			}
			return base.Validate();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" [Tag1=").Append(this.Property1.ToString());
			stringBuilder.Append(" Tag2=").Append(this.Property2.ToString());
			stringBuilder.Append(" RelOp=").Append(this.RelationOperator);
			stringBuilder.Append("]");
		}

		private readonly PropertyTag property1;

		private readonly PropertyTag property2;

		private readonly RelationOperator relop;
	}
}
