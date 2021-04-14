using System;
using System.Collections.Generic;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxLoadBalance.Providers
{
	internal interface IStorePort
	{
		IEnumerable<MailboxTableEntry> GetMailboxTable(DirectoryDatabase database, Guid mailboxGuid, PropTag[] propertiesToLoad);

		DatabaseSizeInfo GetDatabaseSize(DirectoryDatabase database);
	}
}
