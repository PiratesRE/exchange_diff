using System;
using System.Text;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal struct FlagProperties
	{
		internal FlagProperties(uint bits)
		{
			this.bits = bits;
		}

		public bool IsClear
		{
			get
			{
				return 0U == this.bits;
			}
		}

		public uint Mask
		{
			get
			{
				return (this.bits & 2863311530U) | (this.bits & 2863311530U) >> 1;
			}
		}

		public uint Bits
		{
			get
			{
				return this.bits;
			}
		}

		internal int IntegerBag
		{
			get
			{
				return (int)this.bits;
			}
			set
			{
				this.bits = (uint)value;
			}
		}

		public static bool IsFlagProperty(PropertyId id)
		{
			return id >= PropertyId.FirstFlag && id <= PropertyId.MergedCell;
		}

		public static FlagProperties Merge(FlagProperties baseFlags, FlagProperties overrideFlags)
		{
			return new FlagProperties((baseFlags.bits & ~((overrideFlags.bits & 2863311530U) >> 1)) | overrideFlags.bits);
		}

		public static FlagProperties operator &(FlagProperties x, FlagProperties y)
		{
			return new FlagProperties(x.bits & ((y.bits & 2863311530U) | (y.bits & 2863311530U) >> 1));
		}

		public static FlagProperties operator |(FlagProperties x, FlagProperties y)
		{
			return FlagProperties.Merge(x, y);
		}

		public static FlagProperties operator ^(FlagProperties x, FlagProperties y)
		{
			uint num = (x.bits ^ y.bits) & x.Mask & y.Mask;
			return new FlagProperties(num | num << 1);
		}

		public static FlagProperties operator ~(FlagProperties x)
		{
			return new FlagProperties(~((x.bits & 2863311530U) | (x.bits & 2863311530U) >> 1));
		}

		public static bool operator ==(FlagProperties x, FlagProperties y)
		{
			return x.bits == y.bits;
		}

		public static bool operator !=(FlagProperties x, FlagProperties y)
		{
			return x.bits != y.bits;
		}

		public void Set(PropertyId id, bool value)
		{
			int num = (int)((id - PropertyId.FirstFlag) * 2);
			if (value)
			{
				this.bits |= 3U << num;
				return;
			}
			this.bits &= ~(1U << num);
			this.bits |= 2U << num;
		}

		public void Remove(PropertyId id)
		{
			this.bits &= ~(3U << (int)((id - PropertyId.FirstFlag) * 2));
		}

		public void ClearAll()
		{
			this.bits = 0U;
		}

		public bool IsDefined(PropertyId id)
		{
			return 0U != (this.bits & 2U << (int)((id - PropertyId.FirstFlag) * 2));
		}

		public bool IsAnyDefined()
		{
			return this.bits != 0U;
		}

		public bool IsOn(PropertyId id)
		{
			return 0U != (this.bits & 1U << (int)((id - PropertyId.FirstFlag) * 2));
		}

		public bool IsDefinedAndOn(PropertyId id)
		{
			return 3U == (this.bits >> (int)((id - PropertyId.FirstFlag) * 2) & 3U);
		}

		public bool IsDefinedAndOff(PropertyId id)
		{
			return 2U == (this.bits >> (int)((id - PropertyId.FirstFlag) * 2) & 3U);
		}

		public PropertyValue GetPropertyValue(PropertyId id)
		{
			int num = (int)((id - PropertyId.FirstFlag) * 2);
			if ((this.bits & 2U << num) != 0U)
			{
				return new PropertyValue(0U != (this.bits & 1U << num));
			}
			return PropertyValue.Null;
		}

		public void SetPropertyValue(PropertyId id, PropertyValue value)
		{
			if (value.IsBool)
			{
				this.Set(id, value.Bool);
			}
		}

		public bool IsSubsetOf(FlagProperties overrideFlags)
		{
			return 0U == (this.bits & 2863311530U & ~(overrideFlags.bits & 2863311530U));
		}

		public void Merge(FlagProperties overrideFlags)
		{
			this.bits = ((this.bits & ~((overrideFlags.bits & 2863311530U) >> 1)) | overrideFlags.bits);
		}

		public void ReverseMerge(FlagProperties baseFlags)
		{
			this.bits = ((baseFlags.bits & ~((this.bits & 2863311530U) >> 1)) | this.bits);
		}

		public override bool Equals(object obj)
		{
			return obj is FlagProperties && this.bits == ((FlagProperties)obj).bits;
		}

		public override int GetHashCode()
		{
			return (int)this.bits;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(240);
			for (PropertyId propertyId = PropertyId.FirstFlag; propertyId <= PropertyId.MergedCell; propertyId += 1)
			{
				if (this.IsDefined(propertyId))
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(propertyId.ToString());
					stringBuilder.Append(this.IsOn(propertyId) ? ":on" : ":off");
				}
			}
			return stringBuilder.ToString();
		}

		private const uint AllDefinedBits = 2863311530U;

		private const uint AllValueBits = 1431655765U;

		private const uint ValueBit = 1U;

		private const uint DefinedBit = 2U;

		private const uint ValueAndDefinedBits = 3U;

		public static readonly FlagProperties AllUndefined = new FlagProperties(0U);

		public static readonly FlagProperties AllOff = new FlagProperties(0U);

		public static readonly FlagProperties AllOn = new FlagProperties(uint.MaxValue);

		private uint bits;
	}
}
