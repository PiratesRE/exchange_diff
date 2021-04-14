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
	[Cmdlet("Set", "DeviceTenantRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetDeviceTenantRule : SetDeviceRuleBase
	{
		[Parameter(Mandatory = false)]
		private new MultiValuedProperty<Guid> TargetGroups { get; set; }

		[Parameter(Mandatory = true)]
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

		public SetDeviceTenantRule() : base(PolicyScenario.DeviceTenantConditionalAccess)
		{
		}

		protected override DeviceRuleBase CreateDeviceRule(RuleStorage dataObject)
		{
			return new DeviceTenantRule(dataObject);
		}

		protected override void ValidateUnacceptableParameter()
		{
			if (this.Identity != null && !DevicePolicyUtility.IsDeviceTenantRule(this.Identity.ToString()))
			{
				base.WriteError(new ArgumentException(Strings.CanOnlyManipulateDeviceTenantRule), ErrorCategory.InvalidArgument, null);
			}
			if (DevicePolicyUtility.IsPropertySpecified(base.DynamicParametersInstance, ADObjectSchema.Name))
			{
				base.WriteError(new ArgumentException(Strings.CannotChangeDeviceTenantRuleName), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			base.StampChangesOn(dataObject);
			DeviceTenantRule deviceTenantRule = this.CreateDeviceRule(dataObject as RuleStorage) as DeviceTenantRule;
			if (deviceTenantRule != null)
			{
				deviceTenantRule.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
				deviceTenantRule.CopyChangesFrom(base.DynamicParametersInstance);
				deviceTenantRule.ExclusionList = this.ExclusionList;
				this.SetPropsOnDeviceRule(deviceTenantRule);
				deviceTenantRule.UpdateStorageProperties(this, base.DataSession as IConfigurationSession, false);
			}
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
