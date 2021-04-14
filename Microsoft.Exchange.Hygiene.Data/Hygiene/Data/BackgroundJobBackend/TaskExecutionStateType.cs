using System;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	public enum TaskExecutionStateType : byte
	{
		NotStarted,
		Started,
		Election,
		Inauguration,
		Running,
		Failover,
		Failed,
		Completed
	}
}
