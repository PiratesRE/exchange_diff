using System;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface ITask
	{
		string Name { get; }

		int Weight { get; }

		bool CheckPrereqs(ITaskContext taskContext);

		bool NeedsConfiguration(ITaskContext taskContext);

		bool Configure(ITaskContext taskContext);

		bool ValidateConfiguration(ITaskContext taskContext);
	}
}
