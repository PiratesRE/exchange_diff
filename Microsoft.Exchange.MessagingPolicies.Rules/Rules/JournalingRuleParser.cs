using System;
using System.Xml;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class JournalingRuleParser : TransportRuleParser
	{
		public new static JournalingRuleParser Instance
		{
			get
			{
				return JournalingRuleParser.instance;
			}
		}

		public override Rule CreateRule(string name)
		{
			return new JournalingRule(name);
		}

		public override Action CreateAction(string actionName, ShortList<Argument> arguments, string externalName = null)
		{
			JournalBase journalBase = JournalingRuleParser.CreateAction(actionName, arguments);
			journalBase.ExternalName = externalName;
			return journalBase;
		}

		protected override Rule ParseRuleAttributes(XmlReader reader)
		{
			JournalingRule journalingRule = (JournalingRule)base.ParseRuleAttributes(reader);
			string attribute = reader.GetAttribute("gccType");
			GccType gccRuleType;
			if (string.IsNullOrEmpty(attribute))
			{
				gccRuleType = GccType.None;
			}
			else if (!JournalingRuleConstants.TryParseGccType(attribute, out gccRuleType))
			{
				throw new ParserException(RulesStrings.InvalidAttribute("gccType", "rule", attribute), reader);
			}
			journalingRule.GccRuleType = gccRuleType;
			return journalingRule;
		}

		private static JournalBase CreateAction(string actionName, ShortList<Argument> arguments)
		{
			if (actionName != null)
			{
				if (actionName == "Journal")
				{
					return new Journal(arguments);
				}
				if (actionName == "JournalAndReconcile")
				{
					return new JournalAndReconcile(arguments);
				}
			}
			throw new RulesValidationException(RulesStrings.InvalidActionName(actionName));
		}

		private static readonly JournalingRuleParser instance = new JournalingRuleParser();
	}
}
