using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface IMailboxListRestriction
	{
		SearchCriteria Filter(Context context);

		Index Index(MailboxTable mailboxTable);
	}
}
