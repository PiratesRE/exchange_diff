using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules.PolicyNudges
{
	internal sealed class PolicyNudgeRuleParser
	{
		public static PolicyNudgeRuleParser Instance
		{
			get
			{
				return PolicyNudgeRuleParser.instance;
			}
		}

		public PolicyNudgeRule GetRule(string ruleString, string id, DateTime version)
		{
			ETRToPNRTranslator etrtoPNRTranslator = new ETRToPNRTranslator(ruleString, PolicyNudgeRuleParser.emptyMessageStrings, null, null);
			if (!etrtoPNRTranslator.IsValid)
			{
				return null;
			}
			return new PolicyNudgeRule(ruleString, id, version, etrtoPNRTranslator.Enabled, etrtoPNRTranslator.ActivationDate, etrtoPNRTranslator.ExpiryDate);
		}

		private static readonly PolicyNudgeRuleParser instance = new PolicyNudgeRuleParser();

		private static ETRToPNRTranslator.IMessageStrings emptyMessageStrings = new ETRToPNRTranslator.MessageStringCallbackImpl(string.Empty, (ETRToPNRTranslator.OutlookActionTypes type) => PolicyTipMessage.Empty, () => PolicyTipMessage.Empty);
	}
}
