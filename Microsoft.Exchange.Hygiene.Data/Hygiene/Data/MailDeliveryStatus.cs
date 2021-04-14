using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	public enum MailDeliveryStatus : short
	{
		None,
		Delivered,
		Pending,
		Failed,
		Expanded,
		Resolved
	}
}
