using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal delegate ExPerformanceCounter GetPerfCounterDelegate(MailboxReplicationServicePerMdbPerformanceCountersInstance instance);
}
