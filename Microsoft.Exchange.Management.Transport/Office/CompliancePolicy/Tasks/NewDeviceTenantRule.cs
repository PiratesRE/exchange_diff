using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("New", "DeviceTenantRule", SupportsShouldProcess = true)]
	public sealed class NewDeviceTenantRule : NewDeviceRuleBase
	{
		[Parameter(Mandatory = false)]
		private new MultiValuedProperty<Guid> TargetGroups { get; set; }

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<Guid> ExclusionList
		{
			get
			{
				return (MultiValuedProperty<Guid>)base.Fields["ExclusionList"];
			}
			set
			{
				base.Fields["ExclusionList"] = value;
			}
		}

		public NewDeviceTenantRule() : base(PolicyScenario.DeviceTenantConditionalAccess)
		{
		}

		protected override DeviceRuleBase CreateDeviceRule(RuleStorage ruleStorage)
		{
			return new DeviceTenantRule(ruleStorage);
		}

		protected override Exception GetDeviceRuleAlreadyExistsException(string name)
		{
			return new DeviceTenantRuleAlreadyExistsException(name);
		}

		protected override bool GetDeviceRuleGuidFromWorkload(Workload workload, out Guid ruleGuid)
		{
			ruleGuid = default(Guid);
			return DevicePolicyUtility.GetTenantRuleGuidFromWorkload(workload, out ruleGuid);
		}

		protected override void ValidateWorkloadParameter()
		{
			Guid guid;
			if (!this.GetDeviceRuleGuidFromWorkload(Workload.Intune, out guid))
			{
				base.WriteError(new ArgumentException(Strings.InvalidDeviceRuleWorkload), ErrorCategory.InvalidArgument, null);
			}
			base.Name = (this.ruleName = guid.ToString());
		}

		protected override IConfigurable PrepareDataObject()
		{
			RuleStorage ruleStorage = (RuleStorage)base.PrepareDataObject();
			ruleStorage.Name = this.ruleName;
			ruleStorage.SetId(((ADObjectId)this.policyStorage.Identity).GetChildId(this.ruleName));
			DeviceTenantRule deviceTenantRule = this.CreateDeviceRule(ruleStorage) as DeviceTenantRule;
			deviceTenantRule.Policy = Utils.GetUniversalIdentity(this.policyStorage);
			deviceTenantRule.Workload = this.policyStorage.Workload;
			deviceTenantRule.ExclusionList = this.ExclusionList;
			this.SetPropsOnDeviceRule(deviceTenantRule);
			deviceTenantRule.UpdateStorageProperties(this, base.DataSession as IConfigurationSession, true);
			return ruleStorage;
		}

		[Parameter(Mandatory = false)]
		public PolicyResourceScope? ApplyPolicyTo
		{
			get
			{
				return (PolicyResourceScope?)base.Fields[DeviceTenantRule.AccessControl_ResourceScope];
			}
			set
			{
				base.Fields[DeviceTenantRule.AccessControl_ResourceScope] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? BlockUnsupportedDevices
		{
			get
			{
				return (bool?)base.Fields[DeviceTenantRule.AccessControl_AllowActionOnUnsupportedPlatform];
			}
			set
			{
				base.Fields[DeviceTenantRule.AccessControl_AllowActionOnUnsupportedPlatform] = value;
			}
		}

		protected override void SetPropsOnDeviceRule(DeviceRuleBase pdeviceRule)
		{
			DeviceTenantRule deviceTenantRule = (DeviceTenantRule)pdeviceRule;
			deviceTenantRule.ApplyPolicyTo = this.ApplyPolicyTo;
			deviceTenantRule.BlockUnsupportedDevices = this.BlockUnsupportedDevices;
		}
	}
}
