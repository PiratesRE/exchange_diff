using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class Halt : TransportAction
	{
		public Halt(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "Halt";
			}
		}

		public override TransportActionType Type
		{
			get
			{
				return TransportActionType.BifurcationNeeded;
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = baseContext as TransportRulesEvaluationContext;
			if (transportRulesEvaluationContext != null && transportRulesEvaluationContext.Server != null)
			{
				TransportUtils.AddRuleCollectionStamp(transportRulesEvaluationContext.MailItem.Message, transportRulesEvaluationContext.Server.Name);
			}
			return ExecutionControl.SkipAll;
		}
	}
}
