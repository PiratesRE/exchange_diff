using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("New", "AuditConfigurationRule", SupportsShouldProcess = true)]
	public sealed class NewAuditConfigurationRule : NewComplianceRuleBase
	{
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

		[Parameter(Mandatory = true)]
		public MultiValuedProperty<AuditableOperations> AuditOperation
		{
			get
			{
				return (MultiValuedProperty<AuditableOperations>)base.Fields["AuditOperation"];
			}
			set
			{
				base.Fields["AuditOperation"] = value;
			}
		}

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

		[Parameter(Mandatory = false)]
		public new PolicyIdParameter Policy
		{
			get
			{
				return base.Policy;
			}
			private set
			{
				base.Policy = value;
			}
		}

		public NewAuditConfigurationRule() : base(PolicyScenario.AuditSettings)
		{
		}

		protected override void InternalValidate()
		{
			this.ValidateWorkloadParameter();
			this.policyStorage = (PolicyStorage)base.GetDataObject<PolicyStorage>(this.Policy, base.DataSession, null, new LocalizedString?(Strings.ErrorPolicyNotFound(this.Policy.ToString())), new LocalizedString?(Strings.ErrorPolicyNotUnique(this.Policy.ToString())), ExchangeErrorCategory.Client);
			if (this.policyStorage.Mode == Mode.PendingDeletion)
			{
				base.WriteError(new ErrorCannotCreateRuleUnderPendingDeletionPolicyException(this.policyStorage.Name), ErrorCategory.InvalidOperation, null);
			}
			base.InternalValidate();
		}

		private void ValidateWorkloadParameter()
		{
			Guid guid;
			if (!AuditPolicyUtility.GetRuleGuidFromWorkload(this.Workload, out guid))
			{
				base.WriteError(new ArgumentException(Strings.InvalidAuditRuleWorkload), ErrorCategory.InvalidArgument, null);
			}
			this.Name = guid.ToString();
			Guid guid2;
			if (!AuditPolicyUtility.GetPolicyGuidFromWorkload(this.Workload, out guid2))
			{
				base.WriteError(new ArgumentException(Strings.InvalidAuditRuleWorkload), ErrorCategory.InvalidArgument, null);
			}
			this.Policy = new PolicyIdParameter(guid2.ToString());
		}

		protected override IConfigurable PrepareDataObject()
		{
			RuleStorage ruleStorage = (RuleStorage)base.PrepareDataObject();
			ruleStorage.Name = this.Name;
			ruleStorage.SetId(((ADObjectId)this.policyStorage.Identity).GetChildId(this.Name));
			AuditConfigurationRule auditConfigurationRule = new AuditConfigurationRule(ruleStorage)
			{
				Policy = Utils.GetUniversalIdentity(this.policyStorage),
				Workload = this.policyStorage.Workload,
				AuditOperation = this.AuditOperation
			};
			auditConfigurationRule.UpdateStorageProperties();
			return ruleStorage;
		}
	}
}
