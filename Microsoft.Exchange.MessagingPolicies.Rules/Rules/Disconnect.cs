using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class Disconnect : TransportAction
	{
		public Disconnect(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override Version MinimumVersion
		{
			get
			{
				return TransportRuleConstants.VersionedContainerBaseVersion;
			}
		}

		public override string Name
		{
			get
			{
				return "Disconnect";
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			if (transportRulesEvaluationContext.EventType == EventType.EndOfData)
			{
				transportRulesEvaluationContext.EndOfDataSource.Disconnect();
				return ExecutionControl.SkipAll;
			}
			return ExecutionControl.Execute;
		}
	}
}
