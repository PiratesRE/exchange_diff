using System;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public enum RestartRequestReason
	{
		DataAccessError,
		PoisonResult,
		Maintenance,
		Unknown,
		SelfHealing
	}
}
