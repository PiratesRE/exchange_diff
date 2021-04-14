using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface IRoutingConfigProvider
	{
		bool CheckDagSelectorHeader { get; }

		bool LocalLoopDetectionEnabled { get; }

		int LocalLoopDetectionSubDomainLeftToRightOffsetForPerfCounter { get; }

		List<int> LocalLoopMessageDeferralIntervals { get; }

		int LocalLoopSubdomainDepth { get; }

		int LoopDetectionNumberOfTransits { get; }
	}
}
