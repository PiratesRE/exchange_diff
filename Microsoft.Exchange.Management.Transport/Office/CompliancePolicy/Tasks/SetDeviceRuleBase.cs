using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public abstract class SetDeviceRuleBase : SetComplianceRuleBase
	{
		protected abstract DeviceRuleBase CreateDeviceRule(RuleStorage dataObject);

		protected abstract void SetPropsOnDeviceRule(DeviceRuleBase deviceRule);

		protected abstract void ValidateUnacceptableParameter();

		[Parameter(Mandatory = true)]
		public MultiValuedProperty<Guid> TargetGroups
		{
			get
			{
				return (MultiValuedProperty<Guid>)base.Fields["TargetGroups"];
			}
			set
			{
				base.Fields["TargetGroups"] = value;
			}
		}

		protected SetDeviceRuleBase(PolicyScenario scenario) : base(scenario)
		{
			DevicePolicyUtility.ValidateDeviceScenarioArgument(scenario);
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			base.StampChangesOn(dataObject);
			DeviceRuleBase deviceRuleBase = this.CreateDeviceRule(dataObject as RuleStorage);
			deviceRuleBase.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
			deviceRuleBase.CopyChangesFrom(base.DynamicParametersInstance);
			deviceRuleBase.TargetGroups = this.TargetGroups;
			this.SetPropsOnDeviceRule(deviceRuleBase);
			deviceRuleBase.UpdateStorageProperties(this, base.DataSession as IConfigurationSession, false);
		}

		protected override void InternalValidate()
		{
			this.ValidateUnacceptableParameter();
			base.InternalValidate();
		}

		protected override IEnumerable<ChangeNotificationData> OnNotifyChanges()
		{
			return IntuneCompliancePolicySyncNotificationClient.NotifyChange(this, this.DataObject, new List<UnifiedPolicyStorageBase>(), (IConfigurationSession)base.DataSession, this.executionLogger);
		}
	}
}
