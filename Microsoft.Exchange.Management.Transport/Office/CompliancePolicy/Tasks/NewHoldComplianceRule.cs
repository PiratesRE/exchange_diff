using System;
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
	[Cmdlet("New", "HoldComplianceRule", SupportsShouldProcess = true)]
	public sealed class NewHoldComplianceRule : NewComplianceRuleBase
	{
		[Parameter(Mandatory = false)]
		public DateTime? ContentDateFrom
		{
			get
			{
				return (DateTime?)base.Fields[PsHoldRuleSchema.ContentDateFrom];
			}
			set
			{
				base.Fields[PsHoldRuleSchema.ContentDateFrom] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? ContentDateTo
		{
			get
			{
				return (DateTime?)base.Fields[PsHoldRuleSchema.ContentDateTo];
			}
			set
			{
				base.Fields[PsHoldRuleSchema.ContentDateTo] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ContentMatchQuery
		{
			get
			{
				return (string)base.Fields[PsComplianceRuleBaseSchema.ContentMatchQuery];
			}
			set
			{
				base.Fields[PsComplianceRuleBaseSchema.ContentMatchQuery] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int>? HoldContent { get; set; }

		[Parameter(Mandatory = false)]
		public HoldDurationHint HoldDurationDisplayHint
		{
			get
			{
				if (base.Fields[PsHoldRuleSchema.HoldDurationDisplayHint] != null)
				{
					return (HoldDurationHint)base.Fields[PsHoldRuleSchema.HoldDurationDisplayHint];
				}
				return HoldDurationHint.Days;
			}
			set
			{
				base.Fields[PsHoldRuleSchema.HoldDurationDisplayHint] = value;
			}
		}

		public NewHoldComplianceRule() : base(PolicyScenario.Hold)
		{
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (Utils.LoadRuleStoragesByPolicy(base.DataSession, this.policyStorage, this.policyStorage.Identity).Any<RuleStorage>())
			{
				throw new MulipleComplianceRulesFoundInPolicyException(this.policyStorage.Name);
			}
			if (this.HoldContent != null && this.HoldContent.Value <= 0)
			{
				throw new InvalidHoldContentActionException();
			}
			if (!Utils.ValidateContentDateParameter(this.ContentDateFrom, this.ContentDateTo))
			{
				throw new InvalidContentDateFromAndContentDateToPredicateException();
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			RuleStorage ruleStorage = (RuleStorage)base.PrepareDataObject();
			ruleStorage.Name = base.Name;
			ruleStorage.SetId(((ADObjectId)this.policyStorage.Identity).GetChildId(base.Name));
			PsHoldRule psHoldRule = new PsHoldRule(ruleStorage)
			{
				Comment = base.Comment,
				Disabled = base.Disabled,
				Mode = Mode.Enforce,
				Policy = Utils.GetUniversalIdentity(this.policyStorage),
				Workload = this.policyStorage.Workload,
				ContentMatchQuery = this.ContentMatchQuery,
				ContentDateFrom = this.ContentDateFrom,
				ContentDateTo = this.ContentDateTo,
				HoldContent = this.HoldContent,
				HoldDurationDisplayHint = this.HoldDurationDisplayHint
			};
			if (!psHoldRule.GetTaskActions().Any<PsComplianceRuleActionBase>())
			{
				throw new RuleContainsNoActionsException(psHoldRule.Name);
			}
			ADObjectId adobjectId;
			base.TryGetExecutingUserId(out adobjectId);
			psHoldRule.UpdateStorageProperties(this, base.DataSession as IConfigurationSession, true);
			return ruleStorage;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			PsHoldRule psHoldRule = new PsHoldRule(dataObject as RuleStorage);
			psHoldRule.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
			base.WriteResult(psHoldRule);
		}
	}
}
