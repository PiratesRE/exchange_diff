using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public interface IJobScheduler
	{
		void ScheduleJob(IntegrityCheckJob job);

		void RemoveJob(IntegrityCheckJob job);

		void ExecuteJob(Context context, IntegrityCheckJob job);

		IEnumerable<IntegrityCheckJob> GetReadyJobs(JobPriority priority);
	}
}
