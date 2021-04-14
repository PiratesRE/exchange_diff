using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class SubRestriction : SingleRestriction
	{
		internal SubRestriction(SubRestrictionType subRestrictionType, Restriction childRestriction) : base(childRestriction)
		{
			this.subRestrictionType = subRestrictionType;
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.SubRestriction;
			}
		}

		public SubRestrictionType SubRestrictionType
		{
			get
			{
				return this.subRestrictionType;
			}
		}

		internal new static SubRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			SubRestrictionType subRestrictionType = (SubRestrictionType)reader.ReadUInt32();
			Restriction childRestriction;
			if (wireFormatStyle == WireFormatStyle.Nspi && !reader.ReadBool())
			{
				childRestriction = null;
			}
			else
			{
				childRestriction = Restriction.InternalParse(reader, wireFormatStyle, depth);
			}
			return new SubRestriction(subRestrictionType, childRestriction);
		}

		public override void Serialize(Writer writer, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			base.Serialize(writer, string8Encoding, wireFormatStyle);
			writer.WriteUInt32((uint)this.subRestrictionType);
			if (wireFormatStyle == WireFormatStyle.Nspi)
			{
				bool flag = base.ChildRestriction != null;
				writer.WriteBool(flag);
				if (flag)
				{
					base.ChildRestriction.Serialize(writer, string8Encoding, wireFormatStyle);
					return;
				}
			}
			else
			{
				base.ChildRestriction.Serialize(writer, string8Encoding, wireFormatStyle);
			}
		}

		public override ErrorCode Validate()
		{
			if (this.subRestrictionType != SubRestrictionType.Recipients && this.subRestrictionType != SubRestrictionType.Attachments)
			{
				return (ErrorCode)2147746050U;
			}
			return base.Validate();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" [");
			if (base.ChildRestriction != null)
			{
				stringBuilder.Append(base.ChildRestriction);
			}
			stringBuilder.Append("]");
		}

		private readonly SubRestrictionType subRestrictionType;
	}
}
