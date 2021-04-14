using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ContactLinkingSuggestionMatching
	{
		private ContactLinkingSuggestionMatching(double minimumNameMatchPercentage, int minimumNameLengthForPartialCompare)
		{
			this.minimumNameMatchPercentage = minimumNameMatchPercentage;
			this.minimumNameLengthForPartialCompare = minimumNameLengthForPartialCompare;
		}

		internal bool IsFullMatch(CultureInfo culture, string a, string b)
		{
			return !string.IsNullOrEmpty(a) && !string.IsNullOrEmpty(b) && 0 == culture.CompareInfo.Compare(a, b, CompareOptions.IgnoreCase);
		}

		internal int GetPartialMatchCount(CultureInfo culture, string a, string b)
		{
			if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
			{
				return 0;
			}
			int num = Math.Min(a.Length, b.Length);
			if (num < this.minimumNameLengthForPartialCompare)
			{
				return 0;
			}
			int num2 = 0;
			int num3 = 0;
			while (num3 < num && culture.CompareInfo.Compare(a, num3, 1, b, num3, 1, CompareOptions.IgnoreCase) == 0)
			{
				num2++;
				num3++;
			}
			int num4 = 0;
			int num5 = 0;
			while (num5 < num && culture.CompareInfo.Compare(a, a.Length - 1 - num5, 1, b, b.Length - 1 - num5, 1, CompareOptions.IgnoreCase) == 0)
			{
				num4++;
				num5++;
			}
			int num6 = (int)Math.Ceiling((double)num * this.minimumNameMatchPercentage);
			int num7 = Math.Max(num2, num4);
			if (num7 < num6)
			{
				return 0;
			}
			return num7;
		}

		private readonly double minimumNameMatchPercentage;

		private readonly int minimumNameLengthForPartialCompare;

		internal static readonly ContactLinkingSuggestionMatching FirstOrLastName = new ContactLinkingSuggestionMatching(0.5, 5);

		internal static readonly ContactLinkingSuggestionMatching AliasOrEmailAddress = new ContactLinkingSuggestionMatching(0.75, 6);
	}
}
