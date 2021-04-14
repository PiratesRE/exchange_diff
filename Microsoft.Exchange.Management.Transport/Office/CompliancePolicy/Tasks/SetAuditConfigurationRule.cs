using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Set", "AuditConfigurationRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetAuditConfigurationRule : SetComplianceRuleBase
	{
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

		public SetAuditConfigurationRule() : base(PolicyScenario.AuditSettings)
		{
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			RuleStorage ruleStorage = (RuleStorage)dataObject;
			ruleStorage.ResetChangeTracking(true);
			AuditConfigurationRule auditConfigurationRule = new AuditConfigurationRule(dataObject as RuleStorage);
			auditConfigurationRule.PopulateTaskProperties();
			if (ruleStorage.Mode == Mode.PendingDeletion)
			{
				base.WriteError(new ErrorCommonComplianceRuleIsDeletedException(ruleStorage.Name), ErrorCategory.InvalidOperation, null);
			}
			base.StampChangesOn(dataObject);
			auditConfigurationRule.CopyChangesFrom(base.DynamicParametersInstance);
			auditConfigurationRule.AuditOperation = this.AuditOperation;
			auditConfigurationRule.UpdateStorageProperties();
		}

		protected override void InternalValidate()
		{
			this.ValidateUnacceptableParameter();
			base.InternalValidate();
		}

		private void ValidateUnacceptableParameter()
		{
			if (this.Identity != null && !AuditPolicyUtility.IsAuditConfigurationRule(this.Identity.ToString()))
			{
				base.WriteError(new ArgumentException(Strings.CanOnlyManipulateAuditConfigurationRule), ErrorCategory.InvalidArgument, null);
			}
			if (base.DynamicParametersInstance.IsModified(ADObjectSchema.Name))
			{
				base.WriteError(new ArgumentException(Strings.CannotChangeAuditConfigurationRuleName), ErrorCategory.InvalidArgument, null);
			}
		}
	}
}
