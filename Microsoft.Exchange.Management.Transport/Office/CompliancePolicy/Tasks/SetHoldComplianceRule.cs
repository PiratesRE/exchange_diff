using System;
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
	[Cmdlet("Set", "HoldComplianceRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetHoldComplianceRule : SetComplianceRuleBase
	{
		[Parameter(Mandatory = false)]
		public Unlimited<int>? HoldContent
		{
			get
			{
				return (Unlimited<int>?)base.Fields["HoldContent"];
			}
			set
			{
				base.Fields["HoldContent"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public HoldDurationHint HoldDurationDisplayHint
		{
			get
			{
				return (HoldDurationHint)base.Fields["HoldDurationDisplayHint"];
			}
			set
			{
				base.Fields["HoldDurationDisplayHint"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? ContentDateFrom
		{
			get
			{
				return (DateTime?)base.Fields["ContentDateFrom"];
			}
			set
			{
				base.Fields["ContentDateFrom"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? ContentDateTo
		{
			get
			{
				return (DateTime?)base.Fields["ContentDateTo"];
			}
			set
			{
				base.Fields["ContentDateTo"] = value;
			}
		}

		public SetHoldComplianceRule() : base(PolicyScenario.Hold)
		{
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			RuleStorage ruleStorage = (RuleStorage)dataObject;
			ruleStorage.ResetChangeTracking(true);
			base.PsRulePresentationObject = new PsHoldRule(ruleStorage);
			PsHoldRule psHoldRule = (PsHoldRule)base.PsRulePresentationObject;
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
			if (!Utils.ValidateContentDateParameter(psHoldRule.ContentDateFrom, psHoldRule.ContentDateTo))
			{
				throw new InvalidContentDateFromAndContentDateToPredicateException();
			}
			if (!psHoldRule.GetTaskActions().Any<PsComplianceRuleActionBase>())
			{
				base.WriteError(new RuleContainsNoActionsException(psHoldRule.Name), ErrorCategory.InvalidData, psHoldRule);
			}
			psHoldRule.UpdateStorageProperties(this, base.DataSession as IConfigurationSession, false);
		}

		protected override void InternalValidate()
		{
			if (base.Fields.IsModified("HoldContent") && this.HoldContent != null && this.HoldContent.Value <= 0)
			{
				throw new InvalidHoldContentActionException();
			}
			base.InternalValidate();
		}

		protected override void CopyExplicitParameters()
		{
			base.CopyExplicitParameters();
			PsHoldRule psHoldRule = (PsHoldRule)base.PsRulePresentationObject;
			if (base.Fields.IsModified("HoldContent"))
			{
				psHoldRule.HoldContent = this.HoldContent;
			}
			if (base.Fields.IsModified("HoldDurationDisplayHint"))
			{
				psHoldRule.HoldDurationDisplayHint = this.HoldDurationDisplayHint;
			}
			if (base.Fields.IsModified("ContentDateFrom"))
			{
				psHoldRule.ContentDateFrom = this.ContentDateFrom;
			}
			if (base.Fields.IsModified("ContentDateTo"))
			{
				psHoldRule.ContentDateTo = this.ContentDateTo;
			}
		}
	}
}
