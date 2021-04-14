using System;
using Microsoft.Office.CompliancePolicy;
using Microsoft.Office.CompliancePolicy.ComplianceTask;

namespace Microsoft.Exchange.Data.ApplicationLogic.Compliance
{
	internal class ExBindingTask : BindingTask
	{
		public ExecutionLog ComplianceProviderExecutionLog { get; set; }

		public override ComplianceServiceProvider ComplianceServiceProvider
		{
			get
			{
				if (base.ComplianceServiceProvider == null)
				{
					string workloadData = this.WorkloadData;
					this.ComplianceServiceProvider = new ExComplianceServiceProvider(workloadData, this.ComplianceProviderExecutionLog);
				}
				return base.ComplianceServiceProvider;
			}
		}

		public override string GetWorkloadDataFromWorkload()
		{
			string result = string.Empty;
			ExComplianceServiceProvider exComplianceServiceProvider = this.ComplianceServiceProvider as ExComplianceServiceProvider;
			if (exComplianceServiceProvider != null)
			{
				result = exComplianceServiceProvider.PreferredDomainController;
			}
			return result;
		}
	}
}
