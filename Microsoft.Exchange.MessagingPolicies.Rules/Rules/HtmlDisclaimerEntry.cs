using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal struct HtmlDisclaimerEntry
	{
		public string TextHash;

		public string[] HtmlTextSegments;

		public string[] PlainTextSegments;

		public int[] ValidCodePages;
	}
}
