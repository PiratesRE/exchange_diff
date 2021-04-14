using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class DummyTask : BaseRpcTask
	{
		public DummyTask(IContext context, params ContextProperty[] dependentProperties) : base(context, Strings.DummyTaskTitle, Strings.DummyTaskDescription, TaskType.Operation, dependentProperties.Concat(new ContextProperty[]
		{
			ContextPropertySchema.ActualBinding.SetOnly()
		}))
		{
		}

		protected abstract IEmsmdbClient CreateEmsmdbClient();

		protected override IEnumerator<ITask> Process()
		{
			using (IEmsmdbClient dummyClient = this.CreateEmsmdbClient())
			{
				AsyncTask asyncTask = new AsyncTask(base.CreateDerivedContext(), (AsyncCallback asyncCallback, object asyncState) => dummyClient.BeginDummy(this.Get<TimeSpan>(BaseTask.Timeout), asyncCallback, asyncState), delegate(IAsyncResult asyncResult)
				{
					DummyCallResult callResult = dummyClient.EndDummy(asyncResult);
					return this.ApplyCallResult(callResult);
				});
				yield return asyncTask;
				base.Result = asyncTask.Result;
			}
			yield break;
		}
	}
}
