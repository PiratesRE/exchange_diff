using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class TplTaskEngine
	{
		public static Task BeginExecute(ITask task, CancellationToken cancellationToken)
		{
			return Task.Factory.FromAsync<TaskResult>(TaskEngine.BeginExecute(task, null, null), (IAsyncResult asyncResult) => TaskEngine.EndExecute(asyncResult), TaskCreationOptions.AttachedToParent);
		}
	}
}
