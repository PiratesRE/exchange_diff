using System;
using System.Collections;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Set", "DlpComplianceRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetDlpComplianceRule : SetComplianceRuleBase
	{
		[Parameter(Mandatory = false)]
		public Hashtable[] ContentContainsSensitiveInformation
		{
			get
			{
				return (Hashtable[])base.Fields["ContentContainsSensitiveInformation"];
			}
			set
			{
				base.Fields["ContentContainsSensitiveInformation"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AccessScope? AccessScopeIs
		{
			get
			{
				return (AccessScope?)base.Fields[PsDlpComplianceRuleSchema.AccessScopeIs];
			}
			set
			{
				base.Fields[PsDlpComplianceRuleSchema.AccessScopeIs] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ContentPropertyContainsWords
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields[PsDlpComplianceRuleSchema.ContentPropertyContainsWords];
			}
			set
			{
				base.Fields[PsDlpComplianceRuleSchema.ContentPropertyContainsWords] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter BlockAccess
		{
			get
			{
				return (SwitchParameter)(base.Fields[PsDlpComplianceRuleSchema.BlockAccess] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields[PsDlpComplianceRuleSchema.BlockAccess] = value;
			}
		}

		public SetDlpComplianceRule() : base(PolicyScenario.Dlp)
		{
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			RuleStorage ruleStorage = (RuleStorage)dataObject;
			ruleStorage.ResetChangeTracking(true);
			base.PsRulePresentationObject = new PsDlpComplianceRule(ruleStorage);
			PsDlpComplianceRule psDlpComplianceRule = (PsDlpComplianceRule)base.PsRulePresentationObject;
			base.PsRulePresentationObject.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
			if (base.PsRulePresentationObject.ReadOnly)
			{
				throw new TaskRuleIsTooAdvancedToModifyException(base.PsRulePresentationObject.Name);
			}
			if (ruleStorage.Mode == Mode.PendingDeletion)
			{
				base.WriteError(new ErrorCommonComplianceRuleIsDeletedException(ruleStorage.Name), ErrorCategory.InvalidOperation, null);
			}
			base.StampChangesOn(dataObject);
			this.CopyExplicitParameters();
			if (!psDlpComplianceRule.GetTaskActions().Any<PsComplianceRuleActionBase>())
			{
				throw new RuleContainsNoActionsException(psDlpComplianceRule.Name);
			}
			psDlpComplianceRule.UpdateStorageProperties(this, base.DataSession as IConfigurationSession, false);
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.Fields.IsModified("ContentContainsSensitiveInformation") && base.Fields["ContentContainsSensitiveInformation"] != null)
			{
				Utils.ValidateDataClassification(this.ContentContainsSensitiveInformation);
			}
			if (base.Fields.IsModified("AccessScopeIs"))
			{
				Utils.ValidateAccessScopeIsPredicate(this.AccessScopeIs);
			}
		}

		protected override void CopyExplicitParameters()
		{
			base.CopyExplicitParameters();
			PsDlpComplianceRule psDlpComplianceRule = (PsDlpComplianceRule)base.PsRulePresentationObject;
			if (base.Fields.IsModified("ContentContainsSensitiveInformation"))
			{
				psDlpComplianceRule.ContentContainsSensitiveInformation = this.ContentContainsSensitiveInformation;
			}
			if (base.Fields.IsModified(PsDlpComplianceRuleSchema.ContentPropertyContainsWords))
			{
				psDlpComplianceRule.ContentPropertyContainsWords = this.ContentPropertyContainsWords;
			}
			if (base.Fields.IsModified(PsDlpComplianceRuleSchema.AccessScopeIs))
			{
				psDlpComplianceRule.AccessScopeIs = this.AccessScopeIs;
			}
			if (base.Fields.IsModified(PsDlpComplianceRuleSchema.BlockAccess))
			{
				psDlpComplianceRule.BlockAccess = this.BlockAccess;
			}
		}
	}
}
