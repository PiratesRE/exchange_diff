using System;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OwaRulesEvaluator : RulesEvaluator
	{
		public OwaRulesEvaluator(OwaRulesEvaluationContext context) : base(context)
		{
		}

		protected override ExecutionControl EnterRule()
		{
			OwaRulesEvaluationContext owaRulesEvaluationContext = (OwaRulesEvaluationContext)base.Context;
			PolicyTipRule policyTipRule = (PolicyTipRule)owaRulesEvaluationContext.CurrentRule;
			if (owaRulesEvaluationContext.RuleExecutionMonitor != null)
			{
				owaRulesEvaluationContext.RuleExecutionMonitor.RuleId = policyTipRule.ImmutableId.ToString("D");
				owaRulesEvaluationContext.RuleExecutionMonitor.Restart();
			}
			owaRulesEvaluationContext.ResetPerRuleData();
			return base.EnterRule();
		}

		protected override ExecutionControl ExitRule()
		{
			OwaRulesEvaluationContext owaRulesEvaluationContext = (OwaRulesEvaluationContext)base.Context;
			if (owaRulesEvaluationContext.RuleExecutionMonitor != null)
			{
				owaRulesEvaluationContext.RuleExecutionMonitor.Stop(true);
			}
			((OwaRulesEvaluationContext)base.Context).CapturePerRuleData();
			return base.ExitRule();
		}

		protected override ExecutionControl ExecuteAction(Microsoft.Exchange.MessagingPolicies.Rules.Action action, RulesEvaluationContext context)
		{
			ExecutionControl executionControl = base.ExecuteAction(action, context);
			if (executionControl == ExecutionControl.Execute && action is SenderNotify)
			{
				((OwaRulesEvaluationContext)base.Context).CapturePerRuleMatchData();
			}
			return executionControl;
		}
	}
}
