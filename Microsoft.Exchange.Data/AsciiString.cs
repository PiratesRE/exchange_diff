using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class AsciiString : IComparable, IComparable<AsciiString>, IEquatable<AsciiString>
	{
		public AsciiString()
		{
			this.stringValue = string.Empty;
		}

		public AsciiString(string value)
		{
			if (value == null)
			{
				throw new ArgumentException("value");
			}
			if (!AsciiString.IsStringArgumentAscii(value))
			{
				throw new ArgumentException(DataStrings.ArgumentMustBeAscii);
			}
			this.stringValue = value;
		}

		internal static bool IsStringArgumentAscii(string value)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] >= '\u0080')
				{
					return false;
				}
			}
			return true;
		}

		public static AsciiString Parse(string value)
		{
			return new AsciiString(value);
		}

		public static bool TryParse(string value, out AsciiString asciiString)
		{
			bool result = false;
			asciiString = new AsciiString();
			if (value != null && AsciiString.IsStringArgumentAscii(value))
			{
				asciiString.stringValue = value;
				result = true;
			}
			return result;
		}

		public override string ToString()
		{
			if (this.stringValue == null)
			{
				return string.Empty;
			}
			return this.stringValue;
		}

		public static bool operator ==(AsciiString value1, AsciiString value2)
		{
			return 0 == value1.CompareTo(value2);
		}

		public static bool operator !=(AsciiString value1, AsciiString value2)
		{
			return !(value1 == value2);
		}

		public static implicit operator string(AsciiString asciiString)
		{
			return asciiString.stringValue;
		}

		public static explicit operator AsciiString(string value)
		{
			return new AsciiString(value);
		}

		public int CompareTo(AsciiString value)
		{
			return string.Compare(this.stringValue, value.stringValue, StringComparison.OrdinalIgnoreCase);
		}

		public int CompareTo(object value)
		{
			if (value is AsciiString)
			{
				return this.CompareTo((AsciiString)value);
			}
			string text = value as string;
			if (text != null)
			{
				return string.Compare(this.stringValue, text, StringComparison.OrdinalIgnoreCase);
			}
			throw new ArgumentException("Object is not an AsciiString");
		}

		public override bool Equals(object value)
		{
			return 0 == this.CompareTo(value);
		}

		public bool Equals(AsciiString value)
		{
			return 0 == this.CompareTo(value);
		}

		public override int GetHashCode()
		{
			if (this.stringValue == null)
			{
				return 0;
			}
			return this.stringValue.GetHashCode();
		}

		private string stringValue;

		public static AsciiString Empty = new AsciiString();
	}
}
