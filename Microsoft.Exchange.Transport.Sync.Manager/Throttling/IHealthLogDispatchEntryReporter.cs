using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IHealthLogDispatchEntryReporter
	{
		void ReportDispatchAttempt(DispatchEntry dispatchEntry, DispatchTrigger dispatchTrigger, WorkType? workType, DispatchResult dispatchResult, ISubscriptionInformation subscriptionInformation, ExDateTime? lastDispatchTime);
	}
}
