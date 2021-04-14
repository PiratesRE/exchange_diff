using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Remove", "DevicePolicyBase", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public abstract class RemoveDevicePolicyBase : RemoveCompliancePolicyBase
	{
		protected RemoveDevicePolicyBase(PolicyScenario deviceConditionalAccess) : base(deviceConditionalAccess)
		{
		}

		protected override IEnumerable<ChangeNotificationData> OnNotifyChanges(IEnumerable<UnifiedPolicyStorageBase> bindingStorageObjects, IEnumerable<UnifiedPolicyStorageBase> ruleStorageObjects)
		{
			return IntuneCompliancePolicySyncNotificationClient.NotifyChange(this, base.DataObject, ruleStorageObjects, (IConfigurationSession)base.DataSession, this.executionLogger);
		}
	}
}
