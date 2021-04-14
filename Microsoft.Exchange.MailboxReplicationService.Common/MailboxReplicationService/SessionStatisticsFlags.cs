using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	public enum SessionStatisticsFlags
	{
		None = 0,
		ContentIndexingWordBreaking = 1,
		MapiDiagnosticGetProp = 2
	}
}
