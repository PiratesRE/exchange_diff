using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public enum MailboxStatus : short
	{
		Invalid,
		New,
		UserAccessible,
		Disabled,
		SoftDeleted,
		HardDeleted,
		Tombstone
	}
}
