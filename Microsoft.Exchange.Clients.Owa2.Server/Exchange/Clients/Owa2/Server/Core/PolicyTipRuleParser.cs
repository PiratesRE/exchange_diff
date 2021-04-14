using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class PolicyTipRuleParser : RuleParser
	{
		public static PolicyTipRuleParser Instance
		{
			get
			{
				return PolicyTipRuleParser.instance;
			}
		}

		public override Rule CreateRule(string name)
		{
			return new PolicyTipRule(name);
		}

		protected override void CreateRuleSubElements(Rule rule, XmlReader reader, RulesCreationContext creationContext)
		{
			PolicyTipRule policyTipRule = (PolicyTipRule)rule;
			List<Condition> list = new List<Condition>();
			while (base.IsTag(reader, "fork"))
			{
				list.Add(this.ParseFork(reader, creationContext));
				base.ReadNext(reader);
			}
			if (list.Count > 0)
			{
				policyTipRule.ForkConditions = list;
			}
		}

		public override Microsoft.Exchange.MessagingPolicies.Rules.Action CreateAction(string actionName, ShortList<Argument> arguments, string externalName = null)
		{
			if (actionName != null && actionName == "SenderNotify")
			{
				return new SenderNotify(arguments);
			}
			return new NoopAction(arguments);
		}

		public override Property CreateProperty(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new RulesValidationException(RulesStrings.EmptyPropertyName);
			}
			return MessageProperty.Create(propertyName);
		}

		public override Property CreateProperty(string propertyName, string typeName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new RulesValidationException(RulesStrings.EmptyPropertyName);
			}
			return this.CreateProperty(propertyName);
		}

		public override PredicateCondition CreatePredicate(string name, Property property, ShortList<ShortList<KeyValuePair<string, string>>> valueEntries, RulesCreationContext creationContext)
		{
			if (name == "containsDataClassification")
			{
				return new ContainsDataClassificationPredicate(property, valueEntries, creationContext);
			}
			return new UnconditionalTruePredicate();
		}

		public override PredicateCondition CreatePredicate(string name, Property property, ShortList<string> valueEntries, RulesCreationContext creationContext)
		{
			if (name != null)
			{
				if (name == "isSameUser")
				{
					return new OwaIsSameUserPredicate(property, valueEntries, creationContext);
				}
				if (name == "isMemberOf")
				{
					return new IsMemberOfPredicate(property, valueEntries, creationContext);
				}
			}
			return new UnconditionalTruePredicate();
		}

		private Condition ParseFork(XmlReader reader, RulesCreationContext creationContext)
		{
			base.VerifyNotEmptyTag(reader);
			bool flag = false;
			string attribute = reader.GetAttribute("exception");
			if (attribute != null)
			{
				bool.TryParse(attribute, out flag);
			}
			ShortList<string> shortList = new ShortList<string>();
			ScopeType scopeType = ScopeType.None;
			base.ReadNext(reader);
			for (;;)
			{
				if (base.IsTag(reader, "recipient"))
				{
					string requiredAttribute = base.GetRequiredAttribute(reader, "address");
					shortList.Add(requiredAttribute);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "recipient");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "external"))
				{
					scopeType = ScopeType.External;
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "external");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "internal"))
				{
					scopeType = ScopeType.Internal;
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "internal");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "externalPartner"))
				{
					scopeType = ScopeType.ExternalPartner;
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "externalPartner");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "externalNonPartner"))
				{
					scopeType = ScopeType.ExternalNonPartner;
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "externalNonPartner");
					}
					base.ReadNext(reader);
				}
				else
				{
					if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("fork"))
					{
						break;
					}
					if (reader.NodeType == XmlNodeType.Element)
					{
						if (!reader.IsEmptyElement)
						{
							base.ReadNext(reader);
							base.VerifyEndTag(reader, reader.Name);
						}
						base.ReadNext(reader);
					}
				}
			}
			Condition result = new UnconditionalTruePredicate();
			if (shortList.Count > 0)
			{
				SentToPredicate sentToPredicate = new SentToPredicate(MessageProperty.Create("Message.To"), shortList, creationContext);
				result = (flag ? new NotCondition(sentToPredicate) : sentToPredicate);
			}
			else if (scopeType != ScopeType.None)
			{
				SentToScopePredicate sentToScopePredicate = new SentToScopePredicate(MessageProperty.Create("Message.To"), scopeType, creationContext);
				result = (flag ? new NotCondition(sentToScopePredicate) : sentToScopePredicate);
			}
			return result;
		}

		private static readonly HashSet<string> IgnroedForkTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private static PolicyTipRuleParser instance = new PolicyTipRuleParser();
	}
}
