using System;

namespace Microsoft.Exchange.Data.Mapi
{
	[Flags]
	public enum ObjectClass
	{
		Unknown = 0,
		Mailbox = 1,
		Gateway = 2,
		DistributionGroup = 4,
		PublicFolder = 8,
		UserDisabled = 16,
		ExOleDbSystemMailbox = 32,
		SystemAttendantMailbox = 64
	}
}
