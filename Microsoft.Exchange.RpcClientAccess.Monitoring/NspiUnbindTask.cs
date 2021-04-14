using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiUnbindTask : BaseNspiRpcTask
	{
		public NspiUnbindTask(INspiClient nspiClient, IContext context) : base(context, Strings.NspiUnbindTaskTitle, Strings.NspiUnbindTaskDescription, TaskType.Operation, new ContextProperty[0])
		{
			this.nspiClient = nspiClient;
		}

		protected override IEnumerator<ITask> Process()
		{
			AsyncTask asyncTask = new AsyncTask(base.CreateDerivedContext(), (AsyncCallback asyncCallback, object asyncState) => this.nspiClient.BeginUnbind(base.Get<TimeSpan>(BaseTask.Timeout), asyncCallback, asyncState), (IAsyncResult asyncResult) => base.ApplyCallResult(this.nspiClient.EndUnbind(asyncResult)));
			yield return asyncTask;
			base.Result = asyncTask.Result;
			yield break;
		}

		private readonly INspiClient nspiClient;
	}
}
