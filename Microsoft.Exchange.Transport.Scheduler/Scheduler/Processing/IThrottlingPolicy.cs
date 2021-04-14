using System;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal interface IThrottlingPolicy
	{
		PolicyDecision Evaluate(IMessageScope scope, UsageData usage, UsageData totalUsage);
	}
}
