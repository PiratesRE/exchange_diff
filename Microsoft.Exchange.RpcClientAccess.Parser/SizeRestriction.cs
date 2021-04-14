using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class SizeRestriction : Restriction
	{
		internal SizeRestriction(RelationOperator relop, PropertyTag propertyTag, uint size)
		{
			this.relop = relop;
			this.propertyTag = propertyTag;
			this.size = size;
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.Size;
			}
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

		public uint Size
		{
			get
			{
				return this.size;
			}
		}

		internal new static SizeRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			RelationOperator relationOperator = (RelationOperator)reader.ReadByte();
			PropertyTag propertyTag = reader.ReadPropertyTag();
			uint num = reader.ReadUInt32();
			return new SizeRestriction(relationOperator, propertyTag, num);
		}

		public override void Serialize(Writer writer, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			base.Serialize(writer, string8Encoding, wireFormatStyle);
			writer.WriteByte((byte)this.relop);
			writer.WritePropertyTag(this.propertyTag);
			writer.WriteUInt32(this.size);
		}

		public override ErrorCode Validate()
		{
			if (this.propertyTag.ElementPropertyType == PropertyType.String8 && this.size > 2147483647U)
			{
				return (ErrorCode)2147746071U;
			}
			if (this.relop > (RelationOperator)255U)
			{
				return (ErrorCode)2147746050U;
			}
			return base.Validate();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" [Tag=").Append(this.PropertyTag.ToString());
			stringBuilder.Append(" Size=0x").Append(this.Size.ToString("X8"));
			stringBuilder.Append(" RelOp=").Append(this.RelationOperator);
			stringBuilder.Append("]");
		}

		private readonly RelationOperator relop;

		private readonly PropertyTag propertyTag;

		private readonly uint size;
	}
}
