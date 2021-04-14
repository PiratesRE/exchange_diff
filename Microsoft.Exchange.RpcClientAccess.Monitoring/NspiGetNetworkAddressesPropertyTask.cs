using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiGetNetworkAddressesPropertyTask : BaseNspiRpcTask
	{
		public NspiGetNetworkAddressesPropertyTask(INspiClient nspiClient, IContext context) : base(context, Strings.NspiGetNetworkAddressesPropertyTaskTitle, Strings.NspiGetNetworkAddressesPropertyTaskDescription, TaskType.Operation, new ContextProperty[]
		{
			ContextPropertySchema.NspiMinimalIds.GetOnly()
		})
		{
			this.nspiClient = nspiClient;
		}

		protected override IEnumerator<ITask> Process()
		{
			string[] networkAddresses = null;
			AsyncTask asyncGetNetworkAddressesPropertyTask = new AsyncTask(base.CreateDerivedContext(), (AsyncCallback asyncCallback, object asyncState) => this.nspiClient.BeginGetNetworkAddresses(base.Get<int[]>(ContextPropertySchema.NspiMinimalIds), base.Get<TimeSpan>(BaseTask.Timeout), asyncCallback, asyncState), (IAsyncResult asyncResult) => this.ApplyCallResult(this.nspiClient.EndGetNetworkAddresses(asyncResult, out networkAddresses)));
			yield return asyncGetNetworkAddressesPropertyTask;
			base.Result = asyncGetNetworkAddressesPropertyTask.Result;
			yield break;
		}

		private readonly INspiClient nspiClient;
	}
}
