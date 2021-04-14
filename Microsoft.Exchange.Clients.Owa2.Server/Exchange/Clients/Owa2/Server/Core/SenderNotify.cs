using System;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SenderNotify : Microsoft.Exchange.MessagingPolicies.Rules.Action
	{
		public SenderNotify(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return SenderNotify.ArgumentTypes;
			}
		}

		public override string Name
		{
			get
			{
				return "SenderNotify";
			}
		}

		public override bool ShouldExecute(RuleMode mode, RulesEvaluationContext context)
		{
			return base.ShouldExecute(mode, context) || RuleMode.AuditAndNotify == mode;
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			BaseTransportRulesEvaluationContext baseTransportRulesEvaluationContext = (BaseTransportRulesEvaluationContext)baseContext;
			if (baseTransportRulesEvaluationContext.CurrentRule != null && baseTransportRulesEvaluationContext.CurrentRule.Mode == RuleMode.AuditAndNotify)
			{
				baseTransportRulesEvaluationContext.ActionName = DlpPolicyTipAction.NotifyOnly.ToString();
			}
			else
			{
				baseTransportRulesEvaluationContext.ActionName = (string)base.Arguments[0].GetValue(baseTransportRulesEvaluationContext);
			}
			return ExecutionControl.Execute;
		}

		private static readonly Type[] ArgumentTypes = new Type[]
		{
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string)
		};
	}
}
