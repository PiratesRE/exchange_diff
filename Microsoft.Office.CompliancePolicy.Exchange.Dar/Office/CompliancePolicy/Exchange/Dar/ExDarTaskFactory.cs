using System;
using System.Linq;
using Microsoft.Exchange.Data.ApplicationLogic.Compliance;
using Microsoft.Office.CompliancePolicy.Dar;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.LocStrings;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar
{
	internal class ExDarTaskFactory : DarTaskFactory
	{
		public ExDarTaskFactory(DarServiceProvider provider)
		{
			this.provider = provider;
		}

		public override DarTask CreateTask(string taskType)
		{
			if (taskType != null)
			{
				if (taskType == "Common.BindingApplication")
				{
					return new ExBindingTask
					{
						ComplianceProviderExecutionLog = this.provider.ExecutionLog
					};
				}
				if (taskType == "Common.Iteration")
				{
					return new DarIterationTask
					{
						ComplianceServiceProvider = new ExComplianceServiceProvider()
					};
				}
			}
			return base.CreateTask(taskType);
		}

		public override DarTaskAggregate CreateTaskAggregate(string taskType)
		{
			if (base.GetAllTaskTypes().Contains(taskType))
			{
				return new DarTaskAggregate
				{
					TaskType = taskType
				};
			}
			throw new ApplicationException(Strings.TaskTypeUnknown);
		}

		private DarServiceProvider provider;
	}
}
