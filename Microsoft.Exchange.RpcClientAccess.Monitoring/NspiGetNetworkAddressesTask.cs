using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiGetNetworkAddressesTask : BaseTask
	{
		public NspiGetNetworkAddressesTask(INspiClient nspiClient, IContext context) : base(context, Strings.NspiGetNetworkAddressesTaskTitle, Strings.NspiGetNetworkAddressesTaskDescription, TaskType.Operation, new ContextProperty[]
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
				new NspiDNToEphTask(this.nspiClient, base.CreateDerivedContext()),
				new NspiGetNetworkAddressesPropertyTask(this.nspiClient, base.CreateDerivedContext())
			});
			yield return compositeTask;
			base.Result = compositeTask.Result;
			yield break;
		}

		private readonly INspiClient nspiClient;
	}
}
