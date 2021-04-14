using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class VerifyRpcProxyNlbAffinityTask : BaseTask
	{
		public VerifyRpcProxyNlbAffinityTask(IContext context) : base(context, Strings.VerifyRpcProxyTaskTitle, Strings.VerifyRpcProxyTaskDescription, TaskType.Knowledge, RpcHelper.DependenciesOfBuildRpcProxyOnlyBindingInfo.Concat(new ContextProperty[]
		{
			ContextPropertySchema.Exception.SetOnly(),
			ContextPropertySchema.ErrorDetails.SetOnly()
		}))
		{
		}

		protected override IEnumerator<ITask> Process()
		{
			IVerifyRpcProxyClient rpcProxyClient = base.Environment.CreateVerifyRpcProxyClient(RpcHelper.BuildRpcProxyOnlyBindingInfo(base.Properties));
			AsyncTask asyncTask = new AsyncTask(base.CreateDerivedContext(), (AsyncCallback callback, object asyncState) => rpcProxyClient.BeginVerifyRpcProxy(true, callback, asyncState), delegate(IAsyncResult asyncResult)
			{
				VerifyRpcProxyResult verifyRpcProxyResult = rpcProxyClient.EndVerifyRpcProxy(asyncResult);
				return RpcHelper.FigureOutErrorInformation(this.Properties, verifyRpcProxyResult);
			});
			yield return asyncTask;
			base.Result = asyncTask.Result;
			yield break;
		}
	}
}
