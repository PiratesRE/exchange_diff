using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class NearRestriction : SingleRestriction
	{
		internal NearRestriction(uint distance, bool ordered, Restriction childRestriction) : base(childRestriction)
		{
			this.distance = distance;
			this.ordered = ordered;
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.Near;
			}
		}

		public uint Distance
		{
			get
			{
				return this.distance;
			}
		}

		public bool Ordered
		{
			get
			{
				return this.ordered;
			}
		}

		internal new static NearRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			uint num = reader.ReadUInt32();
			bool flag = reader.ReadUInt32() == 1U;
			Restriction childRestriction = Restriction.InternalParse(reader, wireFormatStyle, depth);
			return new NearRestriction(num, flag, childRestriction);
		}

		public override ErrorCode Validate()
		{
			AndRestriction andRestriction = base.ChildRestriction as AndRestriction;
			if (andRestriction == null)
			{
				return (ErrorCode)2147942487U;
			}
			if (andRestriction.ChildRestrictions == null || andRestriction.ChildRestrictions.Length < 2)
			{
				return (ErrorCode)2147942487U;
			}
			if (!NearRestriction.ValidateNestedRestriction(andRestriction.ChildRestrictions))
			{
				return (ErrorCode)2147746071U;
			}
			return base.Validate();
		}

		private static bool ValidateNestedRestriction(Restriction[] nestedRestrictions)
		{
			if (nestedRestrictions == null || nestedRestrictions.Length == 0)
			{
				return false;
			}
			foreach (Restriction restriction in nestedRestrictions)
			{
				if (restriction is OrRestriction)
				{
					if (!NearRestriction.ValidateNestedRestriction(((OrRestriction)restriction).ChildRestrictions))
					{
						return false;
					}
				}
				else if (!(restriction is ContentRestriction))
				{
					return false;
				}
			}
			return true;
		}

		public override void Serialize(Writer writer, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			base.Serialize(writer, string8Encoding, wireFormatStyle);
			writer.WriteUInt32(this.distance);
			writer.WriteUInt32(this.ordered ? 1U : 0U);
			base.ChildRestriction.Serialize(writer, string8Encoding, wireFormatStyle);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" [Distance=").Append(this.Distance);
			stringBuilder.Append(", Ordered=").Append(this.Ordered);
			if (base.ChildRestriction != null)
			{
				stringBuilder.Append(" Child=[").Append(base.ChildRestriction).Append("]");
			}
			stringBuilder.Append("]");
		}

		private readonly uint distance;

		private readonly bool ordered;
	}
}
