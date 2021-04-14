using System;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class ClientAccessRuleAllowAccessAction : ClientAccessRuleAction
	{
		public ClientAccessRuleAllowAccessAction(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "AllowAccess";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return ClientAccessRuleAllowAccessAction.PredicateBaseVersion;
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			ClientAccessRulesEvaluationContext clientAccessRulesEvaluationContext = (ClientAccessRulesEvaluationContext)baseContext;
			if (clientAccessRulesEvaluationContext.WhatIf)
			{
				clientAccessRulesEvaluationContext.WhatIfActionDelegate(clientAccessRulesEvaluationContext.CurrentRule, ClientAccessRulesAction.AllowAccess);
				return ExecutionControl.Execute;
			}
			return ExecutionControl.SkipAll;
		}

		public const string ActionName = "AllowAccess";

		public const ClientAccessRulesAction ActionIdentifier = ClientAccessRulesAction.AllowAccess;

		private static readonly Version PredicateBaseVersion = new Version("15.00.0008.00");
	}
}
