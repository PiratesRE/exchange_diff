using System;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MrsSystemTask : SystemTaskBase
	{
		public MrsSystemTask(Job job, Action callback, SystemWorkloadBase systemWorkload, ResourceReservation reservation, bool ignoreTaskSuccessfulExecutionTime = false) : base(systemWorkload)
		{
			this.Job = job;
			base.ResourceReservation = reservation;
			this.Callback = callback;
			this.IgnoreTaskSuccessfulExecutionTime = ignoreTaskSuccessfulExecutionTime;
		}

		public Action Callback { get; private set; }

		public Job Job { get; private set; }

		public bool IgnoreTaskSuccessfulExecutionTime { get; private set; }

		public Exception Failure { get; private set; }

		protected override TaskStepResult Execute()
		{
			TaskStepResult result;
			using (SettingsContextBase.ActivateContext(this.Job as ISettingsContextProvider))
			{
				this.Job.GetCurrentActivityScope();
				try
				{
					this.Run();
				}
				catch (Exception exception)
				{
					this.Job.PerformCrashingFailureActions(exception);
					throw;
				}
				result = TaskStepResult.Complete;
			}
			return result;
		}

		public void Run()
		{
			CommonUtils.CatchKnownExceptions(delegate
			{
				this.Callback();
			}, delegate(Exception failure)
			{
				this.Failure = failure;
			});
		}
	}
}
