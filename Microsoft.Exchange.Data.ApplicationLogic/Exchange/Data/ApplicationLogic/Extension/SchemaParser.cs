using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal abstract class SchemaParser
	{
		public SchemaParser(SafeXmlDocument xmlDoc, ExtensionInstallScope extensionInstallScope)
		{
			this.xmlDoc = xmlDoc;
			this.extensionInstallScope = extensionInstallScope;
			this.namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
			this.namespaceManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
			this.namespaceManager.AddNamespace(this.GetOweNamespacePrefix(), this.GetOweNamespaceUri());
			this.xpathPrefix = "//" + this.GetOweNamespacePrefix() + ":";
			this.oweNameSpacePrefixWithSemiColon = this.GetOweNamespacePrefix() + ":";
		}

		public abstract Version SchemaVersion { get; }

		public ExtensionInstallScope ExtensionInstallScope
		{
			get
			{
				return this.extensionInstallScope;
			}
			set
			{
				this.extensionInstallScope = value;
			}
		}

		private static bool ParseBoolFromXmlAttribute(XmlAttribute attribute)
		{
			return attribute != null && ExtensionDataHelper.ConvertXmlStringToBoolean(attribute.Value);
		}

		private static Uri ValidateUrl(ExtensionInstallScope extensionScope, string name, string url)
		{
			Uri uri;
			if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
			{
				throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonInvalidUrlValue(name, url)));
			}
			if (uri.IsAbsoluteUri)
			{
				if (!string.Equals(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
				{
					throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonUnsupportedUrlScheme(name, url)));
				}
			}
			else if (ExtensionInstallScope.Default != extensionScope)
			{
				throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonUrlMustBeAbsolute(name, url)));
			}
			return uri;
		}

		private static void ValidateRegEx(string regexName, string regexPattern, string ruleName, string attributeName)
		{
			try
			{
				ExtensionDataHelper.ValidateRegex(regexPattern);
			}
			catch (Exception ex)
			{
				SchemaParser.Tracer.TraceError(0L, "Failed to validate {0} rule's {1} of name '{2}' and value '{3}' with exception: '{4}'", new object[]
				{
					ruleName,
					attributeName,
					regexName,
					regexPattern,
					ex
				});
				throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonInvalidRegEx(ruleName, attributeName)), ex);
			}
		}

		private static void ValidateItemIsRule(XmlNode xmlNode)
		{
			XmlAttribute xmlAttribute = xmlNode.Attributes["ItemClass"];
			if (xmlAttribute != null)
			{
				string value = xmlAttribute.Value;
				if (ObjectClass.IsSmime(value) || ObjectClass.IsRightsManagedContentClass(value))
				{
					throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonItemTypeInvalid));
				}
				XmlAttribute xmlAttribute2 = xmlNode.Attributes["IncludeSubClasses"];
				if (string.Equals(value, "IPM", StringComparison.OrdinalIgnoreCase) && xmlAttribute2 != null && ExtensionDataHelper.ConvertXmlStringToBoolean(xmlAttribute2.Value))
				{
					throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonItemTypeAllTypes));
				}
			}
			XmlAttribute xmlAttribute3 = xmlNode.Attributes["FormType"];
			ItemIsRuleFormType itemIsRuleFormType;
			if (xmlAttribute3 != null && EnumValidator.TryParse<ItemIsRuleFormType>(xmlAttribute3.Value, EnumParseOptions.Default, out itemIsRuleFormType) && (itemIsRuleFormType == ItemIsRuleFormType.Edit || itemIsRuleFormType == ItemIsRuleFormType.ReadOrEdit))
			{
				XmlAttribute xmlAttribute4 = xmlNode.Attributes["IncludeSubClasses"];
				if (xmlAttribute != null || xmlAttribute4 != null)
				{
					throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonItemIsRuleAttributesNotValidForEdit));
				}
			}
		}

		private static void ValidateItemHasKnownEntityRule(XmlNode xmlNode, RequestedCapabilities requestedCapabilities, ref HashSet<string> entitiesRegExNames, ref int regexCount)
		{
			if (requestedCapabilities == RequestedCapabilities.Restricted)
			{
				XmlAttribute attribute = xmlNode.Attributes["EntityType"];
				string item;
				if (!ExtensionDataHelper.TryGetAttributeValue(attribute, out item) || !SchemaParser.AllowedEntityTypesInRestricted.Contains(item))
				{
					throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonOnlySelectedEntitiesInRestricted));
				}
			}
			XmlAttribute attribute2 = xmlNode.Attributes["RegExFilter"];
			XmlAttribute attribute3 = xmlNode.Attributes["FilterName"];
			string text;
			bool flag = ExtensionDataHelper.TryGetAttributeValue(attribute3, out text);
			string regexPattern;
			bool flag2 = ExtensionDataHelper.TryGetAttributeValue(attribute2, out regexPattern);
			if (flag != flag2)
			{
				throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonRegExNameAndValueRequiredInEntitiesRules));
			}
			if (!flag)
			{
				XmlAttribute attribute4 = xmlNode.Attributes["IgnoreCase"];
				string text2;
				if (ExtensionDataHelper.TryGetAttributeValue(attribute4, out text2))
				{
					throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonIgnoreCaseWithoutRegExInEntitiesRules));
				}
				return;
			}
			else
			{
				regexCount++;
				if (entitiesRegExNames == null)
				{
					entitiesRegExNames = new HashSet<string>();
				}
				else if (regexCount > 5)
				{
					throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonTooManyRegexRule(5)));
				}
				if (!entitiesRegExNames.Add(text))
				{
					throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonMultipleRulesWithSameFilterName(text)));
				}
				SchemaParser.ValidateRegEx(text, regexPattern, "ItemHasKnownEntity", "RegExFilter");
				return;
			}
		}

		public abstract Version GetMinApiVersion();

		public string GetAndValidateExtensionId()
		{
			string tagStringValue = ExtensionData.GetTagStringValue(this.xmlDoc, this.GetOweXpath("Id"), this.namespaceManager);
			string text = ExtensionDataHelper.FormatExtensionId(tagStringValue);
			Guid guid;
			if (!GuidHelper.TryParseGuid(text, out guid))
			{
				throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonInvalidID));
			}
			return text;
		}

		public RequestedCapabilities GetRequestedCapabilities()
		{
			return ExtensionData.GetEnumTagValue<RequestedCapabilities>(this.xmlDoc, this.GetOweXpath("Permissions"), this.namespaceManager);
		}

		public Uri GetAndValidateIconUrl(CultureInfo culture)
		{
			return this.GetAndValidateUrls("IconUrl", culture);
		}

		public Uri GetAndValidateHighResolutionIconUrl(CultureInfo culture)
		{
			return this.GetAndValidateUrls("HighResolutionIconUrl", culture);
		}

		private Uri GetAndValidateUrls(string elementName, CultureInfo culture)
		{
			Uri result = null;
			XmlNode xmlNode = this.xmlDoc.SelectSingleNode(this.GetOweXpath(elementName), this.namespaceManager);
			if (xmlNode != null)
			{
				string attributeStringValue = ExtensionData.GetAttributeStringValue(xmlNode, "DefaultValue");
				result = SchemaParser.ValidateUrl(this.extensionInstallScope, elementName, attributeStringValue);
				using (XmlNodeList xmlNodeList = xmlNode.SelectNodes(this.GetOweChildPath("Override"), this.namespaceManager))
				{
					string name = elementName + " " + "Override";
					foreach (object obj in xmlNodeList)
					{
						XmlNode xmlNode2 = (XmlNode)obj;
						string attributeStringValue2 = ExtensionData.GetAttributeStringValue(xmlNode2, "Value");
						Uri uri = SchemaParser.ValidateUrl(this.extensionInstallScope, name, attributeStringValue2);
						string attributeStringValue3 = ExtensionData.GetAttributeStringValue(xmlNode2, "Locale");
						if (string.Compare(culture.ToString(), attributeStringValue3, StringComparison.OrdinalIgnoreCase) == 0)
						{
							result = uri;
						}
					}
				}
			}
			return result;
		}

		public void ValidateSourceLocations()
		{
			string oweXpath = this.GetOweXpath("SourceLocation");
			string xpath = oweXpath + "/" + this.GetOweChildPath("Override");
			this.ValidateSourceLocationUrls(oweXpath, "DefaultValue", "SourceLocation");
			this.ValidateSourceLocationUrls(xpath, "Value", "SourceLocation Override");
		}

		public void ValidateRules()
		{
			RequestedCapabilities requestedCapabilities = this.GetRequestedCapabilities();
			using (XmlNodeList xmlNodeList = this.xmlDoc.SelectNodes(this.GetOweXpath("Rule"), this.namespaceManager))
			{
				int num = 0;
				int num2 = 0;
				HashSet<string> hashSet = null;
				HashSet<string> hashSet2 = null;
				int num3 = 0;
				foreach (object obj in xmlNodeList)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlAttribute attribute = xmlNode.Attributes["type", "http://www.w3.org/2001/XMLSchema-instance"];
					string a;
					if (ExtensionDataHelper.TryGetNameSpaceStrippedAttributeValue(attribute, out a))
					{
						if (string.Equals(a, "RuleCollection", StringComparison.Ordinal))
						{
							num2++;
						}
						else
						{
							num++;
						}
						if (num > 15 || num2 > 15)
						{
							throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonTooManyRule(15)));
						}
						if (string.Equals(a, "ItemIs", StringComparison.Ordinal))
						{
							SchemaParser.ValidateItemIsRule(xmlNode);
						}
						else if (string.Equals(a, "ItemHasKnownEntity", StringComparison.Ordinal))
						{
							SchemaParser.ValidateItemHasKnownEntityRule(xmlNode, requestedCapabilities, ref hashSet2, ref num3);
						}
						else if (string.Equals(a, "ItemHasRegularExpressionMatch", StringComparison.Ordinal))
						{
							num3++;
							if (hashSet == null)
							{
								hashSet = new HashSet<string>();
							}
							string attributeStringValue = ExtensionData.GetAttributeStringValue(xmlNode, "RegExName");
							if (!hashSet.Add(attributeStringValue))
							{
								throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonMultipleRulesWithSameRegExName(attributeStringValue)));
							}
							if (requestedCapabilities == RequestedCapabilities.Restricted)
							{
								throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonNoRegexRuleInRestricted));
							}
							if (num3 > 5)
							{
								throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonTooManyRegexRule(5)));
							}
							SchemaParser.ValidateRegEx(attributeStringValue, ExtensionData.GetAttributeStringValue(xmlNode, "RegExValue"), "ItemHasRegularExpressionMatch", "RegExValue");
						}
					}
				}
			}
		}

		public string GetOweStringElement(string elementName)
		{
			return ExtensionData.GetTagStringValue(this.xmlDoc, this.GetOweXpath(elementName), this.namespaceManager);
		}

		public void ValidateHosts()
		{
			XmlNode oweXmlNode = this.GetOweXmlNode("Hosts");
			if (oweXmlNode == null)
			{
				return;
			}
			bool flag = false;
			foreach (object obj in oweXmlNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					string attributeStringValue = ExtensionData.GetAttributeStringValue(xmlNode, "Name");
					if ("Mailbox".Equals(attributeStringValue, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonNoMailboxInHosts));
			}
		}

		public bool GetDisableEntityHighlighting()
		{
			bool flag = false;
			XmlNode oweXmlNode = this.GetOweXmlNode("DisableEntityHighlighting");
			return oweXmlNode != null && bool.TryParse(oweXmlNode.Value, out flag) && flag;
		}

		public XmlNode GetOweXmlNode(string elementName)
		{
			return this.xmlDoc.SelectSingleNode(this.GetOweXpath(elementName), this.namespaceManager);
		}

		public XmlNode GetMandatoryOweXmlNode(string elementName)
		{
			XmlNode xmlNode = this.xmlDoc.SelectSingleNode(this.GetOweXpath(elementName), this.namespaceManager);
			if (xmlNode == null)
			{
				throw new OwaExtensionOperationException(Strings.FailureReasonTagMissing(elementName));
			}
			return xmlNode;
		}

		public string GetIdForTokenRequests()
		{
			XmlNode xmlNode = this.xmlDoc.SelectSingleNode(this.GetOweXpath("SourceLocation"), this.namespaceManager);
			if (xmlNode == null)
			{
				SchemaParser.Tracer.TraceError(0L, "SourceLocation tag is missing from the given node.");
				throw new OwaExtensionOperationException(Strings.ErrorCanNotReadInstalledList(Strings.FailureReasonSourceLocationTagMissing));
			}
			return ExtensionData.GetAttributeStringValue(xmlNode, "DefaultValue");
		}

		public string GetOweLocaleAwareSetting(string elementName, CultureInfo culture)
		{
			XmlNode xmlNode = this.xmlDoc.SelectSingleNode(this.GetOweXpath(elementName), this.namespaceManager);
			if (xmlNode != null)
			{
				return this.GetLocaleAwareNodeValue(xmlNode, culture.ToString());
			}
			return null;
		}

		public List<FormSettings> GetFormSettings(FormFactor formFactor, CultureInfo culture, string etoken)
		{
			List<FormSettings> list = new List<FormSettings>();
			string settingsNodeName;
			if (formFactor == FormFactor.Mobile)
			{
				settingsNodeName = "PhoneSettings";
			}
			else if (formFactor == FormFactor.Tablet)
			{
				settingsNodeName = "TabletSettings";
			}
			else
			{
				if (formFactor != FormFactor.Desktop)
				{
					SchemaParser.Tracer.TraceError<FormFactor>(0L, "FormFactor {0} is not supported", formFactor);
					return list;
				}
				settingsNodeName = "DesktopSettings";
			}
			foreach (FormSettings.FormSettingsType formSettingsType in SchemaParser.allFormSettingsTypes)
			{
				XmlNode formSettingsParentNode = this.GetFormSettingsParentNode(formSettingsType);
				if (formSettingsParentNode != null)
				{
					FormSettings formSettingsForFormType = this.GetFormSettingsForFormType(formSettingsParentNode, settingsNodeName, culture.ToString(), etoken);
					if (formSettingsForFormType != null)
					{
						formSettingsForFormType.SettingsType = formSettingsType;
						list.Add(formSettingsForFormType);
					}
				}
			}
			return list;
		}

		public bool TryCreateActivationRule(out ActivationRule activationRule)
		{
			return this.TryCreateActivationRuleInternal(this.GetOweXmlNode("Rule"), out activationRule);
		}

		public virtual void ValidateFormSettings()
		{
		}

		internal bool TryUpdateSourceLocation(IExchangePrincipal exchangePrincipal, string elementName, ExtensionData extensionData, out Exception exception, ExtensionDataHelper.TryModifySourceLocationDelegate tryModifySourceLocationDelegate)
		{
			exception = null;
			foreach (object obj in this.xmlDoc.SelectNodes(this.GetOweXpath(elementName), this.namespaceManager))
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlAttribute xmlAttribute = xmlNode.Attributes["DefaultValue"];
				if (!tryModifySourceLocationDelegate(exchangePrincipal, xmlAttribute, extensionData, out exception))
				{
					return false;
				}
				if (xmlNode.ChildNodes != null)
				{
					foreach (object obj2 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						if (this.IsExpectedOweNamespace(xmlNode2.NamespaceURI) && string.Equals(xmlNode2.LocalName, "Override", StringComparison.Ordinal))
						{
							xmlAttribute = xmlNode2.Attributes["Value"];
							if (!tryModifySourceLocationDelegate(exchangePrincipal, xmlAttribute, extensionData, out exception))
							{
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		protected abstract XmlNode GetFormSettingsParentNode(FormSettings.FormSettingsType formSettingsType);

		protected abstract string GetOweNamespacePrefix();

		protected abstract string GetOweNamespaceUri();

		private FormSettings GetFormSettingsForFormType(XmlNode parentContainer, string settingsNodeName, string clientLanguage, string etoken)
		{
			int requestedHeight = 0;
			string text = null;
			foreach (object obj in parentContainer.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (this.IsExpectedOweNamespace(xmlNode.NamespaceURI) && string.Equals(xmlNode.LocalName, settingsNodeName, StringComparison.Ordinal) && xmlNode.ChildNodes != null)
				{
					foreach (object obj2 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						if (this.IsExpectedOweNamespace(xmlNode2.NamespaceURI))
						{
							if (string.Equals(xmlNode2.LocalName, "SourceLocation", StringComparison.Ordinal))
							{
								text = this.GetLocaleAwareNodeValue(xmlNode2, clientLanguage);
							}
							else if (string.Equals(xmlNode2.LocalName, "RequestedHeight", StringComparison.Ordinal))
							{
								int.TryParse(xmlNode2.InnerText, out requestedHeight);
							}
						}
					}
				}
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				return null;
			}
			if (!string.IsNullOrWhiteSpace(etoken))
			{
				string str = (text.IndexOf('?') > 0) ? "&" : "?";
				text = text + str + "et=" + etoken;
			}
			return new FormSettings
			{
				SourceLocation = text,
				RequestedHeight = requestedHeight
			};
		}

		private string GetOweXpath(string elementName)
		{
			return this.xpathPrefix + elementName;
		}

		private string GetOweChildPath(string childElementName)
		{
			return this.oweNameSpacePrefixWithSemiColon + childElementName;
		}

		private bool TryCreateActivationRuleInternal(XmlNode node, out ActivationRule activationRule)
		{
			activationRule = null;
			if (node == null || node.Attributes == null)
			{
				return false;
			}
			XmlAttribute xmlAttribute = node.Attributes["type", "http://www.w3.org/2001/XMLSchema-instance"];
			string a;
			if (!ExtensionDataHelper.TryGetNameSpaceStrippedAttributeValue(xmlAttribute, out a))
			{
				return false;
			}
			if (!string.Equals(a, "ItemIs", StringComparison.Ordinal))
			{
				if (string.Equals(a, "ItemHasKnownEntity", StringComparison.Ordinal))
				{
					KnownEntityType entityType;
					if (EnumValidator.TryParse<KnownEntityType>(node.Attributes["EntityType"].Value, EnumParseOptions.Default, out entityType))
					{
						XmlAttribute xmlAttribute2 = node.Attributes["FilterName"];
						XmlAttribute xmlAttribute3 = node.Attributes["RegExFilter"];
						bool ignoreCase = SchemaParser.ParseBoolFromXmlAttribute(node.Attributes["IgnoreCase"]);
						activationRule = new ItemHasKnownEntityRule(entityType, (xmlAttribute2 != null) ? xmlAttribute2.Value : null, (xmlAttribute3 != null) ? xmlAttribute3.Value : null, ignoreCase);
						return true;
					}
				}
				else if (string.Equals(a, "ItemHasRegularExpressionMatch", StringComparison.Ordinal))
				{
					RegExPropertyName propertyName;
					if (EnumValidator.TryParse<RegExPropertyName>(node.Attributes["PropertyName"].Value, EnumParseOptions.Default, out propertyName))
					{
						bool ignoreCase2 = SchemaParser.ParseBoolFromXmlAttribute(node.Attributes["IgnoreCase"]);
						activationRule = new ItemHasRegularExpressionMatchRule(node.Attributes["RegExName"].Value, node.Attributes["RegExValue"].Value, propertyName, ignoreCase2);
						return true;
					}
				}
				else
				{
					if (string.Equals(a, "ItemHasAttachment", StringComparison.Ordinal))
					{
						activationRule = new ItemHasAttachmentRule();
						return true;
					}
					if (node.ChildNodes != null && 0 < node.ChildNodes.Count && string.Equals(a, "RuleCollection", StringComparison.Ordinal))
					{
						ActivationRule[] array = new ActivationRule[node.ChildNodes.Count];
						int num = 0;
						foreach (object obj in node.ChildNodes)
						{
							XmlNode xmlNode = (XmlNode)obj;
							ActivationRule activationRule2;
							if (this.IsExpectedOweNamespace(xmlNode.NamespaceURI) && string.Equals(xmlNode.LocalName, "Rule", StringComparison.Ordinal) && this.TryCreateActivationRuleInternal(xmlNode, out activationRule2))
							{
								array[num++] = activationRule2;
							}
						}
						xmlAttribute = node.Attributes["Mode"];
						activationRule = new CollectionRule((xmlAttribute == null) ? "Or" : xmlAttribute.Value, array);
						return true;
					}
				}
				return false;
			}
			ItemIsRuleItemType itemType;
			if (EnumValidator.TryParse<ItemIsRuleItemType>(node.Attributes["ItemType"].Value, EnumParseOptions.Default, out itemType))
			{
				XmlAttribute xmlAttribute4 = node.Attributes["FormType"];
				ItemIsRuleFormType formType;
				if (xmlAttribute4 == null || !EnumValidator.TryParse<ItemIsRuleFormType>(xmlAttribute4.Value, EnumParseOptions.Default, out formType))
				{
					formType = ItemIsRuleFormType.Read;
				}
				bool includeSubClasses = SchemaParser.ParseBoolFromXmlAttribute(node.Attributes["IncludeSubClasses"]);
				XmlAttribute xmlAttribute5 = node.Attributes["ItemClass"];
				activationRule = new ItemIsRule(itemType, (xmlAttribute5 != null) ? xmlAttribute5.Value : null, includeSubClasses, formType);
				return true;
			}
			return false;
		}

		private string GetLocaleAwareNodeValue(XmlNode node, string cultureName)
		{
			if (node.ChildNodes != null)
			{
				foreach (object obj in node.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (this.IsExpectedOweNamespace(xmlNode.NamespaceURI) && string.Equals(xmlNode.LocalName, "Override", StringComparison.Ordinal) && string.Equals(xmlNode.Attributes["Locale"].Value, cultureName, StringComparison.OrdinalIgnoreCase))
					{
						return xmlNode.Attributes["Value"].Value;
					}
				}
			}
			return node.Attributes["DefaultValue"].Value;
		}

		private bool IsExpectedOweNamespace(string namespaceUri)
		{
			return string.Equals(namespaceUri, this.GetOweNamespaceUri(), StringComparison.Ordinal);
		}

		private void ValidateSourceLocationUrls(string xpath, string urlAttributeName, string errorMessageName)
		{
			using (XmlNodeList xmlNodeList = this.xmlDoc.SelectNodes(xpath, this.namespaceManager))
			{
				foreach (object obj in xmlNodeList)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string attributeStringValue = ExtensionData.GetAttributeStringValue(xmlNode, urlAttributeName);
					SchemaParser.ValidateUrl(this.extensionInstallScope, errorMessageName, attributeStringValue);
				}
			}
		}

		internal const int MaxRegexRuleNumber = 5;

		internal const int MaxRuleNumber = 15;

		protected static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		private static FormSettings.FormSettingsType[] allFormSettingsTypes = (FormSettings.FormSettingsType[])Enum.GetValues(typeof(FormSettings.FormSettingsType));

		private static readonly HashSet<string> AllowedEntityTypesInRestricted = new HashSet<string>(StringComparer.Ordinal)
		{
			"Address",
			"Url",
			"PhoneNumber"
		};

		protected SafeXmlDocument xmlDoc;

		protected ExtensionInstallScope extensionInstallScope;

		private readonly string xpathPrefix;

		private readonly string oweNameSpacePrefixWithSemiColon;

		private readonly XmlNamespaceManager namespaceManager;
	}
}
