using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiQueryRowsTask : BaseNspiRpcTask
	{
		public NspiQueryRowsTask(INspiClient nspiClient, IContext context) : base(context, Strings.NspiQueryRowsTaskTitle, Strings.NspiQueryRowsTaskDescription, TaskType.Operation, new ContextProperty[]
		{
			ContextPropertySchema.NspiMinimalIds.GetOnly(),
			ContextPropertySchema.HomeMdbLegacyDN.SetOnly(),
			ContextPropertySchema.UserLegacyDN.SetOnly()
		})
		{
			this.nspiClient = nspiClient;
		}

		protected override IEnumerator<ITask> Process()
		{
			AsyncTask asyncQueryRowsTask = new AsyncTask(base.CreateDerivedContext(), (AsyncCallback asyncCallback, object asyncState) => this.nspiClient.BeginQueryRows(base.Get<int[]>(ContextPropertySchema.NspiMinimalIds), base.Get<TimeSpan>(BaseTask.Timeout), asyncCallback, asyncState), delegate(IAsyncResult asyncResult)
			{
				string value = null;
				string value2 = null;
				NspiCallResult nspiCallResult = this.nspiClient.EndQueryRows(asyncResult, out value, out value2);
				if (nspiCallResult.IsSuccessful)
				{
					base.Set<string>(ContextPropertySchema.HomeMdbLegacyDN, value);
					base.Set<string>(ContextPropertySchema.UserLegacyDN, value2);
				}
				return base.ApplyCallResult(nspiCallResult);
			});
			yield return asyncQueryRowsTask;
			base.Result = asyncQueryRowsTask.Result;
			yield break;
		}

		private readonly INspiClient nspiClient;
	}
}
