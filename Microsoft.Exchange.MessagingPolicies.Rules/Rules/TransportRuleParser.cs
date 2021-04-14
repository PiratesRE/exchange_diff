using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class TransportRuleParser : RuleParser
	{
		public static TransportRuleParser Instance
		{
			get
			{
				return TransportRuleParser.instance;
			}
		}

		public override RuleCollection CreateRuleCollection(string name)
		{
			return new TransportRuleCollection(name);
		}

		public override Rule CreateRule(string name)
		{
			return new TransportRule(name);
		}

		protected override void CreateRuleSubElements(Rule rule, XmlReader reader, RulesCreationContext creationContext)
		{
			TransportRule transportRule = (TransportRule)rule;
			List<RuleBifurcationInfo> list = new List<RuleBifurcationInfo>();
			while (base.IsTag(reader, "fork"))
			{
				list.Add(this.ParseFork(reader));
				base.ReadNext(reader);
			}
			if (list.Count > 0)
			{
				transportRule.Fork = list;
			}
		}

		protected override Rule ParseRuleAttributes(XmlReader reader)
		{
			TransportRule transportRule = (TransportRule)base.ParseRuleAttributes(reader);
			string attribute = reader.GetAttribute("senderAddressLocation");
			SenderAddressLocation senderAddressLocation = RuleConstants.TryParseEnum<SenderAddressLocation>(attribute, SenderAddressLocation.Header);
			transportRule.SenderAddressLocation = senderAddressLocation;
			return transportRule;
		}

		public override Action CreateAction(string actionName, ShortList<Argument> arguments, string externalName = null)
		{
			TransportAction transportAction = TransportRuleParser.CreateAction(actionName, arguments);
			transportAction.ExternalName = externalName;
			return transportAction;
		}

		private static TransportAction CreateAction(string actionName, ShortList<Argument> arguments)
		{
			switch (actionName)
			{
			case "DeleteMessage":
				return new DeleteMessage(arguments);
			case "RejectMessage":
				return new RejectMessage(arguments);
			case "Disconnect":
				return new Disconnect(arguments);
			case "Quarantine":
				return new Quarantine(arguments);
			case "SetExtendedPropertyString":
				return new SetExtendedPropertyString(arguments);
			case "LogEvent":
				return new LogEvent(arguments);
			case "SetPriority":
				return new SetPriority(arguments);
			case "AddToRecipient":
				return new AddToRecipient(arguments);
			case "AddToRecipientSmtpOnly":
				return new AddToRecipientSmtpOnly(arguments);
			case "AddCcRecipient":
				return new AddCcRecipient(arguments);
			case "AddCcRecipientSmtpOnly":
				return new AddCcRecipientSmtpOnly(arguments);
			case "ModerateMessageByUser":
				return new ModerateMessageByUser(arguments);
			case "ModerateMessageByManager":
				return new ModerateMessageByManager(arguments);
			case "AddManagerAsRecipientType":
				return new AddManagerAsRecipientType(arguments);
			case "AddEnvelopeRecipient":
				return new AddEnvelopeRecipient(arguments);
			case "RedirectMessage":
				return new RedirectMessage(arguments);
			case "RemoveHeader":
				return new RemoveHeader(arguments);
			case "SetSubject":
				return new SetSubject(arguments);
			case "PrependSubject":
				return new PrependSubject(arguments);
			case "SenderNotify":
				return new SenderNotify(arguments);
			case "Halt":
				return new Halt(arguments);
			case "AddHeader":
				return new AddHeader(arguments);
			case "SetHeader":
				return new SetHeader(arguments);
			case "SetHeaderUniqueValue":
				return new SetHeaderUniqueValue(arguments);
			case "ApplyHtmlDisclaimer":
				return new ApplyHtmlDisclaimer(arguments);
			case "ApplyDisclaimer":
				return new ApplyDisclaimer(arguments);
			case "ApplyDisclaimerWithSeparator":
				return new ApplyDisclaimerWithSeparator(arguments);
			case "ApplyDisclaimerWithSeparatorAndReadingOrder":
				return new ApplyDisclaimerWithSeparatorAndReadingOrder(arguments);
			case "RightsProtectMessage":
				return new RightsProtectMessage(arguments);
			case "AuditSeverityLevel":
				return new AuditSeverityLevelAction(arguments);
			case "GenerateIncidentReport":
				return new GenerateIncidentReport(arguments);
			case "RouteMessageOutboundConnector":
				return new RouteMessageOutboundConnector(arguments);
			case "RouteMessageOutboundRequireTls":
				return new RouteMessageOutboundRequireTls(arguments);
			case "ApplyOME":
				return new ApplyOME(arguments);
			case "RemoveOME":
				return new RemoveOME(arguments);
			case "GenerateNotification":
				return new GenerateNotification(arguments);
			}
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
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new RulesValidationException(RulesStrings.EmptyPropertyName);
			}
			if (propertyName.StartsWith("Message.ExtendedProperties"))
			{
				if (propertyName["Message.ExtendedProperties".Length] != ':')
				{
					throw new RulesValidationException(RulesStrings.DelimeterNotFound("Message.ExtendedProperties"));
				}
				string str = propertyName.Substring("Message.ExtendedProperties".Length + 1);
				Type typeForName = Argument.GetTypeForName(typeName);
				return new ExtendedProperty(string.Intern(str), typeForName);
			}
			else
			{
				if (!string.IsNullOrEmpty(typeName))
				{
					throw new RulesValidationException(RulesStrings.PropertyTypeIsFixed(propertyName));
				}
				return this.CreateProperty(propertyName);
			}
		}

		public override PredicateCondition CreatePredicate(string name, Property property, ShortList<ShortList<KeyValuePair<string, string>>> valueEntries, RulesCreationContext creationContext)
		{
			if (name == "containsDataClassification")
			{
				return new ContainsDataClassificationPredicate(property, valueEntries, creationContext);
			}
			return base.CreatePredicate(name, property, valueEntries, creationContext);
		}

		public override PredicateCondition CreatePredicate(string name, Property property, ShortList<string> valueEntries, RulesCreationContext creationContext)
		{
			switch (name)
			{
			case "isSameUser":
				return new IsSameUserPredicate(property, valueEntries, creationContext);
			case "isMemberOf":
				return new IsMemberOfPredicate(property, valueEntries, creationContext);
			case "isPartner":
				return new IsPartnerPredicate(property, valueEntries, creationContext);
			case "isInternal":
				return new IsInternalPredicate(property, valueEntries, creationContext);
			case "isExternalPartner":
				return new IsExternalPartnerPredicate(property, valueEntries, creationContext);
			case "isMessageType":
				return new IsMessageTypePredicate(valueEntries, creationContext);
			case "senderAttributeContains":
				return new SenderAttributeContainsPredicate(valueEntries, creationContext);
			case "senderAttributeMatches":
				return new SenderAttributeMatchesPredicate(valueEntries, creationContext);
			case "senderAttributeMatchesRegex":
				return new SenderAttributeMatchesRegexPredicate(valueEntries, creationContext);
			case "attachmentContainsWords":
				return new AttachmentContainsWordsPredicate(valueEntries, creationContext);
			case "attachmentMatchesPatterns":
				return new AttachmentMatchesPatternsPredicate(valueEntries, creationContext);
			case "attachmentMatchesRegexPatterns":
				return new AttachmentMatchesRegexPredicate(valueEntries, creationContext);
			case "attachmentPropertyContains":
				return new AttachmentPropertyContainsPredicate(valueEntries, creationContext);
			case "attachmentIsUnsupported":
				return new AttachmentIsUnsupportedPredicate(valueEntries, creationContext);
			case "attachmentProcessingLimitExceeded":
				return new AttachmentProcessingLimitExceededPredicate(valueEntries, creationContext);
			case "attachmentIsPasswordProtected":
				return new AttachmentIsPasswordProtectedPredicate(valueEntries, creationContext);
			case "hasSenderOverride":
				return new HasSenderOverridePredicate(property, valueEntries, creationContext);
			case "ipMatch":
				return new IpMatchPredicate(property, valueEntries, creationContext);
			case "domainIs":
				return new DomainIsPredicate(property, valueEntries, creationContext);
			}
			return base.CreatePredicate(name, property, valueEntries, creationContext);
		}

		private RuleBifurcationInfo ParseFork(XmlReader reader)
		{
			base.VerifyNotEmptyTag(reader);
			RuleBifurcationInfo ruleBifurcationInfo = new RuleBifurcationInfo();
			string attribute = reader.GetAttribute("exception");
			bool exception;
			if (attribute != null && bool.TryParse(attribute, out exception))
			{
				ruleBifurcationInfo.Exception = exception;
			}
			base.ReadNext(reader);
			for (;;)
			{
				if (base.IsTag(reader, "manager"))
				{
					string requiredSmtpAddressAttribute = this.GetRequiredSmtpAddressAttribute(reader, "address");
					ruleBifurcationInfo.Managers.Add(requiredSmtpAddressAttribute);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "manager");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "adAttribute"))
				{
					string requiredAttribute = base.GetRequiredAttribute(reader, "value");
					ruleBifurcationInfo.ADAttributes.Add(requiredAttribute);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "adAttribute");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "adAttributeForTextMatch"))
				{
					string requiredAttribute2 = base.GetRequiredAttribute(reader, "value");
					ruleBifurcationInfo.ADAttributesForTextMatch.Add(requiredAttribute2);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "adAttributeForTextMatch");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "recipientContainsWords"))
				{
					string requiredAttribute3 = base.GetRequiredAttribute(reader, "value");
					ruleBifurcationInfo.RecipientAddressContainsWords.Add(requiredAttribute3);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "recipientContainsWords");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "recipientDomainIs"))
				{
					string requiredAttribute4 = base.GetRequiredAttribute(reader, "value");
					ruleBifurcationInfo.RecipientDomainIs.Add(requiredAttribute4);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "recipientDomainIs");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "recipientMatchesPatterns"))
				{
					string requiredAttribute5 = base.GetRequiredAttribute(reader, "value");
					ruleBifurcationInfo.RecipientMatchesPatterns.Add(requiredAttribute5);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "recipientMatchesPatterns");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "recipientMatchesRegexPatterns"))
				{
					string requiredAttribute6 = base.GetRequiredAttribute(reader, "value");
					ruleBifurcationInfo.RecipientMatchesRegexPatterns.Add(requiredAttribute6);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "recipientMatchesRegexPatterns");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "recipientAttributeContains"))
				{
					string requiredAttribute7 = base.GetRequiredAttribute(reader, "value");
					ruleBifurcationInfo.RecipientAttributeContains.Add(requiredAttribute7);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "recipientAttributeContains");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "recipientAttributeMatches"))
				{
					string requiredAttribute8 = base.GetRequiredAttribute(reader, "value");
					ruleBifurcationInfo.RecipientAttributeMatches.Add(requiredAttribute8);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "recipientAttributeMatches");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "recipientAttributeMatchesRegex"))
				{
					string requiredAttribute9 = base.GetRequiredAttribute(reader, "value");
					ruleBifurcationInfo.RecipientAttributeMatchesRegex.Add(requiredAttribute9);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "recipientAttributeMatchesRegex");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "recipientInSenderList"))
				{
					string requiredAttribute10 = base.GetRequiredAttribute(reader, "value");
					ruleBifurcationInfo.RecipientInSenderList.Add(requiredAttribute10);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "recipientInSenderList");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "senderInRecipientList"))
				{
					string requiredAttribute11 = base.GetRequiredAttribute(reader, "value");
					ruleBifurcationInfo.SenderInRecipientList.Add(requiredAttribute11);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "senderInRecipientList");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "adAttributeValueForTextMatch"))
				{
					string requiredAttribute12 = base.GetRequiredAttribute(reader, "value");
					ruleBifurcationInfo.ADAttributeValue = requiredAttribute12;
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "adAttributeValueForTextMatch");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "recipient"))
				{
					string requiredSmtpAddressAttribute2 = this.GetRequiredSmtpAddressAttribute(reader, "address");
					ruleBifurcationInfo.Recipients.Add(requiredSmtpAddressAttribute2);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "recipient");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "fromRecipient"))
				{
					string requiredSmtpAddressAttribute3 = this.GetRequiredSmtpAddressAttribute(reader, "address");
					ruleBifurcationInfo.FromRecipients.Add(requiredSmtpAddressAttribute3);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "fromRecipient");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "list"))
				{
					string requiredSmtpAddressAttribute4 = this.GetRequiredSmtpAddressAttribute(reader, "name");
					ruleBifurcationInfo.Lists.Add(requiredSmtpAddressAttribute4);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "list");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "fromList"))
				{
					string requiredSmtpAddressAttribute5 = this.GetRequiredSmtpAddressAttribute(reader, "name");
					ruleBifurcationInfo.FromLists.Add(requiredSmtpAddressAttribute5);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "fromList");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "isSenderEvaluation"))
				{
					string requiredAttribute13 = base.GetRequiredAttribute(reader, "value");
					ruleBifurcationInfo.IsSenderEvaluation = string.Equals(requiredAttribute13, "true", StringComparison.InvariantCultureIgnoreCase);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "isSenderEvaluation");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "checkADAttributeEquality"))
				{
					string requiredAttribute14 = base.GetRequiredAttribute(reader, "value");
					ruleBifurcationInfo.CheckADAttributeEquality = string.Equals(requiredAttribute14, "true", StringComparison.InvariantCultureIgnoreCase);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "checkADAttributeEquality");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "managementRelationship"))
				{
					ruleBifurcationInfo.ManagementRelationship = base.GetRequiredAttribute(reader, "value");
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "managementRelationship");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "partner"))
				{
					string requiredAttribute15 = base.GetRequiredAttribute(reader, "domain");
					ruleBifurcationInfo.Partners.Add(requiredAttribute15);
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "partner");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "external"))
				{
					ruleBifurcationInfo.ExternalRecipients = true;
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "external");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "internal"))
				{
					ruleBifurcationInfo.InternalRecipients = true;
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "internal");
					}
					base.ReadNext(reader);
				}
				else if (base.IsTag(reader, "externalPartner"))
				{
					ruleBifurcationInfo.ExternalPartnerRecipients = true;
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "externalPartner");
					}
					base.ReadNext(reader);
				}
				else
				{
					if (!base.IsTag(reader, "externalNonPartner"))
					{
						break;
					}
					ruleBifurcationInfo.ExternalNonPartnerRecipients = true;
					if (!reader.IsEmptyElement)
					{
						base.ReadNext(reader);
						base.VerifyEndTag(reader, "externalNonPartner");
					}
					base.ReadNext(reader);
				}
			}
			base.VerifyEndTag(reader, "fork");
			if (ruleBifurcationInfo.RecipientMatchesPatterns.Count > 0)
			{
				foreach (string legacyPattern in ruleBifurcationInfo.RecipientMatchesPatterns)
				{
					ruleBifurcationInfo.Patterns.Add(RegexUtils.ConvertLegacyRegexToTpl(legacyPattern));
				}
			}
			if (ruleBifurcationInfo.RecipientMatchesRegexPatterns.Count > 0)
			{
				foreach (string item in ruleBifurcationInfo.RecipientMatchesRegexPatterns)
				{
					ruleBifurcationInfo.Patterns.Add(item);
				}
			}
			if (ruleBifurcationInfo.RecipientAttributeContains.Count > 0)
			{
				foreach (string item2 in ruleBifurcationInfo.RecipientAttributeContains)
				{
					ruleBifurcationInfo.Patterns.Add(item2);
				}
			}
			if (ruleBifurcationInfo.RecipientAttributeMatches.Count > 0)
			{
				IEnumerable<string> enumerable = TransportUtils.BuildPatternListForUserAttributeMatchesPredicate(ruleBifurcationInfo.RecipientAttributeMatches);
				foreach (string legacyPattern2 in enumerable)
				{
					ruleBifurcationInfo.Patterns.Add(RegexUtils.ConvertLegacyRegexToTpl(legacyPattern2));
				}
			}
			if (ruleBifurcationInfo.RecipientAttributeMatchesRegex.Count > 0)
			{
				IEnumerable<string> enumerable2 = TransportUtils.BuildPatternListForUserAttributeMatchesPredicate(ruleBifurcationInfo.RecipientAttributeMatchesRegex);
				foreach (string item3 in enumerable2)
				{
					ruleBifurcationInfo.Patterns.Add(item3);
				}
			}
			return ruleBifurcationInfo;
		}

		private string GetRequiredSmtpAddressAttribute(XmlReader reader, string attributeName)
		{
			string requiredAttribute = base.GetRequiredAttribute(reader, attributeName);
			RoutingAddress routingAddress = new RoutingAddress(requiredAttribute);
			if (!routingAddress.IsValid)
			{
				throw new ParserException(TransportRulesStrings.InvalidAddress(requiredAttribute), reader);
			}
			return requiredAttribute;
		}

		private static TransportRuleParser instance = new TransportRuleParser();
	}
}
