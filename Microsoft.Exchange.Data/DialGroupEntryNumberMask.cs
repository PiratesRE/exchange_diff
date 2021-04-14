using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class DialGroupEntryNumberMask : DialGroupEntryField
	{
		public DialGroupEntryNumberMask(string numberMask) : base(numberMask, "NumberMask")
		{
		}

		public static DialGroupEntryNumberMask Parse(string numberMask)
		{
			return new DialGroupEntryNumberMask(numberMask);
		}

		protected override void Validate()
		{
			base.ValidateNullOrEmpty();
			base.ValidateMaxLength(64);
			base.ValidateRegex(DialGroupEntryNumberMask.numberRegex);
		}

		public const int MaxLength = 64;

		public const string NumberRegexString = "^\\*$|^\\d+\\*$|^\\d+x+$|^x+$|^\\d+$";

		private static Regex numberRegex = new Regex("^\\*$|^\\d+\\*$|^\\d+x+$|^x+$|^\\d+$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
	}
}
