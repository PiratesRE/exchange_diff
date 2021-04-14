using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface IMailboxMaintenance
	{
		bool MarkForMaintenance(Context context, MailboxState mailboxState);
	}
}
