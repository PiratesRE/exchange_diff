using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public interface ITask
	{
		TaskResult Result { get; }

		void Initialize(Action resumeDelegate);

		void OnCompleted();

		IEnumerator<ITask> Process();
	}
}
