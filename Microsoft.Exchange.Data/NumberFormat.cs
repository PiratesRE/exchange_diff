using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class NumberFormat
	{
		private NumberFormat(string prefix, int suffixLength)
		{
			this.phoneNumberPrefix = prefix;
			this.phoneNumberSuffixLength = suffixLength;
		}

		public string Prefix
		{
			get
			{
				return this.phoneNumberPrefix;
			}
		}

		public int PhoneNumberSuffixLength
		{
			get
			{
				return this.phoneNumberSuffixLength;
			}
		}

		public bool TryMapNumber(string number, out string mappedNumber)
		{
			mappedNumber = null;
			if (number.Length < this.phoneNumberSuffixLength)
			{
				return false;
			}
			string str = number.Substring(number.Length - this.phoneNumberSuffixLength);
			mappedNumber = this.phoneNumberPrefix + str;
			return true;
		}

		public static NumberFormat Parse(string numberFormat)
		{
			if (string.IsNullOrEmpty(numberFormat))
			{
				return null;
			}
			if (numberFormat.Length > 24)
			{
				throw new FormatException(DataStrings.NumberFormatStringTooLong("Number Format", 24, numberFormat.Length));
			}
			int num = numberFormat.IndexOf("x", StringComparison.OrdinalIgnoreCase);
			string text;
			int suffixLength;
			if (num != -1)
			{
				string input = numberFormat.Substring(num);
				if (!NumberFormat.wildcardDigitRegex.IsMatch(input))
				{
					throw new ArgumentException(DataStrings.InvalidNumberFormat(numberFormat));
				}
				text = numberFormat.Substring(0, num);
				suffixLength = numberFormat.Length - text.Length;
			}
			else
			{
				text = numberFormat;
				suffixLength = 0;
			}
			if (!NumberFormat.numberRegex.IsMatch(text))
			{
				throw new ArgumentException(DataStrings.InvalidNumberFormat(numberFormat));
			}
			return new NumberFormat(text, suffixLength);
		}

		public override string ToString()
		{
			string arg = new string('x', this.phoneNumberSuffixLength);
			return string.Format("{0}{1}", this.phoneNumberPrefix, arg);
		}

		public const int MaxLength = 24;

		public const string AllowedCharacters = "[0-9x]";

		private string phoneNumberPrefix;

		private int phoneNumberSuffixLength;

		private static Regex numberRegex = new Regex("^\\d+$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static Regex wildcardDigitRegex = new Regex("x*$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
	}
}
