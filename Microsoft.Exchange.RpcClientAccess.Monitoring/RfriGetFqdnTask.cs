using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RfriGetFqdnTask : BaseRpcTask
	{
		public RfriGetFqdnTask(IRfriClient rfriClient, IContext context) : base(context, Strings.RfriGetFqdnTaskTitle, Strings.RfriGetFqdnTaskDescription, TaskType.Operation, new ContextProperty[]
		{
			ContextPropertySchema.RpcServerLegacyDN.GetOnly(),
			ContextPropertySchema.RpcServer.SetOnly()
		})
		{
			this.rfriClient = rfriClient;
		}

		protected override IEnumerator<ITask> Process()
		{
			AsyncTask asyncTask = new AsyncTask(base.CreateDerivedContext(), (AsyncCallback asyncCallback, object asyncState) => this.rfriClient.BeginGetFqdnFromLegacyDn(base.Get<string>(ContextPropertySchema.RpcServerLegacyDN), base.Get<TimeSpan>(BaseTask.Timeout), asyncCallback, asyncState), delegate(IAsyncResult asyncResult)
			{
				string value = null;
				RfriCallResult rfriCallResult = this.rfriClient.EndGetFqdnFromLegacyDn(asyncResult, out value);
				if (rfriCallResult.IsSuccessful)
				{
					base.Set<string>(ContextPropertySchema.RpcServer, value);
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
