using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiDNToEphTask : BaseRpcTask
	{
		public NspiDNToEphTask(INspiClient nspiClient, IContext context) : base(context, Strings.NspiDNToEphTaskTitle, Strings.NspiDNToEphTaskDescription, TaskType.Operation, new ContextProperty[]
		{
			ContextPropertySchema.RpcServerLegacyDN.GetOnly(),
			ContextPropertySchema.NspiMinimalIds.SetOnly()
		})
		{
			this.nspiClient = nspiClient;
		}

		protected override IEnumerator<ITask> Process()
		{
			int[] minimalIds = null;
			AsyncTask asyncDNToEphTask = new AsyncTask(base.CreateDerivedContext(), (AsyncCallback asyncCallback, object asyncState) => this.nspiClient.BeginDNToEph(base.Get<string>(ContextPropertySchema.RpcServerLegacyDN), base.Get<TimeSpan>(BaseTask.Timeout), asyncCallback, asyncState), (IAsyncResult asyncResult) => this.ApplyCallResult(this.nspiClient.EndDNToEph(asyncResult, out minimalIds)));
			yield return asyncDNToEphTask;
			base.Result = asyncDNToEphTask.Result;
			if (base.Result == TaskResult.Success && minimalIds != null)
			{
				base.Set<int[]>(ContextPropertySchema.NspiMinimalIds, minimalIds);
			}
			yield break;
		}

		private readonly INspiClient nspiClient;
	}
}
