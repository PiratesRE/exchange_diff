using System;
using System.Xml;

namespace Microsoft.Exchange.MessagingPolicies.Rules.OutlookProtection
{
	internal sealed class OutlookProtectionRuleParser : RuleParser
	{
		private OutlookProtectionRuleParser()
		{
		}

		public static OutlookProtectionRuleParser Instance
		{
			get
			{
				return OutlookProtectionRuleParser.instance;
			}
		}

		public override Action CreateAction(string actionName, ShortList<Argument> arguments, string externalName = null)
		{
			if (actionName != null && actionName == "RightsProtectMessage")
			{
				return new RightsProtectMessageAction(arguments);
			}
			throw new ParserException(RulesStrings.InvalidActionName(actionName));
		}

		public override Property CreateProperty(string propertyName, string typeName)
		{
			return new StringProperty(propertyName);
		}

		public override Property CreateProperty(string propertyName)
		{
			return this.CreateProperty(propertyName, null);
		}

		protected override void CreateRuleSubElements(Rule rule, XmlReader reader, RulesCreationContext creationContext)
		{
			OutlookProtectionRule outlookProtectionRule = (OutlookProtectionRule)rule;
			if (base.IsTag(reader, "userOverridable"))
			{
				outlookProtectionRule.UserOverridable = true;
				base.ReadNext(reader);
				return;
			}
			outlookProtectionRule.UserOverridable = false;
		}

		public override PredicateCondition CreatePredicate(string name, Property property, ShortList<string> valueEntries, RulesCreationContext creationContext)
		{
			if (name != null)
			{
				if (!(name == "recipientIs"))
				{
					if (!(name == "allInternal"))
					{
						if (name == "is")
						{
							if (property == null || !string.Equals(property.Name, "Message.Sender.Department", StringComparison.OrdinalIgnoreCase))
							{
								throw new ParserException(RulesStrings.InvalidPropertyName((property != null) ? property.Name : string.Empty));
							}
							return base.CreatePredicate(name, property, valueEntries, creationContext);
						}
					}
					else
					{
						if (property == null || !string.Equals(property.Name, "Message.ToCcBcc", StringComparison.OrdinalIgnoreCase))
						{
							throw new ParserException(RulesStrings.InvalidPropertyName((property != null) ? property.Name : string.Empty));
						}
						return new AllInternalPredicate();
					}
				}
				else
				{
					if (property == null || !string.Equals(property.Name, "Message.ToCcBcc", StringComparison.OrdinalIgnoreCase))
					{
						throw new ParserException(RulesStrings.InvalidPropertyName((property != null) ? property.Name : string.Empty));
					}
					return new RecipientIsPredicate(valueEntries, creationContext);
				}
			}
			return base.CreatePredicate(name, property, valueEntries, creationContext);
		}

		public override Rule CreateRule(string ruleName)
		{
			return new OutlookProtectionRule(ruleName);
		}

		private static readonly OutlookProtectionRuleParser instance = new OutlookProtectionRuleParser();
	}
}
