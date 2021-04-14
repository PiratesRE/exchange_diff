using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiGetHierarchyInfoTask : BaseNspiRpcTask
	{
		public NspiGetHierarchyInfoTask(INspiClient nspiClient, IContext context) : base(context, Strings.NspiGetHierarchyInfoTaskTitle, Strings.NspiGetHierarchyInfoTaskDescription, TaskType.Operation, new ContextProperty[0])
		{
			this.nspiClient = nspiClient;
		}

		protected override IEnumerator<ITask> Process()
		{
			AsyncTask asyncTask = new AsyncTask(base.CreateDerivedContext(), (AsyncCallback asyncCallback, object asyncState) => this.nspiClient.BeginGetHierarchyInfo(base.Get<TimeSpan>(BaseTask.Timeout), asyncCallback, asyncState), delegate(IAsyncResult asyncResult)
			{
				int num = -1;
				return base.ApplyCallResult(this.nspiClient.EndGetHierarchyInfo(asyncResult, out num));
			});
			yield return asyncTask;
			base.Result = asyncTask.Result;
			yield break;
		}

		private readonly INspiClient nspiClient;
	}
}
