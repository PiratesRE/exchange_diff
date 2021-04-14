using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;
using Microsoft.Exchange.MessagingPolicies.Rules.Classification;
using Microsoft.Exchange.MessagingPolicies.Rules.PolicyNudges;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.PolicyNudges
{
	internal class SerializerVisitor15 : Visitor15
	{
		private SerializerVisitor15()
		{
		}

		internal static XmlElement Serialize(PolicyNudges15 policyNudges, bool hasChanged, CachedOrganizationConfiguration serverConfig, ADObjectId senderADObjectId, XmlDocument xmlDoc)
		{
			SerializerVisitor15 serializerVisitor = new SerializerVisitor15();
			serializerVisitor.serverConfig = serverConfig;
			serializerVisitor.senderADObjectId = senderADObjectId;
			serializerVisitor.xmlDoc = xmlDoc;
			serializerVisitor.isValidLocale = false;
			if (policyNudges != null)
			{
				serializerVisitor.outlookCultureTag = policyNudges.OutlookLocale;
				serializerVisitor.exchangeLocale = PolicyNudgeConfigurationUtils.GetExchangeLocaleFromOutlookCultureTag(serializerVisitor.outlookCultureTag);
				serializerVisitor.isValidLocale = (serializerVisitor.exchangeLocale != null);
				if (policyNudges.ClassificationItems != null && !string.IsNullOrEmpty(policyNudges.ClassificationItems.EngineVersion))
				{
					serializerVisitor.canOutlookSupportFullPnrXml = PolicyNudgeConfigurationUtils.CanOutlookSupportFullPnrXml(policyNudges.ClassificationItems.EngineVersion);
				}
			}
			serializerVisitor.hasChanged = (hasChanged || !serializerVisitor.isValidLocale);
			new PolicyNudges15().Accept(serializerVisitor);
			return serializerVisitor.returnElement;
		}

		internal override void Visit(PolicyNudges15 policyNudges)
		{
			this.returnElement = ServiceXml.CreateElement(this.xmlDoc, "PolicyNudgeRulesConfiguration", "http://schemas.microsoft.com/exchange/services/2006/messages");
			this.currentElements = new Stack<XmlElement>();
			this.currentElements.Push(this.returnElement);
			new PolicyNudgeRules15().Accept(this);
			new ClassificationItems15().Accept(this);
			this.currentElements.Pop();
		}

		internal override void Visit(PolicyNudgeRules15 policyNudgeRules)
		{
			XmlElement xmlElement = this.xmlDoc.CreateElement("PolicyNudgeRules");
			if (this.hasChanged && this.isValidLocale)
			{
				foreach (PolicyNudgeRule policyNudgeRule in from rule in this.serverConfig.PolicyNudgeRules.Rules
				where rule.IsEnabled && (this.canOutlookSupportFullPnrXml || rule.IsPnrXmlValid)
				select rule)
				{
					PolicyNudgeConfigurationUtils.AdDistributionListResolver distributionListResolver = new PolicyNudgeConfigurationUtils.AdDistributionListResolver(this.serverConfig, this.senderADObjectId);
					PolicyNudgeConfigurationUtils.DataClassificationResolver dataClassificationResolver = new PolicyNudgeConfigurationUtils.DataClassificationResolver(this.serverConfig);
					PolicyNudgeRule.References references;
					string ruleXml = policyNudgeRule.GetRuleXml(this.exchangeLocale, new PolicyNudgeConfigurationUtils.AdMessageStrings(this.serverConfig, this.outlookCultureTag), distributionListResolver, dataClassificationResolver, this.canOutlookSupportFullPnrXml, out references);
					if (!string.IsNullOrEmpty(ruleXml))
					{
						XmlElement xmlElement2 = this.xmlDoc.CreateElement("PolicyNudgeRule");
						xmlElement2.SetAttribute("id", policyNudgeRule.ID);
						xmlElement2.SetAttribute("version", policyNudgeRule.Version.ToBinary().ToString());
						xmlElement2.InnerXml = ETRToPNRTranslator.Evaluate(ruleXml, distributionListResolver);
						OtherAttribtuesUtils.ApplyVersionedItems(xmlElement2, "messageString", references.Messages);
						OtherAttribtuesUtils.ApplyVersionedItems(xmlElement2, "distributionList", references.DistributionLists);
						xmlElement.AppendChild(xmlElement2);
					}
				}
			}
			PolicyNudgeConfigurationUtils.MarkElementAsApply(xmlElement, this.hasChanged);
			this.currentElements.Peek().AppendChild(xmlElement);
		}

		internal override void Visit(PolicyNudgeRule15 policyNudgeRule)
		{
			throw new NotImplementedException();
		}

		internal override void Visit(ClassificationItems15 classificationItems)
		{
			new ClassificationDefinitions15().Accept(this);
		}

		internal override void Visit(ClassificationDefinitions15 classificationDefinitions)
		{
			XmlElement xmlElement = this.xmlDoc.CreateElement("ClassificationDefinitions");
			this.SerializeServerItems<ClassificationRulePackage>(xmlElement, "ClassificationDefinition", this.serverConfig.ClassificationDefinitions, (ClassificationRulePackage r) => r.ID, (ClassificationRulePackage r) => r.Version, (ClassificationRulePackage r) => r.RuleXml);
			this.currentElements.Peek().AppendChild(xmlElement);
		}

		internal override void Visit(ClassificationDefinition15 classificationDefinition)
		{
			throw new NotImplementedException();
		}

		private void SerializeServerItems<T>(XmlElement itemsElement, string itemElementName, IEnumerable<T> serverConfigItems, Func<T, string> id, Func<T, DateTime> version, Func<T, string> xml)
		{
			if (this.hasChanged && this.isValidLocale)
			{
				foreach (T arg in serverConfigItems)
				{
					XmlElement xmlElement = this.xmlDoc.CreateElement(itemElementName);
					xmlElement.SetAttribute("id", id(arg));
					xmlElement.SetAttribute("version", version(arg).ToBinary().ToString());
					xmlElement.InnerXml = xml(arg);
					itemsElement.AppendChild(xmlElement);
				}
			}
			PolicyNudgeConfigurationUtils.MarkElementAsApply(itemsElement, this.hasChanged);
		}

		private bool hasChanged;

		private bool isValidLocale;

		private CachedOrganizationConfiguration serverConfig;

		private ADObjectId senderADObjectId;

		private string outlookCultureTag;

		private string exchangeLocale;

		private XmlDocument xmlDoc;

		private XmlElement returnElement;

		private Stack<XmlElement> currentElements;

		private bool canOutlookSupportFullPnrXml;
	}
}
