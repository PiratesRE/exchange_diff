using System;
using System.Collections.Generic;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Transport.Probes
{
	internal interface IMailboxProvider
	{
		MailboxSelectionResult TryGetMailboxToUse(out Guid mbxGuid, out Guid mdbGuid, out string emailAddress);

		MailboxDatabaseSelectionResult GetAllMailboxDatabaseInfo(out ICollection<MailboxDatabaseInfo> mailboxDatabases);
	}
}
