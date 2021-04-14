using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("New", "DeviceTenantPolicy", SupportsShouldProcess = true)]
	public sealed class NewDeviceTenantPolicy : NewDevicePolicyBase
	{
		[Parameter(Mandatory = false)]
		public new string Name
		{
			get
			{
				return base.Name;
			}
			private set
			{
				base.Name = value;
			}
		}

		public NewDeviceTenantPolicy() : base(PolicyScenario.DeviceTenantConditionalAccess)
		{
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.PsPolicyPresentationObject = new DeviceTenantPolicy(this.DataObject)
			{
				Name = this.Name,
				Comment = base.Comment,
				Workload = Workload.Intune,
				Enabled = base.Enabled,
				Mode = Mode.Enforce,
				ExchangeBinding = base.InternalExchangeBindings,
				SharePointBinding = base.InternalSharePointBindings
			};
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			DeviceTenantPolicy deviceTenantPolicy = new DeviceTenantPolicy(dataObject as PolicyStorage)
			{
				StorageBindings = Utils.LoadBindingStoragesByPolicy(base.DataSession, dataObject as PolicyStorage)
			};
			deviceTenantPolicy.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
			base.WriteResult(deviceTenantPolicy);
		}

		private void ValidateWorkloadParameter()
		{
			Guid guid;
			if (DevicePolicyUtility.GetTenantPolicyGuidFromWorkload(Workload.Intune, out guid))
			{
				this.policyName = guid.ToString();
				this.Name = this.policyName;
				return;
			}
			base.WriteError(new ArgumentException(Strings.InvalidCombinationOfCompliancePolicyTypeAndWorkload), ErrorCategory.InvalidArgument, null);
		}

		protected override void InternalValidate()
		{
			this.ValidateWorkloadParameter();
			base.InternalValidate();
		}

		private string policyName;
	}
}
