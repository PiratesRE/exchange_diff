using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal enum RecipientType : byte
	{
		To = 1,
		Cc,
		Bcc,
		P1 = 16,
		SubmittedTo = 129,
		SubmittedCc,
		SubmittedBcc
	}
}
