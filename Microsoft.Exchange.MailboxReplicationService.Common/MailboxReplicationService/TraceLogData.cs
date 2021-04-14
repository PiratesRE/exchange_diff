using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal struct TraceLogData
	{
		public string TracerName { get; set; }

		public TraceType TraceType { get; set; }

		public string TraceMessage { get; set; }
	}
}
