using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("New", "AuditConfigurationPolicy", SupportsShouldProcess = true)]
	public sealed class NewAuditConfigurationPolicy : NewCompliancePolicyBase
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

		[Parameter(Mandatory = true)]
		public Workload Workload
		{
			get
			{
				return (Workload)base.Fields[PsCompliancePolicyBaseSchema.Workload];
			}
			set
			{
				base.Fields[PsCompliancePolicyBaseSchema.Workload] = value;
			}
		}

		public NewAuditConfigurationPolicy() : base(PolicyScenario.AuditSettings)
		{
		}

		protected override IConfigurable PrepareDataObject()
		{
			base.Name = this.policyName;
			return (PolicyStorage)base.PrepareDataObject();
		}

		protected override void InternalValidate()
		{
			this.ValidateWorkloadParameter(this.Workload);
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.PsPolicyPresentationObject = new AuditConfigurationPolicy(this.DataObject)
			{
				Name = this.policyName,
				Workload = AuditConfigUtility.GetEffectiveWorkload(this.Workload),
				Enabled = false,
				Mode = Mode.Enforce,
				ExchangeBinding = base.InternalExchangeBindings,
				SharePointBinding = base.InternalSharePointBindings
			};
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			AuditConfigurationPolicy result = new AuditConfigurationPolicy(dataObject as PolicyStorage);
			base.WriteResult(result);
		}

		private void ValidateWorkloadParameter(Workload workload)
		{
			Guid guid;
			if (AuditPolicyUtility.GetPolicyGuidFromWorkload(this.Workload, out guid))
			{
				this.policyName = guid.ToString();
				this.Name = this.policyName;
				return;
			}
			base.WriteError(new ArgumentException(Strings.InvalidCombinationOfCompliancePolicyTypeAndWorkload), ErrorCategory.InvalidArgument, null);
		}

		private string policyName;
	}
}
