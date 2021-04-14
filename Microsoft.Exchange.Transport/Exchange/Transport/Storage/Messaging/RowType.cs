using System;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	[Flags]
	internal enum RowType
	{
		Message = 1,
		Recipient = 2,
		All = 3
	}
}
