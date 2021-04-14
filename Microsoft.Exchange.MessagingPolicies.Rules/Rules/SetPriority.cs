using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class SetPriority : TransportAction
	{
		public SetPriority(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "SetPriority";
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			return ExecutionControl.Execute;
		}
	}
}
