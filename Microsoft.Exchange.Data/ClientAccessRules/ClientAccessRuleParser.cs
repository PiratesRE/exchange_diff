using System;
using System.Collections.Generic;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class ClientAccessRuleParser : RuleParser
	{
		public static ClientAccessRuleParser Instance
		{
			get
			{
				return ClientAccessRuleParser.instance;
			}
		}

		public override RuleCollection CreateRuleCollection(string name)
		{
			return new ClientAccessRuleCollection(name);
		}

		public override Rule CreateRule(string name)
		{
			return new ClientAccessRule(name);
		}

		public override Microsoft.Exchange.MessagingPolicies.Rules.Action CreateAction(string actionName, ShortList<Argument> arguments, string externalName = null)
		{
			ClientAccessRuleAction clientAccessRuleAction = ClientAccessRuleParser.CreateAction(actionName, arguments);
			clientAccessRuleAction.ExternalName = externalName;
			return clientAccessRuleAction;
		}

		private static ClientAccessRuleAction CreateAction(string actionName, ShortList<Argument> arguments)
		{
			if (string.Compare(actionName, "AllowAccess", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return new ClientAccessRuleAllowAccessAction(arguments);
			}
			if (string.Compare(actionName, "DenyAccess", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return new ClientAccessRuleDenyAccessAction(arguments);
			}
			throw new RulesValidationException(RulesStrings.InvalidActionName(actionName));
		}

		public override Property CreateProperty(string propertyName)
		{
			if (propertyName != null)
			{
				if (propertyName == "ClientIpProperty")
				{
					return new ClientAccessRulesClientIpProperty(propertyName, typeof(string));
				}
				if (propertyName == "SourceTcpPortNumberProperty")
				{
					return new ClientAccessRulesSourcePortNumberProperty(propertyName, typeof(string));
				}
				if (propertyName == "ProtocolProperty")
				{
					return new ClientAccessRulesProtocolProperty(propertyName, typeof(string));
				}
				if (propertyName == "UsernamePatternProperty")
				{
					return new ClientAccessRulesUsernamePatternProperty(propertyName, typeof(string));
				}
				if (propertyName == "AuthenticationTypeProperty")
				{
					return new ClientAccessRulesAuthenticationTypeProperty(propertyName, typeof(string));
				}
				if (propertyName == "UserRecipientFilterProperty")
				{
					return new ClientAccessRulesUserRecipientFilterProperty(propertyName, typeof(string));
				}
			}
			throw new RulesValidationException(RulesStrings.InvalidPropertyName(propertyName));
		}

		public override Property CreateProperty(string propertyName, string typeName)
		{
			if (propertyName != null)
			{
				if (propertyName == "ClientIpProperty")
				{
					return new ClientAccessRulesClientIpProperty(propertyName, Argument.GetTypeForName(typeName));
				}
				if (propertyName == "SourceTcpPortNumberProperty")
				{
					return new ClientAccessRulesSourcePortNumberProperty(propertyName, Argument.GetTypeForName(typeName));
				}
				if (propertyName == "ProtocolProperty")
				{
					return new ClientAccessRulesProtocolProperty(propertyName, Argument.GetTypeForName(typeName));
				}
				if (propertyName == "UsernamePatternProperty")
				{
					return new ClientAccessRulesUsernamePatternProperty(propertyName, Argument.GetTypeForName(typeName));
				}
				if (propertyName == "AuthenticationTypeProperty")
				{
					return new ClientAccessRulesAuthenticationTypeProperty(propertyName, Argument.GetTypeForName(typeName));
				}
				if (propertyName == "UserRecipientFilterProperty")
				{
					return new ClientAccessRulesUserRecipientFilterProperty(propertyName, Argument.GetTypeForName(typeName));
				}
			}
			throw new RulesValidationException(RulesStrings.InvalidPropertyName(propertyName));
		}

		public PredicateCondition CreatePredicate(string name, Property property, IEnumerable<string> valueEntries)
		{
			return base.CreatePredicate(name, property, new ShortList<string>(valueEntries));
		}

		public override PredicateCondition CreatePredicate(string name, Property property, ShortList<string> valueEntries, RulesCreationContext creationContext)
		{
			if (name != null)
			{
				if (name == "anyOfClientIPAddressesOrRangesPredicate")
				{
					return new AnyOfClientIPAddressesOrRangesPredicate(property, valueEntries, creationContext);
				}
				if (name == "anyOfSourceTcpPortNumbersPredicate")
				{
					return new AnyOfSourceTcpPortNumbersPredicate(property, valueEntries, creationContext);
				}
				if (name == "anyOfProtocolsPredicate")
				{
					return new AnyOfProtocolsPredicate(property, valueEntries, creationContext);
				}
				if (name == "usernameMatchesAnyOfPatternsPredicate")
				{
					return new UsernameMatchesAnyOfPatternsPredicate(property, valueEntries, creationContext);
				}
				if (name == "anyOfAuthenticationTypesPredicate")
				{
					return new AnyOfAuthenticationTypesPredicate(property, valueEntries, creationContext);
				}
				if (name == "UserRecipientFilterPredicate")
				{
					return new UserRecipientFilterPredicate(property, valueEntries, creationContext);
				}
			}
			return base.CreatePredicate(name, property, valueEntries, creationContext);
		}

		private static ClientAccessRuleParser instance = new ClientAccessRuleParser();
	}
}
