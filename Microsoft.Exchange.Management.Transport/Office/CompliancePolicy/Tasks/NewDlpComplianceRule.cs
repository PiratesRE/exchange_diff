using System;
using System.Collections;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("New", "DlpComplianceRule", SupportsShouldProcess = true)]
	public sealed class NewDlpComplianceRule : NewComplianceRuleBase
	{
		[Parameter(Mandatory = false)]
		public Hashtable[] ContentContainsSensitiveInformation { get; set; }

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

		public NewDlpComplianceRule() : base(PolicyScenario.Dlp)
		{
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.ContentContainsSensitiveInformation != null)
			{
				Utils.ValidateDataClassification(this.ContentContainsSensitiveInformation);
			}
			Utils.ValidateAccessScopeIsPredicate(this.AccessScopeIs);
		}

		protected override IConfigurable PrepareDataObject()
		{
			RuleStorage ruleStorage = (RuleStorage)base.PrepareDataObject();
			ruleStorage.Name = base.Name;
			ruleStorage.SetId(((ADObjectId)this.policyStorage.Identity).GetChildId(base.Name));
			ruleStorage.MasterIdentity = Guid.NewGuid();
			PsDlpComplianceRule psDlpComplianceRule = new PsDlpComplianceRule(ruleStorage)
			{
				Comment = base.Comment,
				Disabled = base.Disabled,
				Mode = Mode.Enforce,
				Policy = Utils.GetUniversalIdentity(this.policyStorage),
				Workload = this.policyStorage.Workload,
				ContentPropertyContainsWords = this.ContentPropertyContainsWords,
				ContentContainsSensitiveInformation = this.ContentContainsSensitiveInformation,
				AccessScopeIs = this.AccessScopeIs,
				BlockAccess = this.BlockAccess
			};
			if (!psDlpComplianceRule.GetTaskActions().Any<PsComplianceRuleActionBase>())
			{
				throw new RuleContainsNoActionsException(psDlpComplianceRule.Name);
			}
			ADObjectId adobjectId;
			base.TryGetExecutingUserId(out adobjectId);
			psDlpComplianceRule.UpdateStorageProperties(this, base.DataSession as IConfigurationSession, true);
			return ruleStorage;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			PsDlpComplianceRule psDlpComplianceRule = new PsDlpComplianceRule(dataObject as RuleStorage);
			psDlpComplianceRule.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
			base.WriteResult(psDlpComplianceRule);
		}
	}
}
