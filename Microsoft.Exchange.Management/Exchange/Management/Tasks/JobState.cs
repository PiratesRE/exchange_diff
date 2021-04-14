using System;

namespace Microsoft.Exchange.Management.Tasks
{
	public enum JobState
	{
		Initializing,
		Queued,
		Running,
		Succeeded,
		Failed
	}
}
