using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MdnHeaderType
	{
		public const string OriginalRecipient = "Original-recipient";

		public const string FinalRecipient = "Final-recipient";

		public const string Disposition = "Disposition";

		public const string OriginalMessageId = "Original-Message-ID";

		public const string CorrelationKey = "X-MSExch-Correlation-Key";

		public const string DisplayName = "X-Display-Name";
	}
}
