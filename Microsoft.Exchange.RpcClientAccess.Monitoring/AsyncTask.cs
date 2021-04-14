using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AsyncTask : BaseTask
	{
		public AsyncTask(IContext context, AsyncTask.BeginDelegate beginDelegate, AsyncTask.EndDelegate endDelegate) : base(context, Strings.AsyncTaskTitle, Strings.AsyncTaskDescription, TaskType.Infrastructure, new ContextProperty[0])
		{
			this.beginDelegate = beginDelegate;
			this.endDelegate = endDelegate;
		}

		protected override IEnumerator<ITask> Process()
		{
			IAsyncResult asyncResult = this.beginDelegate(delegate(IAsyncResult result)
			{
				base.Resume();
			}, this);
			yield return null;
			base.Result = this.endDelegate(asyncResult);
			yield break;
		}

		private readonly AsyncTask.BeginDelegate beginDelegate;

		private readonly AsyncTask.EndDelegate endDelegate;

		public delegate IAsyncResult BeginDelegate(AsyncCallback asyncCallback, object asyncState);

		public delegate TaskResult EndDelegate(IAsyncResult asyncResult);
	}
}
