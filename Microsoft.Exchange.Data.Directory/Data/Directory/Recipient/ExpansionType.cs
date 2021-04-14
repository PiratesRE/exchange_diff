using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal enum ExpansionType
	{
		None,
		GroupMembership,
		AlternateRecipientForward,
		AlternateRecipientDeliverAndForward,
		ContactChain
	}
}
