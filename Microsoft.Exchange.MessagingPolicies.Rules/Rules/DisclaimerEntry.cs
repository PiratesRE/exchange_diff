using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal struct DisclaimerEntry
	{
		public string TextHash;

		public string AppendedHtmlText;

		public string AppendedPlainText;

		public string PrependedHtmlText;

		public string PrependedPlainText;

		public int[] ValidCodePages;
	}
}
