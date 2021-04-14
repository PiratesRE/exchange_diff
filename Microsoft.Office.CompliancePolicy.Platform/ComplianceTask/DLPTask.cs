using System;
using Microsoft.Office.CompliancePolicy.Dar;

namespace Microsoft.Office.CompliancePolicy.ComplianceTask
{
	public class DLPTask : PeriodicPolicyTask
	{
		public DLPTask()
		{
			this.Category = DarTaskCategory.Low;
			base.TaskRetryTotalCount = 20;
			base.TaskRetryInterval = new TimeSpan(0, 0, 300);
		}

		public override string TaskType
		{
			get
			{
				return "Common.DLPApplication";
			}
		}

		public override int MaxLevel
		{
			get
			{
				return 1;
			}
		}

		private const int DefaultRetryCount = 20;

		private const int DefaultRetryIntervalInSeconds = 300;
	}
}
