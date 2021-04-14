using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EmsmdbConnectTask : EmsmdbOperationBaseTask
	{
		public EmsmdbConnectTask(IEmsmdbClient emsmdbClient, IContext context) : base(context, Strings.EmsmdbConnectTaskTitle, Strings.EmsmdbConnectTaskDescription, TaskType.Operation, new ContextProperty[]
		{
			ContextPropertySchema.UserLegacyDN.GetOnly(),
			ContextPropertySchema.UseMonitoringContext.GetOnly(),
			ContextPropertySchema.RespondingRpcClientAccessServerVersion.SetOnly(),
			ContextPropertySchema.RequestUrl.SetOnly(),
			ContextPropertySchema.RespondingHttpServer.SetOnly(),
			ContextPropertySchema.RespondingRpcProxyServer.SetOnly()
		})
		{
			this.emsmdbClient = emsmdbClient;
		}

		protected override IEnumerator<ITask> Process()
		{
			AsyncTask asyncTask = new AsyncTask(base.CreateDerivedContext(), (AsyncCallback asyncCallback, object asyncState) => this.emsmdbClient.BeginConnect(base.Get<string>(ContextPropertySchema.UserLegacyDN), base.Get<TimeSpan>(BaseTask.Timeout), base.Get<bool>(ContextPropertySchema.UseMonitoringContext), asyncCallback, asyncState), (IAsyncResult asyncResult) => this.ApplyCallResult(this.emsmdbClient.EndConnect(asyncResult)));
			yield return asyncTask;
			base.Result = asyncTask.Result;
			yield break;
		}

		private TaskResult ApplyCallResult(ConnectCallResult callResult)
		{
			base.Set<string>(ContextPropertySchema.RequestUrl, this.emsmdbClient.GetConnectionUriString());
			if (callResult.ServerVersion != null)
			{
				base.Set<MapiVersion>(ContextPropertySchema.RespondingRpcClientAccessServerVersion, callResult.ServerVersion.Value);
			}
			base.Set<string>(ContextPropertySchema.RespondingHttpServer, callResult.HttpResponseHeaders[Constants.ClientAccessServerHeaderName]);
			base.Set<string>(ContextPropertySchema.RespondingRpcProxyServer, callResult.HttpResponseHeaders[Constants.BackendServerHeaderName]);
			return base.ApplyCallResult(callResult);
		}

		private readonly IEmsmdbClient emsmdbClient;
	}
}
