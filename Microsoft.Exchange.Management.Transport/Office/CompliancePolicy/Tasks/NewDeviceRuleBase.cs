using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public abstract class NewDeviceRuleBase : NewComplianceRuleBase
	{
		protected abstract void SetPropsOnDeviceRule(DeviceRuleBase deviceRule);

		protected abstract DeviceRuleBase CreateDeviceRule(RuleStorage ruleStorage);

		protected abstract Exception GetDeviceRuleAlreadyExistsException(string name);

		protected abstract bool GetDeviceRuleGuidFromWorkload(Workload workload, out Guid ruleGuid);

		protected NewDeviceRuleBase(PolicyScenario scenario) : base(scenario)
		{
		}

		[Parameter(Mandatory = false)]
		public new string Name
		{
			get
			{
				return base.Name;
			}
			protected set
			{
				base.Name = value;
			}
		}

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

		protected override void InternalValidate()
		{
			this.ValidateWorkloadParameter();
			base.InternalValidate();
		}

		protected override IConfigurable PrepareDataObject()
		{
			RuleStorage ruleStorage = (RuleStorage)base.PrepareDataObject();
			ruleStorage.Name = this.ruleName;
			ruleStorage.SetId(((ADObjectId)this.policyStorage.Identity).GetChildId(this.ruleName));
			DeviceRuleBase deviceRuleBase = this.CreateDeviceRule(ruleStorage);
			deviceRuleBase.Policy = Utils.GetUniversalIdentity(this.policyStorage);
			deviceRuleBase.Workload = this.policyStorage.Workload;
			deviceRuleBase.TargetGroups = this.TargetGroups;
			this.SetPropsOnDeviceRule(deviceRuleBase);
			deviceRuleBase.UpdateStorageProperties(this, base.DataSession as IConfigurationSession, true);
			return ruleStorage;
		}

		protected virtual void ValidateWorkloadParameter()
		{
			Guid guid;
			if (!this.GetDeviceRuleGuidFromWorkload(Workload.Intune, out guid))
			{
				base.WriteError(new ArgumentException(Strings.InvalidDeviceRuleWorkload), ErrorCategory.InvalidArgument, null);
			}
			this.Name = (this.ruleName = base.Policy.RawIdentity + guid.ToString("B"));
		}

		protected override IEnumerable<ChangeNotificationData> OnNotifyChanges()
		{
			return IntuneCompliancePolicySyncNotificationClient.NotifyChange(this, this.DataObject, new List<UnifiedPolicyStorageBase>(), (IConfigurationSession)base.DataSession, this.executionLogger);
		}

		protected string ruleName;
	}
}
