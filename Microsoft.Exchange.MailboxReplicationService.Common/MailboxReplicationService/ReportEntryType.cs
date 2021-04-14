using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public enum ReportEntryType
	{
		Informational,
		Warning,
		WarningCondition,
		Error = 4,
		Debug = 8
	}
}
