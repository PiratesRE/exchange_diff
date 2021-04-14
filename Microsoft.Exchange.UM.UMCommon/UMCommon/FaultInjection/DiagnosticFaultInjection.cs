using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon.FaultInjection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DiagnosticFaultInjection
	{
		internal static bool TryCreateException(string exceptionType, ref Exception exception)
		{
			return false;
		}

		public const uint MaxCallReceivedLid = 3676712253U;

		public const uint NoAvailableDiskspaceLid = 2871405885U;

		public const uint ParseSipUri = 2602970429U;

		public const uint TryFindHuntgroupWhenNoPilotIdentifier = 3945147709U;

		public const uint TryMapCallToWorkerProcess = 2334534973U;

		public const uint IsCallToDifferentForest = 3408276797U;

		public const uint MonitoringDelayInCallRouter = 3062246717U;

		public const uint MonitoringDelayInUMService = 4135988541U;

		public const uint MonitoringDelayInUMWorkerProcess = 2525375805U;
	}
}
