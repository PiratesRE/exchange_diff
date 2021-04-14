using System;
using Microsoft.Office.CompliancePolicy.ComplianceData;
using Microsoft.Office.CompliancePolicy.Dar;

namespace Microsoft.Office.CompliancePolicy.ComplianceTask
{
	public abstract class PeriodicPolicyTask : ComplianceTask
	{
		public PeriodicPolicyTask()
		{
			this.Category = DarTaskCategory.Low;
			base.TaskRetryTotalCount = 20;
			base.TaskRetryInterval = new TimeSpan(0, 0, 300);
		}

		public virtual int MaxLevel
		{
			get
			{
				return this.maxLevel;
			}
			set
			{
				this.maxLevel = value;
			}
		}

		[SerializableTaskData]
		public string Scope { get; set; }

		public override void CompleteTask(DarTaskManager darTaskManager)
		{
		}

		public override DarTaskExecutionResult Execute(DarTaskManager darTaskManager)
		{
			ComplianceItemContainer complianceItemContainer = this.ComplianceServiceProvider.GetComplianceItemContainer(base.TenantId, this.Scope);
			ComplianceService complianceService = new ComplianceService(this, darTaskManager);
			return complianceService.ApplyPeriodicPolicies(complianceItemContainer);
		}

		private const int DefaultRetryCount = 20;

		private const int DefaultRetryIntervalInSeconds = 300;

		private int maxLevel = int.MaxValue;
	}
}
