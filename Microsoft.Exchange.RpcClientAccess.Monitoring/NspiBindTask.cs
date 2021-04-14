using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiBindTask : BaseNspiRpcTask
	{
		public NspiBindTask(INspiClient nspiClient, IContext context) : base(context, Strings.NspiBindTaskTitle, Strings.NspiBindTaskDescription, TaskType.Operation, new ContextProperty[0])
		{
			this.nspiClient = nspiClient;
		}

		protected override IEnumerator<ITask> Process()
		{
			AsyncTask asyncTask = new AsyncTask(base.CreateDerivedContext(), (AsyncCallback asyncCallback, object asyncState) => this.nspiClient.BeginBind(base.Get<TimeSpan>(BaseTask.Timeout), asyncCallback, asyncState), (IAsyncResult asyncResult) => base.ApplyCallResult(this.nspiClient.EndBind(asyncResult)));
			yield return asyncTask;
			base.Result = asyncTask.Result;
			yield break;
		}

		private readonly INspiClient nspiClient;
	}
}
