using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public interface ITaskModuleFactory
	{
		IEnumerable<ITaskModule> Create(TaskContext context);
	}
}
