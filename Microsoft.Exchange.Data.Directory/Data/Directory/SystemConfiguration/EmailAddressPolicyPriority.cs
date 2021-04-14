using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public struct EmailAddressPolicyPriority : IComparable, IComparable<EmailAddressPolicyPriority>, IEquatable<EmailAddressPolicyPriority>
	{
		public EmailAddressPolicyPriority(int priority)
		{
			this.priority = priority;
		}

		public EmailAddressPolicyPriority(string priority)
		{
			if (!string.IsNullOrEmpty(priority))
			{
				priority.Trim();
			}
			if (string.Equals(DirectoryStrings.EmailAddressPolicyPriorityLowest, priority, StringComparison.OrdinalIgnoreCase) || string.Equals("Lowest", priority, StringComparison.OrdinalIgnoreCase))
			{
				this.priority = EmailAddressPolicyPriority.Lowest.priority;
				return;
			}
			if (!int.TryParse(priority, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, null, out this.priority))
			{
				throw new FormatException(DirectoryStrings.EmailAddressPolicyPriorityLowestFormatError(priority));
			}
		}

		public static EmailAddressPolicyPriority Parse(string priority)
		{
			return new EmailAddressPolicyPriority(priority);
		}

		public static implicit operator int(EmailAddressPolicyPriority value)
		{
			return value.priority;
		}

		public static explicit operator EmailAddressPolicyPriority(int value)
		{
			return new EmailAddressPolicyPriority(value);
		}

		public static bool operator ==(EmailAddressPolicyPriority a, EmailAddressPolicyPriority b)
		{
			return a.priority == b.priority;
		}

		public static bool operator !=(EmailAddressPolicyPriority a, EmailAddressPolicyPriority b)
		{
			return a.priority != b.priority;
		}

		public override string ToString()
		{
			if (this.Equals(EmailAddressPolicyPriority.Lowest))
			{
				return DirectoryStrings.EmailAddressPolicyPriorityLowest;
			}
			return this.priority.ToString();
		}

		public override int GetHashCode()
		{
			return this.priority.GetHashCode();
		}

		public int CompareTo(EmailAddressPolicyPriority value)
		{
			return this.priority.CompareTo(value.priority);
		}

		public int CompareTo(object value)
		{
			if (!(value is EmailAddressPolicyPriority))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "object must be of type {0}", new object[]
				{
					base.GetType().Name
				}));
			}
			return this.priority.CompareTo(((EmailAddressPolicyPriority)value).priority);
		}

		public bool Equals(EmailAddressPolicyPriority value)
		{
			return this.priority.Equals(value.priority);
		}

		public override bool Equals(object value)
		{
			if (!(value is EmailAddressPolicyPriority))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "object must be of type {0}", new object[]
				{
					base.GetType().Name
				}));
			}
			return this.priority.Equals(((EmailAddressPolicyPriority)value).priority);
		}

		public int ToInt32()
		{
			return this.priority;
		}

		public const int MaxLength = 10;

		public const string AllowedCharacters = "[0-9]";

		private int priority;

		public static readonly int HighestPriorityValue = 1;

		public static readonly int LenientHighestPriorityValue = 0;

		public static readonly int LowestPriorityValue = int.MaxValue;

		public static readonly EmailAddressPolicyPriority Highest = new EmailAddressPolicyPriority(EmailAddressPolicyPriority.HighestPriorityValue);

		public static readonly EmailAddressPolicyPriority Lowest = new EmailAddressPolicyPriority(EmailAddressPolicyPriority.LowestPriorityValue);
	}
}
