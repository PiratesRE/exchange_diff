using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public enum CriticalBlockScope
	{
		None,
		StoreObject,
		MailboxSession,
		MailboxShared,
		Database
	}
}
