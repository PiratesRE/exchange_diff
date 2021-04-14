using System;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class UnconditionalTruePredicate : PredicateCondition
	{
		public override string Name
		{
			get
			{
				return "UnconditionalTrue";
			}
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			return true;
		}
	}
}
