using System;
using Microsoft.Exchange.Management.Deployment.Analysis;

namespace Microsoft.Exchange.Management.Deployment.PrereqAnalysisSample
{
	internal class PrereqRuleConclusion : PrereqConclusion, IRuleConclusion
	{
		public PrereqRuleConclusion()
		{
		}

		public PrereqRuleConclusion(RuleResult ruleResult) : base(ruleResult)
		{
			this.severity = ruleResult.Source.Features.GetFeature<SeverityFeature>().Severity;
			this.message = ruleResult.Source.Features.GetFeature<MessageFeature>().TextFunction(ruleResult);
		}

		public bool IsConditionMet
		{
			get
			{
				return (bool)base.Value;
			}
			set
			{
				base.ThrowIfReadOnly();
				base.Value = value;
			}
		}

		public Severity Severity
		{
			get
			{
				return this.severity;
			}
			set
			{
				base.ThrowIfReadOnly();
				this.severity = value;
			}
		}

		public string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				base.ThrowIfReadOnly();
				this.message = value;
			}
		}

		private Severity severity;

		private string message;
	}
}
