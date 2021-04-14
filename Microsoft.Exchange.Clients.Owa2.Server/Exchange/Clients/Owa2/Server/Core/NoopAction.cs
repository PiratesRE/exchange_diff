using System;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class NoopAction : Microsoft.Exchange.MessagingPolicies.Rules.Action
	{
		public NoopAction(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "Noop";
			}
		}

		public override void ValidateArguments(ShortList<Argument> inputArguments)
		{
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			return ExecutionControl.Execute;
		}
	}
}
