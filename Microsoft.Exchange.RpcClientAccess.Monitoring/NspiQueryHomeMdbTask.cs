using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiQueryHomeMdbTask : BaseTask
	{
		public NspiQueryHomeMdbTask(INspiClient nspiClient, IContext context) : base(context, Strings.NspiQueryHomeMDBTaskTitle, Strings.NspiQueryHomeMDBTaskDescription, TaskType.Operation, new ContextProperty[]
		{
			ContextPropertySchema.NspiMinimalIds.SetOnly()
		})
		{
			this.nspiClient = nspiClient;
		}

		protected override IEnumerator<ITask> Process()
		{
			base.Set<int[]>(ContextPropertySchema.NspiMinimalIds, null);
			BaseTask compositeTask = new CompositeTask(base.CreateDerivedContext(), new ITask[]
			{
				new NspiGetMatchesTask(this.nspiClient, base.CreateDerivedContext()),
				new NspiQueryRowsTask(this.nspiClient, base.CreateDerivedContext())
			});
			yield return compositeTask;
			base.Result = compositeTask.Result;
			yield break;
		}

		private readonly INspiClient nspiClient;
	}
}
