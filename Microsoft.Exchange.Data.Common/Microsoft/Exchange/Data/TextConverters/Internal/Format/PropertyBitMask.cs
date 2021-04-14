using System;
using System.Text;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal struct PropertyBitMask
	{
		internal PropertyBitMask(uint bits1, uint bits2)
		{
			this.Bits1 = bits1;
			this.Bits2 = bits2;
		}

		public bool IsClear
		{
			get
			{
				return this.Bits1 == 0U && 0U == this.Bits2;
			}
		}

		public static PropertyBitMask operator |(PropertyBitMask x, PropertyBitMask y)
		{
			return new PropertyBitMask(x.Bits1 | y.Bits1, x.Bits2 | y.Bits2);
		}

		public static PropertyBitMask operator &(PropertyBitMask x, PropertyBitMask y)
		{
			return new PropertyBitMask(x.Bits1 & y.Bits1, x.Bits2 & y.Bits2);
		}

		public static PropertyBitMask operator ^(PropertyBitMask x, PropertyBitMask y)
		{
			return new PropertyBitMask(x.Bits1 ^ y.Bits1, x.Bits2 ^ y.Bits2);
		}

		public static PropertyBitMask operator ~(PropertyBitMask x)
		{
			return new PropertyBitMask(~x.Bits1, ~x.Bits2);
		}

		public static bool operator ==(PropertyBitMask x, PropertyBitMask y)
		{
			return x.Bits1 == y.Bits1 && x.Bits2 == y.Bits2;
		}

		public static bool operator !=(PropertyBitMask x, PropertyBitMask y)
		{
			return x.Bits1 != y.Bits1 || x.Bits2 != y.Bits2;
		}

		public void Or(PropertyBitMask newBits)
		{
			this.Bits1 |= newBits.Bits1;
			this.Bits2 |= newBits.Bits2;
		}

		public bool IsSet(PropertyId id)
		{
			return 0U != ((id < PropertyId.ListLevel) ? (this.Bits1 & 1U << (int)(id - PropertyId.FontColor)) : (this.Bits2 & 1U << (int)(id - PropertyId.FontColor - 32)));
		}

		public bool IsNotSet(PropertyId id)
		{
			return 0U == ((id < PropertyId.ListLevel) ? (this.Bits1 & 1U << (int)(id - PropertyId.FontColor)) : (this.Bits2 & 1U << (int)(id - PropertyId.FontColor - 32)));
		}

		public void Set(PropertyId id)
		{
			if (id < PropertyId.ListLevel)
			{
				this.Bits1 |= 1U << (int)(id - PropertyId.FontColor);
				return;
			}
			this.Bits2 |= 1U << (int)(id - PropertyId.FontColor - 32);
		}

		public void Clear(PropertyId id)
		{
			if (id < PropertyId.ListLevel)
			{
				this.Bits1 &= ~(1U << (int)(id - PropertyId.FontColor));
				return;
			}
			this.Bits2 &= ~(1U << (int)(id - PropertyId.FontColor - 32));
		}

		public bool IsSubsetOf(PropertyBitMask overrideFlags)
		{
			return (this.Bits1 & ~(overrideFlags.Bits1 != 0U)) == 0U && 0U == (this.Bits2 & ~overrideFlags.Bits2);
		}

		public void ClearAll()
		{
			this.Bits1 = 0U;
			this.Bits2 = 0U;
		}

		public override bool Equals(object obj)
		{
			return obj is PropertyBitMask && this.Bits1 == ((PropertyBitMask)obj).Bits1 && this.Bits2 == ((PropertyBitMask)obj).Bits2;
		}

		public override int GetHashCode()
		{
			return (int)(this.Bits1 ^ this.Bits2);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(896);
			for (PropertyId propertyId = PropertyId.FontColor; propertyId < PropertyId.MaxValue; propertyId += 1)
			{
				if (this.IsSet(propertyId))
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(propertyId.ToString());
				}
			}
			return stringBuilder.ToString();
		}

		public PropertyBitMask.DefinedPropertyIdEnumerator GetEnumerator()
		{
			return new PropertyBitMask.DefinedPropertyIdEnumerator(this);
		}

		internal void Set1(uint bits1)
		{
			this.Bits1 = bits1;
		}

		internal void Set2(uint bits2)
		{
			this.Bits2 = bits2;
		}

		public const PropertyId FirstNonFlag = PropertyId.FontColor;

		public static readonly PropertyBitMask AllOff = new PropertyBitMask(0U, 0U);

		public static readonly PropertyBitMask AllOn = new PropertyBitMask(uint.MaxValue, uint.MaxValue);

		internal uint Bits1;

		internal uint Bits2;

		public struct DefinedPropertyIdEnumerator
		{
			internal DefinedPropertyIdEnumerator(PropertyBitMask mask)
			{
				this.Bits = ((ulong)mask.Bits2 << 32 | (ulong)mask.Bits1);
				this.CurrentBit = 1UL;
				this.CurrentId = ((this.Bits != 0UL) ? PropertyId.MergedCell : PropertyId.MaxValue);
			}

			public PropertyId Current
			{
				get
				{
					return this.CurrentId;
				}
			}

			public bool MoveNext()
			{
				while (this.CurrentId != PropertyId.MaxValue)
				{
					if (this.CurrentId != PropertyId.MergedCell)
					{
						this.CurrentBit <<= 1;
					}
					this.CurrentId += 1;
					if (this.CurrentId != PropertyId.MaxValue && 0UL != (this.Bits & this.CurrentBit))
					{
						return true;
					}
				}
				return false;
			}

			internal ulong Bits;

			internal ulong CurrentBit;

			internal PropertyId CurrentId;
		}
	}
}
