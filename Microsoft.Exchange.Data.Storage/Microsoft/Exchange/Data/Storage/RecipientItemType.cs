using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum RecipientItemType
	{
		To = 1,
		Cc,
		Bcc,
		P1 = 268435456,
		Unknown = -1
	}
}
