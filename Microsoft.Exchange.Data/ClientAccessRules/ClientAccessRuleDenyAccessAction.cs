using System;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class ClientAccessRuleDenyAccessAction : ClientAccessRuleAction
	{
		public ClientAccessRuleDenyAccessAction(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "DenyAccess";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return ClientAccessRuleDenyAccessAction.PredicateBaseVersion;
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			ClientAccessRulesEvaluationContext clientAccessRulesEvaluationContext = (ClientAccessRulesEvaluationContext)baseContext;
			clientAccessRulesEvaluationContext.DenyAccessDelegate(clientAccessRulesEvaluationContext);
			if (clientAccessRulesEvaluationContext.WhatIf)
			{
				clientAccessRulesEvaluationContext.WhatIfActionDelegate(clientAccessRulesEvaluationContext.CurrentRule, ClientAccessRulesAction.DenyAccess);
				return ExecutionControl.Execute;
			}
			return ExecutionControl.SkipAll;
		}

		public const string ActionName = "DenyAccess";

		public const ClientAccessRulesAction ActionIdentifier = ClientAccessRulesAction.DenyAccess;

		private static readonly Version PredicateBaseVersion = new Version("15.00.0008.00");
	}
}
