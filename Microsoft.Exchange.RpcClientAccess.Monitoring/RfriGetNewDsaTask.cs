using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RfriGetNewDsaTask : BaseRpcTask
	{
		public RfriGetNewDsaTask(IRfriClient rfriClient, IContext context) : base(context, Strings.RfriGetNewDsaTaskTitle, Strings.RfriGetNewDsaTaskDescription, TaskType.Operation, new ContextProperty[]
		{
			ContextPropertySchema.UserLegacyDN.GetOnly(),
			ContextPropertySchema.DirectoryServer.SetOnly()
		})
		{
			this.rfriClient = rfriClient;
		}

		protected override IEnumerator<ITask> Process()
		{
			AsyncTask asyncTask = new AsyncTask(base.CreateDerivedContext(), (AsyncCallback asyncCallback, object asyncState) => this.rfriClient.BeginGetNewDsa(base.Get<string>(ContextPropertySchema.UserLegacyDN), base.Get<TimeSpan>(BaseTask.Timeout), asyncCallback, asyncState), delegate(IAsyncResult asyncResult)
			{
				string value;
				RfriCallResult rfriCallResult = this.rfriClient.EndGetNewDsa(asyncResult, out value);
				if (rfriCallResult.IsSuccessful)
				{
					base.Set<string>(ContextPropertySchema.DirectoryServer, value);
				}
				return base.ApplyCallResult(rfriCallResult);
			});
			yield return asyncTask;
			base.Result = asyncTask.Result;
			yield break;
		}

		private readonly IRfriClient rfriClient;
	}
}
