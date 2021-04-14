using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class TaskEngine
	{
		public static IAsyncResult BeginExecute(ITask task, AsyncCallback asyncCallback, object asyncState)
		{
			IAsyncResult result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				ExecuteAsyncResult executeAsyncResult = new ExecuteAsyncResult((ExecuteAsyncResult asyncResult) => new ExecuteContext(task, asyncResult), asyncCallback, asyncState);
				disposeGuard.Add<ExecuteAsyncResult>(executeAsyncResult);
				executeAsyncResult.ExecuteContext.Begin();
				disposeGuard.Success();
				result = executeAsyncResult;
			}
			return result;
		}

		public static TaskResult EndExecute(IAsyncResult asyncResult)
		{
			ExecuteAsyncResult executeAsyncResult = (ExecuteAsyncResult)asyncResult;
			TaskResult result;
			try
			{
				result = executeAsyncResult.ExecuteContext.End();
			}
			finally
			{
				executeAsyncResult.Dispose();
			}
			return result;
		}
	}
}
