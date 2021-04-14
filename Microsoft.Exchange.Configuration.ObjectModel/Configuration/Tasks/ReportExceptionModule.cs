using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class ReportExceptionModule : ITaskModule, ICriticalFeature
	{
		public ReportExceptionModule(TaskContext context)
		{
			this.context = context;
		}

		bool ICriticalFeature.IsCriticalException(Exception ex)
		{
			return false;
		}

		public void Init(ITaskEvent task)
		{
			task.Error += this.ReportException;
		}

		public void Dispose()
		{
		}

		private void ReportException(object sender, GenericEventArg<TaskErrorEventArg> e)
		{
			if (e.Data.ExceptionHandled)
			{
				return;
			}
			TaskLogger.LogError(e.Data.Exception);
			bool flag;
			if (this.context != null && this.context.ErrorInfo.Exception != null && !this.context.ErrorInfo.IsKnownError && TaskHelper.ShouldReportException(e.Data.Exception, out flag))
			{
				if (!flag)
				{
					this.context.CommandShell.WriteWarning(Strings.UnhandledErrorMessage(e.Data.Exception.Message));
				}
				TaskLogger.SendWatsonReport(e.Data.Exception, this.context.InvocationInfo.CommandName, this.context.InvocationInfo.UserSpecifiedParameters);
			}
		}

		private readonly TaskContext context;
	}
}
