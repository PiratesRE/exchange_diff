using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class AutoReportProgressModule : ITaskModule, ICriticalFeature
	{
		public AutoReportProgressModule(TaskContext context)
		{
			this.context = context;
		}

		bool ICriticalFeature.IsCriticalException(Exception ex)
		{
			return false;
		}

		public void Init(ITaskEvent task)
		{
			task.IterateCompleted += this.ReportProgress;
			task.Error += this.ReportProgressOnError;
		}

		public void Dispose()
		{
		}

		private void ReportProgressOnError(object sender, GenericEventArg<TaskErrorEventArg> e)
		{
			if (this.context.ErrorInfo.IsKnownError && !this.context.ErrorInfo.TerminatePipeline)
			{
				this.ReportProgress(sender, e);
			}
		}

		private void ReportProgress(object sender, EventArgs e)
		{
			if (this.context.CurrentObjectIndex >= 0)
			{
				ExProgressRecord exProgressRecord = new ExProgressRecord(this.context.CurrentObjectIndex, Strings.TaskCompleted, Strings.TaskCompleted);
				exProgressRecord.RecordType = ProgressRecordType.Completed;
				this.context.CommandShell.WriteProgress(exProgressRecord);
			}
		}

		private readonly TaskContext context;
	}
}
