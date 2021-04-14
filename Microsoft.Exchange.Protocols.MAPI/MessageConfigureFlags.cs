using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	[Flags]
	public enum MessageConfigureFlags
	{
		None = 0,
		CreateNewMessage = 1,
		IsAssociated = 2,
		IsContentAggregation = 4,
		RequestReadOnly = 8,
		RequestWrite = 16,
		IsReportMessage = 32,
		SkipQuotaCheck = 64
	}
}
