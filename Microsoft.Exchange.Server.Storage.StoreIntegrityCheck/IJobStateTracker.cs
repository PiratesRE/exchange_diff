using System;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public interface IJobStateTracker
	{
		void MoveToState(JobState state);
	}
}
