using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Assistants
{
	internal class TimeBasedAssistantTask : SystemTaskBase
	{
		internal TimeBasedAssistantTask(SystemWorkloadBase workload, TimeBasedDatabaseDriver driver, ResourceReservation reservation) : base(workload, reservation)
		{
			this.driver = driver;
		}

		internal IEnumerable<ResourceKey> ResourceDependencies
		{
			get
			{
				return this.driver.ResourceDependencies;
			}
		}

		protected override TaskStepResult Execute()
		{
			DateTime utcNow = DateTime.UtcNow;
			do
			{
				this.context = this.driver.ProcessNextTask(this.context);
			}
			while ((this.context != null || this.driver.HasTask()) && DateTime.UtcNow - utcNow < Configuration.BatchDuration);
			if (this.context == null)
			{
				return TaskStepResult.Complete;
			}
			return TaskStepResult.Yield;
		}

		private TimeBasedDatabaseDriver driver;

		private AssistantTaskContext context;
	}
}
