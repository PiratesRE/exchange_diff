using System;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class Quarantine : TransportAction
	{
		public Quarantine(ShortList<Argument> arguments) : base(arguments)
		{
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
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			if (transportRulesEvaluationContext.EventType == EventType.EndOfData)
			{
				transportRulesEvaluationContext.EndOfDataSource.Quarantine(null, "Quarantined by rule agent");
				transportRulesEvaluationContext.MessageQuarantined = true;
				return ExecutionControl.SkipAll;
			}
			if (VariantConfiguration.InvariantNoFlightingSnapshot.CompliancePolicy.QuarantineAction.Enabled)
			{
				CommonUtils.StampPutInQuarantineHeader(transportRulesEvaluationContext.MailItem.Message, QuarantineFlavor.Policy, 0, false);
				transportRulesEvaluationContext.MessageQuarantined = true;
				return ExecutionControl.SkipAll;
			}
			return ExecutionControl.Execute;
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
				return "Quarantine";
			}
		}
	}
}
