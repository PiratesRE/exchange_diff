using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class NotRestriction : SingleRestriction
	{
		internal NotRestriction(Restriction childRestriction) : base(childRestriction)
		{
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.Not;
			}
		}

		internal new static NotRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			Restriction childRestriction = Restriction.InternalParse(reader, wireFormatStyle, depth);
			return new NotRestriction(childRestriction);
		}

		public override void Serialize(Writer writer, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			base.Serialize(writer, string8Encoding, wireFormatStyle);
			base.ChildRestriction.Serialize(writer, string8Encoding, wireFormatStyle);
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
	}
}
