using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.MessagingPolicies.HygieneRules
{
	internal class HygieneTransportRuleParser : RuleParser
	{
		public static HygieneTransportRuleParser Instance
		{
			get
			{
				return HygieneTransportRuleParser.instance;
			}
		}

		public override RuleCollection CreateRuleCollection(string name)
		{
			return new RuleCollection(name);
		}

		public override Rule CreateRule(string name)
		{
			return new HygieneTransportRule(name);
		}

		public override Microsoft.Exchange.MessagingPolicies.Rules.Action CreateAction(string actionName, ShortList<Argument> arguments, string externalName = null)
		{
			if (actionName != null)
			{
				Microsoft.Exchange.MessagingPolicies.Rules.Action action;
				if (!(actionName == "Halt"))
				{
					if (!(actionName == "SetHeader"))
					{
						goto IL_35;
					}
					action = new SetHeaderAction(arguments);
				}
				else
				{
					action = new HaltAction(arguments);
				}
				action.ExternalName = externalName;
				return action;
			}
			IL_35:
			throw new RulesValidationException(RulesStrings.InvalidActionName(actionName));
		}

		public override Property CreateProperty(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new RulesValidationException(RulesStrings.EmptyPropertyName);
			}
			if (!propertyName.StartsWith("Message.Headers"))
			{
				return MessageProperty.Create(propertyName);
			}
			if (propertyName["Message.Headers".Length] != ':')
			{
				throw new RulesValidationException(RulesStrings.DelimeterNotFound("Message.Headers"));
			}
			string str = propertyName.Substring("Message.Headers".Length + 1);
			return new HeaderProperty(string.Intern(str));
		}

		public override Property CreateProperty(string propertyName, string typeName)
		{
			return this.CreateProperty(propertyName);
		}

		protected override void CreateRuleSubElements(Rule rule, XmlReader reader, RulesCreationContext creationContext)
		{
			HygieneTransportRule hygieneTransportRule = (HygieneTransportRule)rule;
			List<BifurcationInfo> list = new List<BifurcationInfo>();
			while (base.IsTag(reader, "fork"))
			{
				list.Add(this.ParseFork(reader));
				base.ReadNext(reader);
			}
			if (list.Count > 0)
			{
				hygieneTransportRule.Fork = list;
			}
		}

		private BifurcationInfo ParseFork(XmlReader reader)
		{
			base.VerifyNotEmptyTag(reader);
			BifurcationInfo bifurcationInfo = new BifurcationInfo();
			string attribute = reader.GetAttribute("exception");
			bool exception;
			if (attribute != null && bool.TryParse(attribute, out exception))
			{
				bifurcationInfo.Exception = exception;
			}
			base.ReadNext(reader);
			for (;;)
			{
				if (base.IsTag(reader, "recipientDomainIs"))
				{
					string requiredAttribute = base.GetRequiredAttribute(reader, "value");
					bifurcationInfo.RecipientDomainIs.Add(requiredAttribute);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "recipientDomainIs");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "recipient"))
				{
					string requiredSmtpAddressAttribute = this.GetRequiredSmtpAddressAttribute(reader, "address");
					bifurcationInfo.Recipients.Add(requiredSmtpAddressAttribute);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "recipient");
					}
					base.ReadNext(reader);
				}
				else
				{
					if (!base.IsTag(reader, "list"))
					{
						break;
					}
					string requiredSmtpAddressAttribute2 = this.GetRequiredSmtpAddressAttribute(reader, "name");
					bifurcationInfo.Lists.Add(requiredSmtpAddressAttribute2);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "list");
					}
					base.ReadNext(reader);
				}
			}
			base.VerifyEndTag(reader, "fork");
			return bifurcationInfo;
		}

		private string GetRequiredSmtpAddressAttribute(XmlReader reader, string attributeName)
		{
			string requiredAttribute = base.GetRequiredAttribute(reader, attributeName);
			RoutingAddress routingAddress = new RoutingAddress(requiredAttribute);
			if (!routingAddress.IsValid)
			{
				throw new ParserException(HygieneRulesStrings.InvalidAddress(requiredAttribute), reader);
			}
			return requiredAttribute;
		}

		private static HygieneTransportRuleParser instance = new HygieneTransportRuleParser();
	}
}
