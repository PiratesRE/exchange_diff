using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface IMailboxQuarantineProvider
	{
		void PrequarantineMailbox(Guid databaseGuid, Guid mailboxGuid, string reason);

		void UnquarantineMailbox(Guid databaseGuid, Guid mailboxGuid);

		List<PrequarantinedMailbox> GetPreQuarantinedMailboxes(Guid databaseGuid);

		bool IsMigrationAccessAllowed(Guid databaseGuid, Guid mailboxGuid);
	}
}
