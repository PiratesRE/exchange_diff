using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class SetErrorExecutionContextModule : ITaskModule, ICriticalFeature
	{
		private IErrorExecutionContext ErrorExecutionContext
		{
			get
			{
				if (SetErrorExecutionContextModule.executionContext == null)
				{
					lock (SetErrorExecutionContextModule.syncObject)
					{
						if (SetErrorExecutionContextModule.executionContext == null)
						{
							SetErrorExecutionContextModule.executionContext = new ErrorExecutionContext(this.context.InvocationInfo.ShellHostName);
						}
					}
				}
				return SetErrorExecutionContextModule.executionContext;
			}
		}

		public SetErrorExecutionContextModule(TaskContext context)
		{
			this.context = context;
		}

		bool ICriticalFeature.IsCriticalException(Exception ex)
		{
			return false;
		}

		public void Init(ITaskEvent task)
		{
			task.Error += this.SetErrorExecutionContext;
		}

		public void Dispose()
		{
		}

		private void SetErrorExecutionContext(object sender, GenericEventArg<TaskErrorEventArg> e)
		{
			if (e.Data.ExceptionHandled)
			{
				return;
			}
			IErrorContextException ex = e.Data as IErrorContextException;
			if (ex != null)
			{
				ex.SetContext(this.ErrorExecutionContext);
			}
		}

		private readonly TaskContext context;

		private static volatile IErrorExecutionContext executionContext;

		private static object syncObject = new object();
	}
}
