using System;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	public enum RuleScope : byte
	{
		None,
		ContentFilter,
		ProtocolFilterFrontdoor,
		ProtocolFilterHub,
		MailboxDeliveryFilter
	}
}
