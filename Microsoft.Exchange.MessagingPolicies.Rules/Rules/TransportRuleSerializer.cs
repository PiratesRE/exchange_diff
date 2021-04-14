using System;
using System.Xml;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class TransportRuleSerializer : RuleSerializer
	{
		public static TransportRuleSerializer Instance
		{
			get
			{
				return TransportRuleSerializer.instance;
			}
		}

		protected override void SaveRuleSubElements(XmlWriter writer, Rule baseRule)
		{
			TransportRule transportRule = (TransportRule)baseRule;
			if (transportRule.Fork != null)
			{
				foreach (RuleBifurcationInfo bifInfo in transportRule.Fork)
				{
					this.SaveFork(writer, bifInfo);
				}
			}
		}

		protected override void SaveProperty(XmlWriter xmlWriter, Property property)
		{
			if (property == null)
			{
				xmlWriter.WriteAttributeString("property", string.Empty);
				return;
			}
			if (property is ExtendedProperty)
			{
				xmlWriter.WriteAttributeString("property", "Message.ExtendedProperties:" + property.Name);
				xmlWriter.WriteAttributeString("type", Argument.GetTypeName(property.Type));
				return;
			}
			if (property is HeaderProperty)
			{
				xmlWriter.WriteAttributeString("property", "Message.Headers:" + property.Name);
				return;
			}
			base.SaveProperty(xmlWriter, property);
		}

		protected override void SaveRuleAttributes(XmlWriter xmlWriter, Rule rule)
		{
			base.SaveRuleAttributes(xmlWriter, rule);
			TransportRule transportRule = rule as TransportRule;
			if (transportRule.SenderAddressLocation != SenderAddressLocation.Header)
			{
				xmlWriter.WriteAttributeString("senderAddressLocation", Enum.GetName(typeof(SenderAddressLocation), transportRule.SenderAddressLocation));
			}
		}

		private void SaveFork(XmlWriter xmlWriter, RuleBifurcationInfo bifInfo)
		{
			xmlWriter.WriteStartElement("fork");
			if (bifInfo.Exception)
			{
				xmlWriter.WriteAttributeString("exception", bifInfo.Exception.ToString());
			}
			foreach (string value in bifInfo.FromRecipients)
			{
				xmlWriter.WriteStartElement("fromRecipient");
				xmlWriter.WriteAttributeString("address", value);
				xmlWriter.WriteEndElement();
			}
			foreach (string value2 in bifInfo.FromLists)
			{
				xmlWriter.WriteStartElement("fromList");
				xmlWriter.WriteAttributeString("name", value2);
				xmlWriter.WriteEndElement();
			}
			foreach (string value3 in bifInfo.Recipients)
			{
				xmlWriter.WriteStartElement("recipient");
				xmlWriter.WriteAttributeString("address", value3);
				xmlWriter.WriteEndElement();
			}
			foreach (string value4 in bifInfo.Managers)
			{
				xmlWriter.WriteStartElement("manager");
				xmlWriter.WriteAttributeString("address", value4);
				xmlWriter.WriteEndElement();
			}
			if (bifInfo.Managers.Count > 0 || bifInfo.ADAttributesForTextMatch.Count > 0)
			{
				string value5 = bifInfo.IsSenderEvaluation ? "true" : "false";
				xmlWriter.WriteStartElement("isSenderEvaluation");
				xmlWriter.WriteAttributeString("value", value5);
				xmlWriter.WriteEndElement();
			}
			foreach (string value6 in bifInfo.ADAttributes)
			{
				xmlWriter.WriteStartElement("adAttribute");
				xmlWriter.WriteAttributeString("value", value6);
				xmlWriter.WriteEndElement();
			}
			foreach (string value7 in bifInfo.RecipientAddressContainsWords)
			{
				xmlWriter.WriteStartElement("recipientContainsWords");
				xmlWriter.WriteAttributeString("value", value7);
				xmlWriter.WriteEndElement();
			}
			foreach (string value8 in bifInfo.RecipientDomainIs)
			{
				xmlWriter.WriteStartElement("recipientDomainIs");
				xmlWriter.WriteAttributeString("value", value8);
				xmlWriter.WriteEndElement();
			}
			foreach (string value9 in bifInfo.RecipientMatchesPatterns)
			{
				xmlWriter.WriteStartElement("recipientMatchesPatterns");
				xmlWriter.WriteAttributeString("value", value9);
				xmlWriter.WriteEndElement();
			}
			foreach (string value10 in bifInfo.RecipientMatchesRegexPatterns)
			{
				xmlWriter.WriteStartElement("recipientMatchesRegexPatterns");
				xmlWriter.WriteAttributeString("value", value10);
				xmlWriter.WriteEndElement();
			}
			foreach (string value11 in bifInfo.RecipientInSenderList)
			{
				xmlWriter.WriteStartElement("recipientInSenderList");
				xmlWriter.WriteAttributeString("value", value11);
				xmlWriter.WriteEndElement();
			}
			foreach (string value12 in bifInfo.SenderInRecipientList)
			{
				xmlWriter.WriteStartElement("senderInRecipientList");
				xmlWriter.WriteAttributeString("value", value12);
				xmlWriter.WriteEndElement();
			}
			foreach (string value13 in bifInfo.RecipientAttributeContains)
			{
				xmlWriter.WriteStartElement("recipientAttributeContains");
				xmlWriter.WriteAttributeString("value", value13);
				xmlWriter.WriteEndElement();
			}
			foreach (string value14 in bifInfo.RecipientAttributeMatches)
			{
				xmlWriter.WriteStartElement("recipientAttributeMatches");
				xmlWriter.WriteAttributeString("value", value14);
				xmlWriter.WriteEndElement();
			}
			foreach (string value15 in bifInfo.RecipientAttributeMatchesRegex)
			{
				xmlWriter.WriteStartElement("recipientAttributeMatchesRegex");
				xmlWriter.WriteAttributeString("value", value15);
				xmlWriter.WriteEndElement();
			}
			foreach (string value16 in bifInfo.ADAttributesForTextMatch)
			{
				xmlWriter.WriteStartElement("adAttributeForTextMatch");
				xmlWriter.WriteAttributeString("value", value16);
				xmlWriter.WriteEndElement();
			}
			if (bifInfo.ADAttributes.Count > 0 || bifInfo.ADAttributesForTextMatch.Count > 0)
			{
				string value17 = bifInfo.CheckADAttributeEquality ? "true" : "false";
				xmlWriter.WriteStartElement("checkADAttributeEquality");
				xmlWriter.WriteAttributeString("value", value17);
				xmlWriter.WriteEndElement();
			}
			if (bifInfo.ADAttributesForTextMatch.Count > 0)
			{
				xmlWriter.WriteStartElement("adAttributeValueForTextMatch");
				xmlWriter.WriteAttributeString("value", bifInfo.ADAttributeValue);
				xmlWriter.WriteEndElement();
			}
			if (!string.IsNullOrEmpty(bifInfo.ManagementRelationship))
			{
				xmlWriter.WriteStartElement("managementRelationship");
				xmlWriter.WriteAttributeString("value", bifInfo.ManagementRelationship);
				xmlWriter.WriteEndElement();
			}
			foreach (string value18 in bifInfo.Lists)
			{
				xmlWriter.WriteStartElement("list");
				xmlWriter.WriteAttributeString("name", value18);
				xmlWriter.WriteEndElement();
			}
			foreach (string value19 in bifInfo.Partners)
			{
				xmlWriter.WriteStartElement("partner");
				xmlWriter.WriteAttributeString("domain", value19);
				xmlWriter.WriteEndElement();
			}
			if (bifInfo.InternalRecipients)
			{
				xmlWriter.WriteStartElement("internal");
				xmlWriter.WriteEndElement();
			}
			if (bifInfo.ExternalRecipients)
			{
				xmlWriter.WriteStartElement("external");
				xmlWriter.WriteEndElement();
			}
			if (bifInfo.ExternalPartnerRecipients)
			{
				xmlWriter.WriteStartElement("externalPartner");
				xmlWriter.WriteEndElement();
			}
			if (bifInfo.ExternalNonPartnerRecipients)
			{
				xmlWriter.WriteStartElement("externalNonPartner");
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
		}

		private static readonly TransportRuleSerializer instance = new TransportRuleSerializer();
	}
}
