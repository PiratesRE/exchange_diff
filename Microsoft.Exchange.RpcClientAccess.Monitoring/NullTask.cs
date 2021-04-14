using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NullTask : ITask
	{
		public TaskResult Result
		{
			get
			{
				return TaskResult.Success;
			}
		}

		public void Initialize(Action resumeDelegate)
		{
		}

		public void OnCompleted()
		{
		}

		public IEnumerator<ITask> Process()
		{
			yield break;
		}
	}
}
