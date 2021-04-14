using System;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public enum ResultType
	{
		Abandoned,
		TimedOut,
		Poisoned,
		Succeeded,
		Failed,
		Quarantined,
		Rejected
	}
}
