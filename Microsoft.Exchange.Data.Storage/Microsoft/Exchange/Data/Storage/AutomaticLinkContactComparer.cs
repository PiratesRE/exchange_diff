using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class AutomaticLinkContactComparer
	{
		internal abstract ContactLinkingOperation Match(IContactLinkingMatchProperties contact1, IContactLinkingMatchProperties contact2);

		protected bool MatchEmails(IContactLinkingMatchProperties contact1, IContactLinkingMatchProperties contact2)
		{
			return contact1.EmailAddresses.Overlaps(contact2.EmailAddresses);
		}

		protected bool EqualsIgnoreCaseAndWhiteSpace(string s1, string s2)
		{
			return !string.IsNullOrWhiteSpace(s1) && !string.IsNullOrWhiteSpace(s2) && string.Equals(s1.Trim(), s2.Trim(), StringComparison.OrdinalIgnoreCase);
		}

		private const CompareOptions IgnoreCaseWhiteSpaceAndDiacriticsCompareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;
	}
}
