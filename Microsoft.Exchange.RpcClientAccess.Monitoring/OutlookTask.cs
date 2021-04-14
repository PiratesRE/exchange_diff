using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OutlookTask : BaseTask
	{
		public OutlookTask(IContext context) : base(context, Strings.OutlookTaskTitle, Strings.OutlookTaskDescription, TaskType.Knowledge, new ContextProperty[0])
		{
		}

		protected override IEnumerator<ITask> Process()
		{
			ITask task = new CompositeTask(base.CreateDerivedContext(), new ITask[]
			{
				new DiscoverWebProxyTask(base.CreateDerivedContext()),
				new VerifyRpcProxyTask(base.CreateDerivedContext()),
				new NspiTask(base.CreateDerivedContext()),
				new RfriTask(base.CreateDerivedContext()),
				new EmsmdbTask(base.CreateDerivedContext())
			});
			yield return task;
			base.Result = task.Result;
			yield break;
		}
	}
}
