using System;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	[Flags]
	internal enum DeleteMailboxFlags : uint
	{
		None = 0U,
		MailboxMoved = 1U,
		HardDelete = 2U,
		MoveFailed = 4U,
		SoftDelete = 8U,
		RemoveSoftDeleted = 16U,
		AcceptableFlagsMask = 31U
	}
}
