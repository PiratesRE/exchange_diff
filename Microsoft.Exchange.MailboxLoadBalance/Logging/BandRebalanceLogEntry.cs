using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Logging
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BandRebalanceLogEntry
	{
		public string SourceDatabase { get; set; }

		public string TargetDatabase { get; set; }

		public long RebalanceUnits { get; set; }

		public string BatchName { get; set; }

		public string Metric { get; set; }
	}
}
