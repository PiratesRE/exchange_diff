using System;

namespace Microsoft.Exchange.Security
{
	public class SanitizedStringBase<SanitizingPolicy> : ISanitizedString<SanitizingPolicy>, IComparable, IComparable<ISanitizedString<SanitizingPolicy>>, IComparable<string>, IEquatable<ISanitizedString<SanitizingPolicy>>, IEquatable<string> where SanitizingPolicy : ISanitizingPolicy, new()
	{
		public SanitizedStringBase() : this(string.Empty)
		{
		}

		public SanitizedStringBase(string value)
		{
			if (value == null)
			{
				this.containedString = string.Empty;
			}
			else
			{
				this.containedString = value;
			}
			this.isContainedStringTrusted = (this.containedString.Length == 0);
		}

		public string UntrustedValue
		{
			get
			{
				if (this.isContainedStringTrusted)
				{
					return null;
				}
				return this.containedString;
			}
			set
			{
				if (value == null)
				{
					this.containedString = string.Empty;
				}
				else
				{
					this.containedString = value;
				}
				this.isContainedStringTrusted = (value.Length == 0);
			}
		}

		protected string TrustedValue
		{
			get
			{
				if (this.isContainedStringTrusted)
				{
					return this.containedString;
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.containedString = string.Empty;
				}
				else
				{
					this.containedString = value;
				}
				this.isContainedStringTrusted = true;
			}
		}

		public static bool IsNullOrEmpty(SanitizedStringBase<SanitizingPolicy> str)
		{
			return str == null || str.containedString.Length == 0;
		}

		public sealed override string ToString()
		{
			if (!this.isContainedStringTrusted)
			{
				if (!StringSanitizer<SanitizingPolicy>.IsTrustedString(this.containedString))
				{
					this.containedString = this.Sanitize(this.containedString);
				}
				this.isContainedStringTrusted = true;
			}
			return this.containedString;
		}

		public void DecreeToBeTrusted()
		{
			this.isContainedStringTrusted = true;
		}

		public void DecreeToBeUntrusted()
		{
			this.isContainedStringTrusted = false;
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return int.MaxValue;
			}
			if (obj is ISanitizedString<SanitizingPolicy>)
			{
				ISanitizedString<SanitizingPolicy> sanitizedString = (ISanitizedString<SanitizingPolicy>)obj;
				return this.ToString().CompareTo(sanitizedString.ToString());
			}
			if (obj is string)
			{
				return this.ToString().CompareTo((string)obj);
			}
			throw new ArgumentException("Object is neither a string nor an ISanitizedString<" + typeof(SanitizingPolicy) + ">");
		}

		public int CompareTo(ISanitizedString<SanitizingPolicy> other)
		{
			if (other == null)
			{
				return int.MaxValue;
			}
			return this.ToString().CompareTo(other.ToString());
		}

		public int CompareTo(string other)
		{
			if (other == null)
			{
				return int.MaxValue;
			}
			return this.ToString().CompareTo(other);
		}

		public bool Equals(ISanitizedString<SanitizingPolicy> other)
		{
			return other != null && this.ToString().Equals(other.ToString());
		}

		public bool Equals(string other)
		{
			return other != null && this.ToString().Equals(other);
		}

		public bool Equals(ISanitizedString<SanitizingPolicy> value, StringComparison comparisonType)
		{
			return value != null && this.ToString().Equals(value.ToString(), comparisonType);
		}

		public bool Equals(string value, StringComparison comparisonType)
		{
			return value != null && this.ToString().Equals(value, comparisonType);
		}

		public sealed override bool Equals(object obj)
		{
			return obj != null && this.ToString().Equals(obj.ToString());
		}

		public sealed override int GetHashCode()
		{
			return this.containedString.GetHashCode();
		}

		protected virtual string Sanitize(string rawValue)
		{
			return StringSanitizer<SanitizingPolicy>.Sanitize(rawValue);
		}

		private string containedString;

		private bool isContainedStringTrusted;
	}
}
