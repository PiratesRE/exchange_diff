using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	[Flags]
	public enum RecipientType
	{
		Orig = 0,
		To = 1,
		Cc = 2,
		Bcc = 3,
		P1 = 268435456,
		Submitted = -2147483648,
		SubmittedTo = -2147483647,
		SubmittedCc = -2147483646,
		SubmittedBcc = -2147483645,
		Unknown = -1
	}
}
