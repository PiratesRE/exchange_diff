using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RfriTask : BaseTask
	{
		public RfriTask(IContext context) : base(context, Strings.RfriTaskTitle, Strings.RfriTaskDescription, TaskType.Knowledge, RpcHelper.DependenciesOfBuildCompleteBindingInfo.Concat(new ContextProperty[]
		{
			ContextPropertySchema.ActualBinding.SetOnly()
		}))
		{
		}

		protected override IEnumerator<ITask> Process()
		{
			using (IRfriClient rfriClient = base.Environment.CreateRfriClient(RpcHelper.BuildCompleteBindingInfo(base.Properties, 6002)))
			{
				base.Set<string>(ContextPropertySchema.ActualBinding, rfriClient.BindingString);
				BaseTask composite = new CompositeTask(base.CreateDerivedContext(), new ITask[]
				{
					new RfriGetNewDsaTask(rfriClient, base.CreateDerivedContext()),
					new RfriGetFqdnTask(rfriClient, base.CreateDerivedContext())
				});
				yield return composite;
				base.Result = composite.Result;
			}
			yield break;
		}
	}
}
