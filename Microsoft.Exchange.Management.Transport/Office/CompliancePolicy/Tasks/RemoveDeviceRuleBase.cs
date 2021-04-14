using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public abstract class RemoveDeviceRuleBase : RemoveComplianceRuleBase
	{
		protected abstract DeviceRuleBase CreateDeviceRule(RuleStorage ruleStorage);

		protected RemoveDeviceRuleBase(PolicyScenario scenario) : base(scenario)
		{
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.RemoveDeviceConfiguationRuleConfirmation(this.Identity.ToString());
			}
		}

		protected override IEnumerable<ChangeNotificationData> OnNotifyChanges()
		{
			return IntuneCompliancePolicySyncNotificationClient.NotifyChange(this, base.DataObject, new List<UnifiedPolicyStorageBase>(), (IConfigurationSession)base.DataSession, this.executionLogger);
		}
	}
}
