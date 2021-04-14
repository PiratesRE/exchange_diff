using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IWorkflow
	{
		IEnumerable<ITask> Tasks { get; }

		int PercentCompleted { get; }

		void Initialize();

		void UpdateProgress(ITask task);
	}
}
