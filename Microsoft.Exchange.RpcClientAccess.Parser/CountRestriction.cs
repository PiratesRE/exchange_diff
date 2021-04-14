using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class CountRestriction : SingleRestriction
	{
		internal CountRestriction(uint count, Restriction childRestriction) : base(childRestriction)
		{
			this.count = count;
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.Count;
			}
		}

		public uint Count
		{
			get
			{
				return this.count;
			}
		}

		internal new static CountRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			uint num = reader.ReadUInt32();
			Restriction childRestriction = Restriction.InternalParse(reader, wireFormatStyle, depth);
			return new CountRestriction(num, childRestriction);
		}

		public override void Serialize(Writer writer, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			base.Serialize(writer, string8Encoding, wireFormatStyle);
			writer.WriteUInt32(this.count);
			base.ChildRestriction.Serialize(writer, string8Encoding, wireFormatStyle);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" [Count=").Append(this.Count);
			if (base.ChildRestriction != null)
			{
				stringBuilder.Append(" Child=[").Append(base.ChildRestriction).Append("]");
			}
			stringBuilder.Append("]");
		}

		private readonly uint count;
	}
}
