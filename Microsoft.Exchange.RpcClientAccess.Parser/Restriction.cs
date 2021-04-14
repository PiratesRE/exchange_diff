using System;
using System.Collections;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal abstract class Restriction
	{
		public static Restriction Parse(Reader reader, WireFormatStyle wireFormatStyle)
		{
			return Restriction.InternalParse(reader, wireFormatStyle, 0U);
		}

		internal static Restriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			if (depth++ >= 256U)
			{
				throw new BufferParseException("Restriction structure exceeds maximal depth.");
			}
			RestrictionType restrictionType = (RestrictionType)reader.ReadByte();
			RestrictionType restrictionType2 = restrictionType;
			switch (restrictionType2)
			{
			case RestrictionType.And:
				return AndRestriction.InternalParse(reader, wireFormatStyle, depth);
			case RestrictionType.Or:
				return OrRestriction.InternalParse(reader, wireFormatStyle, depth);
			case RestrictionType.Not:
				return NotRestriction.InternalParse(reader, wireFormatStyle, depth);
			case RestrictionType.Content:
				return ContentRestriction.InternalParse(reader, wireFormatStyle, depth);
			case RestrictionType.Property:
				return PropertyRestriction.InternalParse(reader, wireFormatStyle, depth);
			case RestrictionType.CompareProps:
				return ComparePropsRestriction.InternalParse(reader, wireFormatStyle, depth);
			case RestrictionType.BitMask:
				return BitMaskRestriction.InternalParse(reader, wireFormatStyle, depth);
			case RestrictionType.Size:
				return SizeRestriction.InternalParse(reader, wireFormatStyle, depth);
			case RestrictionType.Exists:
				return ExistsRestriction.InternalParse(reader, wireFormatStyle, depth);
			case RestrictionType.SubRestriction:
				return SubRestriction.InternalParse(reader, wireFormatStyle, depth);
			case RestrictionType.Comment:
				return CommentRestriction.InternalParse(reader, wireFormatStyle, depth);
			case RestrictionType.Count:
				return CountRestriction.InternalParse(reader, wireFormatStyle, depth);
			case (RestrictionType)12U:
				break;
			case RestrictionType.Near:
				return NearRestriction.InternalParse(reader, wireFormatStyle, depth);
			default:
				switch (restrictionType2)
				{
				case RestrictionType.True:
					return TrueRestriction.InternalParse(reader, wireFormatStyle, depth);
				case RestrictionType.False:
					return FalseRestriction.InternalParse(reader, wireFormatStyle, depth);
				default:
					if (restrictionType2 == RestrictionType.Null)
					{
						return NullRestriction.InternalParse(reader, wireFormatStyle, depth);
					}
					break;
				}
				break;
			}
			throw new BufferParseException(string.Format("The restriction type is not supported. Type = {0}.", restrictionType));
		}

		internal static PropertyValue? ReadNullablePropertyValue(Reader reader, WireFormatStyle wireFormatStyle)
		{
			if (wireFormatStyle == WireFormatStyle.Nspi && !reader.ReadBool())
			{
				return null;
			}
			return new PropertyValue?(reader.ReadPropertyValue(wireFormatStyle));
		}

		internal static void WriteNullablePropertyValue(Writer writer, PropertyValue? propertyValue, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			if (wireFormatStyle == WireFormatStyle.Nspi)
			{
				writer.WriteNullablePropertyValue(propertyValue, string8Encoding, wireFormatStyle);
				return;
			}
			writer.WritePropertyValue(propertyValue.Value, string8Encoding, wireFormatStyle);
		}

		public virtual void Serialize(Writer writer, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			writer.WriteByte((byte)this.RestrictionType);
		}

		internal abstract RestrictionType RestrictionType { get; }

		internal virtual void ResolveString8Values(Encoding string8Encoding)
		{
		}

		public virtual ErrorCode Validate()
		{
			return ErrorCode.None;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			this.AppendToString(stringBuilder);
			return stringBuilder.ToString();
		}

		public override bool Equals(object other)
		{
			Restriction restriction = other as Restriction;
			if (restriction == null)
			{
				return false;
			}
			if (this.GetHashCode() != other.GetHashCode())
			{
				return false;
			}
			byte[] x = this.Serialize();
			byte[] y = restriction.Serialize();
			return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
		}

		public override int GetHashCode()
		{
			if (this.hashCode == null)
			{
				byte[] obj = this.Serialize();
				this.hashCode = new int?(StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj));
			}
			return this.hashCode.Value;
		}

		private byte[] Serialize()
		{
			return BufferWriter.Serialize(delegate(Writer writer)
			{
				this.Serialize(writer, CTSGlobals.AsciiEncoding, WireFormatStyle.Rop);
			});
		}

		internal virtual void AppendToString(StringBuilder stringBuilder)
		{
			stringBuilder.Append(this.RestrictionType);
		}

		public const int MaximalDepth = 256;

		private int? hashCode;
	}
}
