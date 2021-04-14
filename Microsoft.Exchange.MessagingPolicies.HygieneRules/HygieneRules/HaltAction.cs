using System;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.MessagingPolicies.HygieneRules
{
	internal class HaltAction : Microsoft.Exchange.MessagingPolicies.Rules.Action
	{
		public HaltAction(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "Halt";
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			return ExecutionControl.SkipAll;
		}
	}
}
