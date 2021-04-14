using System;
using Microsoft.Exchange.Management.Deployment.HybridConfigurationDetection;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class TenantDetectionTask : SessionTask
	{
		public TenantDetectionTask() : base(HybridStrings.TenantDetectionTaskName, 1)
		{
		}

		public override bool CheckPrereqs(ITaskContext taskContext)
		{
			if (!base.CheckPrereqs(taskContext))
			{
				return false;
			}
			base.Logger.LogInformation(HybridStrings.HybridEngineCheckingForUpgradeTenant);
			using (HybridConfigurationDetection hybridConfigurationDetection = new HybridConfigurationDetection(base.Logger))
			{
				if (!hybridConfigurationDetection.RunTenantHybridTest(null, taskContext.TenantSession.GetOrganizationConfig().OrganizationConfigHash))
				{
					base.Logger.LogInformation(HybridStrings.ReturnResultForHybridDetectionWasFalse);
					return false;
				}
			}
			return true;
		}

		private const string CASRole = "ClientAccess";

		private const string isCoexistenceDomainKey = "IsCoexistenceDomain";
	}
}
