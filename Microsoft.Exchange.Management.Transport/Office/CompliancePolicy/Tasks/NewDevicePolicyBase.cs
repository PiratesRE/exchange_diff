using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("New", "DevicePolicyBase", SupportsShouldProcess = true)]
	public abstract class NewDevicePolicyBase : NewCompliancePolicyBase
	{
		protected NewDevicePolicyBase(PolicyScenario deviceConditionalAccess) : base(deviceConditionalAccess)
		{
		}

		protected override IEnumerable<ChangeNotificationData> OnNotifyChanges()
		{
			return IntuneCompliancePolicySyncNotificationClient.NotifyChange(this, this.DataObject, new List<UnifiedPolicyStorageBase>(), (IConfigurationSession)base.DataSession, this.executionLogger);
		}
	}
}
