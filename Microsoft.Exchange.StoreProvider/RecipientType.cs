using System;

namespace Microsoft.Mapi
{
	internal enum RecipientType
	{
		Orig,
		To,
		Cc,
		Bcc,
		P1 = 268435456,
		Submitted = -2147483648,
		Unknown = -1
	}
}
