using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class BitMaskRestriction : Restriction
	{
		internal BitMaskRestriction(BitMaskOperator bitMaskOperator, PropertyTag propertyTag, uint bitMask)
		{
			this.bitMaskOperator = bitMaskOperator;
			this.propertyTag = propertyTag;
			this.bitMask = bitMask;
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.BitMask;
			}
		}

		internal new static BitMaskRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			BitMaskOperator bitMaskOperator = (BitMaskOperator)reader.ReadByte();
			PropertyTag propertyTag = reader.ReadPropertyTag();
			uint num = reader.ReadUInt32();
			return new BitMaskRestriction(bitMaskOperator, propertyTag, num);
		}

		public override void Serialize(Writer writer, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			base.Serialize(writer, string8Encoding, wireFormatStyle);
			writer.WriteByte((byte)this.bitMaskOperator);
			writer.WriteUInt32(this.propertyTag);
			writer.WriteUInt32(this.bitMask);
		}

		public PropertyTag PropertyTag
		{
			get
			{
				return this.propertyTag;
			}
		}

		public uint BitMask
		{
			get
			{
				return this.bitMask;
			}
		}

		public BitMaskOperator BitMaskOperator
		{
			get
			{
				return this.bitMaskOperator;
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" [Operator=").Append(this.BitMaskOperator);
			stringBuilder.Append(" Tag=").Append(this.PropertyTag.ToString());
			stringBuilder.Append(" Mask=0x").Append(this.BitMask.ToString("X08"));
			stringBuilder.Append("]");
		}

		private readonly BitMaskOperator bitMaskOperator;

		private readonly uint bitMask;

		private readonly PropertyTag propertyTag;
	}
}
