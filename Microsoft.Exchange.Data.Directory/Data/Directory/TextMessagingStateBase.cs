using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class TextMessagingStateBase : IEquatable<TextMessagingStateBase>
	{
		public TextMessagingStateBase(int rawValue)
		{
			this.RawValue = rawValue;
		}

		public static TextMessagingStateBase ParseFromADString(string valueString)
		{
			if (string.IsNullOrEmpty(valueString))
			{
				throw new ArgumentNullException("valueString");
			}
			return TextMessagingStateBase.FromRawInteger32(int.Parse(valueString));
		}

		public static TextMessagingStateBase FromRawInteger32(int rawValue)
		{
			if ((-2147483648 & rawValue) == 0)
			{
				return new TextMessagingDeliveryPointState(rawValue);
			}
			return new ReservedTextMessagingState(rawValue);
		}

		internal int RawValue { get; set; }

		public string ToADString()
		{
			return this.RawValue.ToString();
		}

		public bool Equals(TextMessagingStateBase other)
		{
			return !object.ReferenceEquals(null, other) && this.RawValue.Equals(other.RawValue);
		}

		internal const int StartBitReserved = 31;

		internal const int MaskReserved = -2147483648;
	}
}
