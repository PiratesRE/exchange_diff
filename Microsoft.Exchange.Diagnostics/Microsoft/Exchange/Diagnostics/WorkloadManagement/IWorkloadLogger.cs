using System;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal interface IWorkloadLogger
	{
		void LogActivityEvent(IActivityScope activityScope, ActivityEventType eventType);
	}
}
