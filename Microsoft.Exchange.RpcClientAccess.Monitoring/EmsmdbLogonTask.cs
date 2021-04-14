using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EmsmdbLogonTask : EmsmdbOperationBaseTask
	{
		public EmsmdbLogonTask(IEmsmdbClient emsmdbClient, IContext context) : base(context, Strings.EmsmdbLogonTaskTitle, Strings.EmsmdbLogonTaskDescription, TaskType.Operation, new ContextProperty[]
		{
			ContextPropertySchema.MailboxLegacyDN.GetOnly()
		})
		{
			this.emsmdbClient = emsmdbClient;
		}

		protected override IEnumerator<ITask> Process()
		{
			AsyncTask asyncTask = new AsyncTask(base.CreateDerivedContext(), (AsyncCallback asyncCallback, object asyncState) => this.emsmdbClient.BeginLogon(base.Get<string>(ContextPropertySchema.MailboxLegacyDN), base.Get<TimeSpan>(BaseTask.Timeout), asyncCallback, asyncState), (IAsyncResult asyncResult) => this.ApplyCallResult(this.emsmdbClient.EndLogon(asyncResult)));
			yield return asyncTask;
			base.Result = asyncTask.Result;
			yield break;
		}

		private TaskResult ApplyCallResult(LogonCallResult callResult)
		{
			if (callResult.LogonErrorCode != ErrorCode.None)
			{
				base.Set<RopExecutionException>(ContextPropertySchema.Exception, new RopExecutionException(Strings.RpcCallResultErrorCodeDescription(callResult.GetType().Name), callResult.LogonErrorCode));
			}
			return base.ApplyCallResult(callResult);
		}

		private readonly IEmsmdbClient emsmdbClient;
	}
}
