using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class DialGroupEntryDialedNumber : DialGroupEntryField
	{
		public DialGroupEntryDialedNumber(string dialedNumber) : base(dialedNumber, "DialedNumber")
		{
		}

		public static DialGroupEntryDialedNumber Parse(string dialedNumber)
		{
			return new DialGroupEntryDialedNumber(dialedNumber);
		}

		protected override void Validate()
		{
			base.ValidateNullOrEmpty();
			base.ValidateMaxLength(64);
			base.ValidateRegex(DialGroupEntryDialedNumber.dialedNumberRegex);
		}

		public const int MaxLength = 64;

		public const string DialedNumberRegexString = "^\\+?(\\*$|\\d+\\*$|\\d+x+$|x+$|\\d+$)";

		private static Regex dialedNumberRegex = new Regex("^\\+?(\\*$|\\d+\\*$|\\d+x+$|x+$|\\d+$)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
	}
}
