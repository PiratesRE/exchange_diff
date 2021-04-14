using System;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Categorizer;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal class LegacyRecipientRecord
	{
		public string Address;

		public RecipientP2Type P2Type;

		public HistoryRecord History;
	}
}
