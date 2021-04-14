using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Set", "DevicePolicyBase", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public abstract class SetDevicePolicyBase : SetCompliancePolicyBase
	{
		public SetDevicePolicyBase(PolicyScenario scenario) : base(scenario)
		{
			DevicePolicyUtility.ValidateDeviceScenarioArgument(scenario);
		}

		protected override IEnumerable<ChangeNotificationData> OnNotifyChanges()
		{
			return IntuneCompliancePolicySyncNotificationClient.NotifyChange(this, this.DataObject, new List<UnifiedPolicyStorageBase>(), (IConfigurationSession)base.DataSession, base.ExecutionLogger);
		}
	}
}
