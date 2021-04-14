using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal interface ITask
	{
		IBudget Budget { get; }

		TimeSpan MaxExecutionTime { get; }

		object State { get; set; }

		string Description { get; set; }

		WorkloadSettings WorkloadSettings { get; }

		IActivityScope GetActivityScope();

		TaskExecuteResult Execute(TimeSpan queueAndDelayTime, TimeSpan totalTime);

		void Complete(TimeSpan queueAndDelayTime, TimeSpan totalTime);

		void Cancel();

		void Timeout(TimeSpan queueAndDelayTime, TimeSpan totalTime);

		ResourceKey[] GetResources();

		TaskExecuteResult CancelStep(LocalizedException exception);
	}
}
