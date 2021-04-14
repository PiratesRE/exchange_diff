using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiGetMatchesTask : BaseRpcTask
	{
		public NspiGetMatchesTask(INspiClient nspiClient, IContext context) : base(context, Strings.NspiGetMatchesTaskTitle, Strings.NspiGetMatchesTaskDescription, TaskType.Operation, new ContextProperty[]
		{
			ContextPropertySchema.PrimarySmtpAddress.GetOnly(),
			ContextPropertySchema.NspiMinimalIds.SetOnly()
		})
		{
			this.nspiClient = nspiClient;
		}

		protected override IEnumerator<ITask> Process()
		{
			int[] minimalIds = null;
			AsyncTask asyncGetMatchesTask = new AsyncTask(base.CreateDerivedContext(), (AsyncCallback asyncCallback, object asyncState) => this.nspiClient.BeginGetMatches(base.Get<string>(ContextPropertySchema.PrimarySmtpAddress), base.Get<TimeSpan>(BaseTask.Timeout), asyncCallback, asyncState), (IAsyncResult asyncResult) => this.ApplyCallResult(this.nspiClient.EndGetMatches(asyncResult, out minimalIds)));
			yield return asyncGetMatchesTask;
			base.Result = asyncGetMatchesTask.Result;
			if (base.Result == TaskResult.Success && minimalIds != null)
			{
				base.Set<int[]>(ContextPropertySchema.NspiMinimalIds, minimalIds);
			}
			yield break;
		}

		private readonly INspiClient nspiClient;
	}
}
