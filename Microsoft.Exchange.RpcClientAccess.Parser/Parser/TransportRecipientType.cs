using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	public enum TransportRecipientType
	{
		Orig,
		To,
		Cc,
		Bcc,
		P1 = 268435456,
		SubmittedTo = -2147483647,
		SubmittedCc,
		SubmittedBcc
	}
}
