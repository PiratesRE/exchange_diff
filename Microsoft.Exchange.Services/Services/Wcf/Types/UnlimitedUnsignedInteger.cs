using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[TypeConverter(typeof(SimpleGenericsTypeConverter))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UnlimitedUnsignedInteger : IComparable, IEquatable<UnlimitedUnsignedInteger>, IEquatable<Unlimited<ulong>>, IEquatable<Unlimited<ByteQuantifiedSize>>, IComparable<UnlimitedUnsignedInteger>, IComparable<Unlimited<ulong>>, IComparable<Unlimited<ByteQuantifiedSize>>
	{
		public static string UnlimitedString
		{
			get
			{
				return Unlimited<ulong>.UnlimitedString;
			}
		}

		public UnlimitedUnsignedInteger()
		{
			this.limitedValue = ulong.MaxValue;
			this.IsUnlimited = true;
		}

		public UnlimitedUnsignedInteger(ulong limitedValue)
		{
			this.IsUnlimited = false;
			this.limitedValue = limitedValue;
		}

		public UnlimitedUnsignedInteger(Unlimited<ulong> value)
		{
			this.IsUnlimited = value.IsUnlimited;
			if (!this.IsUnlimited)
			{
				this.limitedValue = value.Value;
			}
		}

		public UnlimitedUnsignedInteger(Unlimited<ByteQuantifiedSize> value)
		{
			this.IsUnlimited = value.IsUnlimited;
			if (!this.IsUnlimited)
			{
				this.limitedValue = value.Value.ToBytes();
			}
		}

		[DataMember]
		public bool IsUnlimited { get; set; }

		[DataMember]
		public ulong Value
		{
			get
			{
				if (this.IsUnlimited)
				{
					return ulong.MaxValue;
				}
				return this.limitedValue;
			}
			set
			{
				this.limitedValue = value;
			}
		}

		public override bool Equals(object other)
		{
			UnlimitedUnsignedInteger unlimitedUnsignedInteger = other as UnlimitedUnsignedInteger;
			if (unlimitedUnsignedInteger != null)
			{
				return this.Equals(unlimitedUnsignedInteger);
			}
			if (other is Unlimited<ByteQuantifiedSize>)
			{
				Unlimited<ByteQuantifiedSize> other2 = (Unlimited<ByteQuantifiedSize>)other;
				return this.Equals(other2);
			}
			if (other is Unlimited<ulong>)
			{
				Unlimited<ulong> other3 = (Unlimited<ulong>)other;
				return this.Equals(other3);
			}
			return false;
		}

		public bool Equals(Unlimited<ByteQuantifiedSize> other)
		{
			return (this.IsUnlimited && other.IsUnlimited) || this.IsUnlimited || this.limitedValue.Equals(other.Value.ToBytes());
		}

		public bool Equals(Unlimited<ulong> other)
		{
			return (this.IsUnlimited && other.IsUnlimited) || this.IsUnlimited || this.limitedValue.Equals(other.Value);
		}

		public bool Equals(UnlimitedUnsignedInteger other)
		{
			return this.IsUnlimited == other.IsUnlimited && (this.IsUnlimited || this.limitedValue.Equals(other.Value));
		}

		public override int GetHashCode()
		{
			if (!this.IsUnlimited)
			{
				return this.Value.GetHashCode();
			}
			return 0;
		}

		public Unlimited<ByteQuantifiedSize> ToUnlimitedByteQuantifiedSize()
		{
			if (this.IsUnlimited)
			{
				return Unlimited<ByteQuantifiedSize>.UnlimitedValue;
			}
			return new Unlimited<ByteQuantifiedSize>(new ByteQuantifiedSize(this.limitedValue));
		}

		public override string ToString()
		{
			if (!this.IsUnlimited)
			{
				return this.limitedValue.ToString(CultureInfo.InvariantCulture);
			}
			return Unlimited<ulong>.UnlimitedString;
		}

		public int CompareTo(Unlimited<ByteQuantifiedSize> other)
		{
			if (this.IsUnlimited)
			{
				if (!other.IsUnlimited)
				{
					return 1;
				}
				return 0;
			}
			else
			{
				if (!other.IsUnlimited)
				{
					return Comparer<ByteQuantifiedSize>.Default.Compare(new ByteQuantifiedSize(this.limitedValue), other.Value);
				}
				return -1;
			}
		}

		public int CompareTo(Unlimited<ulong> other)
		{
			if (this.IsUnlimited)
			{
				if (!other.IsUnlimited)
				{
					return 1;
				}
				return 0;
			}
			else
			{
				if (!other.IsUnlimited)
				{
					return Comparer<ulong>.Default.Compare(this.limitedValue, other.Value);
				}
				return -1;
			}
		}

		public int CompareTo(UnlimitedUnsignedInteger other)
		{
			if (this.IsUnlimited)
			{
				if (!other.IsUnlimited)
				{
					return 1;
				}
				return 0;
			}
			else
			{
				if (!other.IsUnlimited)
				{
					return Comparer<ulong>.Default.Compare(this.limitedValue, other.Value);
				}
				return -1;
			}
		}

		int IComparable.CompareTo(object other)
		{
			if (other == null)
			{
				return 1;
			}
			UnlimitedUnsignedInteger unlimitedUnsignedInteger = other as UnlimitedUnsignedInteger;
			if (unlimitedUnsignedInteger != null)
			{
				return this.CompareTo(unlimitedUnsignedInteger);
			}
			if (other is Unlimited<ByteQuantifiedSize>)
			{
				return this.CompareTo((Unlimited<ByteQuantifiedSize>)other);
			}
			if (other is Unlimited<ulong>)
			{
				return this.CompareTo((Unlimited<ulong>)other);
			}
			throw new ArgumentException(DataStrings.ExceptionObjectInvalid);
		}

		private ulong limitedValue;
	}
}
