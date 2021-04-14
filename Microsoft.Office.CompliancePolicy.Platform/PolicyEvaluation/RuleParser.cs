using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class RuleParser
	{
		public RuleParser(IPolicyParserFactory policyParserFactory)
		{
			ArgumentValidator.ThrowIfNull("policyParserFactory", policyParserFactory);
			IEnumerable<string> actionsSupportedByFactory = policyParserFactory.GetSupportedActions();
			using (IEnumerator<string> enumerator = (from tagName in RuleParser.MandatoryActions
			where !actionsSupportedByFactory.Contains(tagName)
			select tagName).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					string text = enumerator.Current;
					throw new CompliancePolicyValidationException("Supplied parser factory does not support '{0}' action. All workloads must support common actions.", new object[]
					{
						text
					});
				}
			}
			this.policyParserFactory = policyParserFactory;
		}

		protected RuleParser()
		{
		}

		public static string GetDisabledRuleXml(string inputRule)
		{
			string outerXml;
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				StringReader input = new StringReader(inputRule);
				XmlTextReader reader = new XmlTextReader(input)
				{
					DtdProcessing = DtdProcessing.Prohibit,
					XmlResolver = null
				};
				XmlReaderSettings settings = new XmlReaderSettings
				{
					ConformanceLevel = ConformanceLevel.Auto,
					IgnoreComments = true,
					DtdProcessing = DtdProcessing.Prohibit,
					XmlResolver = null
				};
				XmlReader reader2 = XmlReader.Create(reader, settings);
				xmlDocument.Load(reader2);
				XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("enabled");
				xmlAttribute.Value = RuleConstants.StringFromRuleState(RuleState.Disabled);
				XmlAttributeCollection attributes = xmlDocument.DocumentElement.Attributes;
				XmlAttribute refNode = (XmlAttribute)attributes.GetNamedItem("name");
				attributes.Remove(attributes["enabled"]);
				attributes.InsertAfter(xmlAttribute, refNode);
				outerXml = xmlDocument.OuterXml;
			}
			catch (XmlException innerException)
			{
				throw new CompliancePolicyParserException("Compliance policy parsing error", innerException);
			}
			return outerXml;
		}

		public PolicyRule GetRule(string ruleString)
		{
			XmlTextReader xmlTextReader = null;
			XmlReader xmlReader = null;
			PolicyRule result = null;
			StringReader stringReader = new StringReader(ruleString);
			try
			{
				xmlTextReader = new XmlTextReader(stringReader);
				xmlTextReader.DtdProcessing = DtdProcessing.Prohibit;
				xmlTextReader.XmlResolver = null;
				XmlReaderSettings settings = new XmlReaderSettings
				{
					ConformanceLevel = ConformanceLevel.Auto,
					IgnoreComments = true,
					DtdProcessing = DtdProcessing.Prohibit,
					XmlResolver = null
				};
				xmlReader = XmlReader.Create(xmlTextReader, settings);
				this.ReadNext(xmlReader);
				this.VerifyTag(xmlReader, "rule");
				result = this.ParseRule(xmlReader);
			}
			catch (XmlException innerException)
			{
				throw new CompliancePolicyParserException("Error parsing rule", innerException);
			}
			catch (CompliancePolicyException innerException2)
			{
				throw new CompliancePolicyParserException("Error creating rule from XML", innerException2);
			}
			finally
			{
				if (xmlReader != null)
				{
					xmlReader.Close();
				}
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
				if (stringReader != null)
				{
					stringReader.Close();
				}
			}
			return result;
		}

		public IList<PolicyRule> LoadStream(Stream stream)
		{
			XmlTextReader xmlTextReader = null;
			XmlReader xmlReader = null;
			IList<PolicyRule> result = null;
			try
			{
				xmlTextReader = new XmlTextReader(stream)
				{
					DtdProcessing = DtdProcessing.Prohibit,
					XmlResolver = null
				};
				XmlReaderSettings settings = new XmlReaderSettings
				{
					ConformanceLevel = ConformanceLevel.Auto,
					IgnoreComments = true,
					DtdProcessing = DtdProcessing.Prohibit,
					XmlResolver = null
				};
				xmlReader = XmlReader.Create(xmlTextReader, settings);
				this.ReadNext(xmlReader);
				if (xmlReader.NodeType == XmlNodeType.XmlDeclaration)
				{
					this.ReadNext(xmlReader);
				}
				this.VerifyTag(xmlReader, "rules");
				result = this.ParseRules(xmlReader);
			}
			catch (XmlException innerException)
			{
				throw new CompliancePolicyParserException("Error parsing rule", innerException);
			}
			catch (CompliancePolicyValidationException innerException2)
			{
				throw new CompliancePolicyParserException("Error creating rule from XML", innerException2);
			}
			finally
			{
				if (xmlReader != null)
				{
					xmlReader.Close();
				}
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
			return result;
		}

		public Action CreateAction(string actionName, string externalName = null)
		{
			List<Argument> arguments = new List<Argument>();
			return this.CreateAction(actionName, arguments, externalName);
		}

		public virtual Action CreateAction(string actionName, List<Argument> arguments, string externalName = null)
		{
			Action action = this.policyParserFactory.CreateAction(actionName, arguments, externalName);
			if (action != null)
			{
				return action;
			}
			throw new CompliancePolicyParserException(string.Format("Workload factory failed to create action '{0}'.", actionName));
		}

		public virtual Property CreateProperty(string propertyName, string typeName = null)
		{
			Property property = this.policyParserFactory.CreateProperty(propertyName, typeName) ?? Property.CreateProperty(propertyName, typeName);
			if (property == null)
			{
				throw new CompliancePolicyParserException(string.Format("No suitable factory for the property '{0}' found. ", propertyName));
			}
			return property;
		}

		public virtual PolicyRule CreateRule(string ruleName)
		{
			return new PolicyRule
			{
				Name = ruleName
			};
		}

		public virtual IList<PolicyRule> CreateRuleCollection()
		{
			return new List<PolicyRule>();
		}

		public virtual PredicateCondition CreatePredicate(string name, Property property, List<List<KeyValuePair<string, string>>> valueEntries)
		{
			if (name.Equals("containsDataClassification", StringComparison.InvariantCultureIgnoreCase))
			{
				return new ContentContainsSensitiveInformationPredicate(valueEntries);
			}
			throw new CompliancePolicyValidationException("Invalid condition " + name);
		}

		public virtual PredicateCondition CreatePredicate(string name, Property property, List<string> valueEntries)
		{
			PredicateCondition predicateCondition = this.policyParserFactory.CreatePredicate(name, property, valueEntries);
			if (predicateCondition != null)
			{
				return predicateCondition;
			}
			switch (name)
			{
			case "equal":
				return new EqualPredicate(property, valueEntries);
			case "exists":
				return new ExistsPredicate(property, valueEntries);
			case "greaterThan":
				return new GreaterThanPredicate(property, valueEntries);
			case "greaterThanOrEqual":
				return new GreaterThanOrEqualPredicate(property, valueEntries);
			case "is":
				return new IsPredicate(property, valueEntries);
			case "lessThan":
				return new LessThanPredicate(property, valueEntries);
			case "lessThanOrEqual":
				return new LessThanOrEqualPredicate(property, valueEntries);
			case "notEqual":
				return new NotEqualPredicate(property, valueEntries);
			case "notExists":
				return new NotExistsPredicate(property, valueEntries);
			case "textQueryMatch":
				return new TextQueryPredicate(property, valueEntries);
			case "auditOperations":
				return new AuditOperationsPredicate(property, valueEntries);
			case "NameValuesPairConfiguration":
				return new NameValuesPairConfigurationPredicate(property, valueEntries);
			case "contentMetadataContains":
				return new ContentMetadataContainsPredicate(valueEntries);
			}
			throw new CompliancePolicyValidationException("Condition name is not recognized - " + name);
		}

		internal Argument ParseArgument(XmlReader reader)
		{
			string attribute = reader.GetAttribute("value");
			string attribute2 = reader.GetAttribute("property");
			if (attribute != null && attribute2 != null)
			{
				throw new CompliancePolicyParserException("Argument is incorrect");
			}
			Argument argument = null;
			if (attribute != null)
			{
				argument = new Value(attribute);
			}
			else if (attribute2 != null)
			{
				argument = this.CreateProperty(attribute2, reader.GetAttribute("type"));
			}
			if (!reader.IsEmptyElement)
			{
				this.ReadNext(reader);
				if (this.IsTag(reader, "value") && reader.NodeType == XmlNodeType.Element)
				{
					if (argument != null)
					{
						throw new CompliancePolicyParserException("Argument is incorrect. The argument tag should not have any children tags if it has the value attribute.");
					}
					List<string> list = new List<string>();
					while (this.IsTag(reader, "value") && reader.NodeType == XmlNodeType.Element)
					{
						list.Add(this.ParseSimpleValue(reader));
						this.ReadNext(reader);
					}
					if (!list.Any<string>())
					{
						throw new CompliancePolicyParserException("Argument is incorrect. There must be children tags under the argument tag if the argument tag does not have the value attribute.");
					}
					argument = new Value(list);
				}
				this.VerifyEndTag(reader, "argument");
			}
			return argument;
		}

		protected virtual void CreateRuleSubElements(PolicyRule rule, XmlReader reader)
		{
		}

		protected virtual PolicyRule ParseRuleAttributes(XmlReader reader)
		{
			this.VerifyTag(reader, "rule");
			string requiredAttribute = this.GetRequiredAttribute(reader, "name");
			string attribute = reader.GetAttribute("comments");
			string attribute2 = reader.GetAttribute("enabled");
			string attribute3 = reader.GetAttribute("id");
			Guid empty = Guid.Empty;
			if (attribute3 != null && !Guid.TryParse(attribute3, out empty))
			{
				throw new CompliancePolicyParserException(string.Format("Invalid Attribute {0} {1}", "rule", attribute3), new object[]
				{
					reader
				});
			}
			RuleState enabled;
			if (string.IsNullOrEmpty(attribute2))
			{
				enabled = RuleState.Enabled;
			}
			else if (!RuleConstants.TryParseEnabled(attribute2, out enabled))
			{
				throw new CompliancePolicyParserException(string.Format("Invalid Attribute {0} {1} {2}", "enabled", "rule", attribute2), new object[]
				{
					reader
				});
			}
			string attribute4 = reader.GetAttribute("expiryDate");
			DateTime? expiryDate;
			if (!PolicyUtils.TryParseNullableDateTimeUtc(attribute4, out expiryDate))
			{
				throw new CompliancePolicyParserException(string.Format("Invalid Attribute {0} {1} {2}", "expiryDate", "rule", attribute4), new object[]
				{
					reader
				});
			}
			string attribute5 = reader.GetAttribute("activationDate");
			DateTime? activationDate;
			if (!PolicyUtils.TryParseNullableDateTimeUtc(attribute5, out activationDate))
			{
				throw new CompliancePolicyParserException(string.Format("Invalid Attribute {0} {1} {2}", "activationDate", "rule", attribute5), new object[]
				{
					reader
				});
			}
			string attribute6 = reader.GetAttribute("mode");
			RuleMode mode = RuleConstants.TryParseEnum<RuleMode>(attribute6, RuleMode.Enforce);
			PolicyRule policyRule = this.CreateRule(requiredAttribute);
			policyRule.IsTooAdvancedToParse = false;
			policyRule.Comments = attribute;
			policyRule.Enabled = enabled;
			policyRule.ExpiryDate = expiryDate;
			policyRule.ActivationDate = activationDate;
			policyRule.Mode = mode;
			policyRule.ImmutableId = empty;
			return policyRule;
		}

		protected void ReadNext(XmlReader reader)
		{
			this.ReadNext(reader, true);
		}

		protected void Skip(XmlReader reader)
		{
			reader.Skip();
			while (reader.NodeType == XmlNodeType.Whitespace)
			{
				if (!reader.Read())
				{
					throw new CompliancePolicyParserException("End of stream reached", new object[]
					{
						reader
					});
				}
			}
		}

		protected void ReadNext(XmlReader reader, bool ignoreWhiteSpace)
		{
			if (!reader.Read())
			{
				throw new CompliancePolicyParserException("End of stream reached", new object[]
				{
					reader
				});
			}
			while (ignoreWhiteSpace && reader.NodeType == XmlNodeType.Whitespace)
			{
				if (!reader.Read())
				{
					throw new CompliancePolicyParserException("End of stream reached", new object[]
					{
						reader
					});
				}
			}
		}

		protected void VerifyEndTag(XmlReader reader, string tagName)
		{
			if (reader.NodeType != XmlNodeType.EndElement || !reader.Name.Equals(tagName))
			{
				throw new CompliancePolicyParserException(string.Format("End tag not found '{0}'", tagName));
			}
		}

		protected void VerifyTag(XmlReader reader, string tagName)
		{
			if (reader.NodeType != XmlNodeType.Element || !reader.Name.Equals(tagName))
			{
				throw new CompliancePolicyParserException(string.Format("Tag not found '{0}'", tagName));
			}
		}

		protected bool IsTag(XmlReader reader, string tagName)
		{
			return reader.NodeType == XmlNodeType.Element && reader.Name.Equals(tagName);
		}

		protected void VerifyNotEmptyTag(XmlReader reader)
		{
			if (reader.IsEmptyElement)
			{
				throw new CompliancePolicyParserException(string.Format("Tag '{0}' is empty", reader.Name));
			}
		}

		protected string GetRequiredAttribute(XmlReader reader, string attributeName)
		{
			string attribute = reader.GetAttribute(attributeName);
			if (attribute == null)
			{
				throw new CompliancePolicyParserException(string.Format("Attribute not found '{0}' '{1}'", attributeName, reader.Name));
			}
			return attribute;
		}

		protected void VerifyAttributeValue(XmlReader reader, string attributeName, string attributeValue)
		{
			if (string.IsNullOrWhiteSpace(attributeValue))
			{
				throw new CompliancePolicyParserException(string.Format("Attribute not found '{0}' '{1}'", attributeName, reader.Name));
			}
		}

		private Property ParseProperty(XmlReader reader)
		{
			string requiredAttribute = this.GetRequiredAttribute(reader, "property");
			string attribute = reader.GetAttribute("type");
			string attribute2 = reader.GetAttribute("suppl");
			if (requiredAttribute.Equals(string.Empty))
			{
				return null;
			}
			Property property = this.CreateProperty(requiredAttribute, attribute);
			property.SupplementalInfo = attribute2;
			return property;
		}

		private IList<PolicyRule> ParseRules(XmlReader reader)
		{
			this.GetRequiredAttribute(reader, "name");
			IList<PolicyRule> list = this.CreateRuleCollection();
			if (reader.IsEmptyElement)
			{
				return list;
			}
			this.ReadNext(reader);
			while (this.IsTag(reader, "rule"))
			{
				PolicyRule rule = this.ParseRule(reader);
				if (list.Any((PolicyRule r) => string.Compare(r.Name, rule.Name, StringComparison.InvariantCultureIgnoreCase) == 0))
				{
					throw new CompliancePolicyParserException(string.Format("Rule '{0}' already exists. No duplicate names allowed", rule.Name));
				}
				list.Add(rule);
				this.ReadNext(reader);
			}
			this.VerifyEndTag(reader, "rules");
			return list;
		}

		private PolicyRule ParseRule(XmlReader reader)
		{
			this.VerifyNotEmptyTag(reader);
			PolicyRule policyRule = this.ParseRuleAttributes(reader);
			this.ReadNext(reader);
			bool flag = this.IsTag(reader, "version");
			if (flag)
			{
				this.VerifyTag(reader, "version");
				string requiredAttribute = this.GetRequiredAttribute(reader, "requiredMinVersion");
				Version v = null;
				try
				{
					v = new Version(requiredAttribute);
				}
				catch (ArgumentException innerException)
				{
					throw new CompliancePolicyParserException("Error parsing rule", innerException);
				}
				catch (FormatException innerException2)
				{
					throw new CompliancePolicyParserException("Error parsing rule", innerException2);
				}
				policyRule.IsTooAdvancedToParse = (v > PolicyRule.HighestHonoredVersion);
				if (policyRule.IsTooAdvancedToParse)
				{
					this.Skip(reader);
				}
				else
				{
					this.ReadNext(reader);
				}
			}
			if (!policyRule.IsTooAdvancedToParse)
			{
				this.CreateRuleSubElements(policyRule, reader);
				if (this.IsTag(reader, "tags"))
				{
					this.ParseRuleTags(reader, policyRule);
				}
				this.VerifyTag(reader, "condition");
				policyRule.Condition = this.ParseCondition(reader);
				this.ReadNext(reader);
				while (this.IsTag(reader, "action"))
				{
					Action item = this.ParseAction(reader);
					policyRule.Actions.Add(item);
					this.ReadNext(reader);
				}
				if (flag)
				{
					this.VerifyEndTag(reader, "version");
					this.ReadNext(reader);
				}
			}
			if (flag)
			{
				while (this.IsTag(reader, "version"))
				{
					this.Skip(reader);
				}
			}
			this.VerifyEndTag(reader, "rule");
			return policyRule;
		}

		private void ParseRuleTags(XmlReader reader, PolicyRule rule)
		{
			this.ReadNext(reader);
			while (this.IsTag(reader, "tag"))
			{
				rule.AddTag(this.ParseRuleTag(reader));
				this.ReadNext(reader);
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (!(reader.Name == "tag"))
					{
						break;
					}
					this.ReadNext(reader);
				}
			}
			this.VerifyEndTag(reader, "tags");
			this.ReadNext(reader);
		}

		private RuleTag ParseRuleTag(XmlReader reader)
		{
			string text = null;
			string text2 = null;
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			while (reader.MoveToNextAttribute())
			{
				string name;
				if ((name = reader.Name) != null)
				{
					if (name == "name")
					{
						text = reader.Value;
						continue;
					}
					if (name == "type")
					{
						text2 = reader.Value;
						continue;
					}
				}
				list.Add(new KeyValuePair<string, string>(reader.Name, reader.Value));
			}
			this.VerifyAttributeValue(reader, "name", text);
			this.VerifyAttributeValue(reader, "type", text2);
			RuleTag result = new RuleTag(text, text2);
			list.ForEach(delegate(KeyValuePair<string, string> item)
			{
				result.Data.Add(item.Key, item.Value);
			});
			return result;
		}

		private Action ParseAction(XmlReader reader)
		{
			string requiredAttribute = this.GetRequiredAttribute(reader, "name");
			string attribute = reader.GetAttribute("externalName");
			List<Argument> list = new List<Argument>();
			if (!reader.IsEmptyElement)
			{
				this.ReadNext(reader);
				while (this.IsTag(reader, "argument"))
				{
					list.Add(this.ParseArgument(reader));
					this.ReadNext(reader);
				}
				this.VerifyEndTag(reader, "action");
			}
			return this.CreateAction(requiredAttribute, list, attribute);
		}

		private Condition ParseCondition(XmlReader reader)
		{
			this.VerifyNotEmptyTag(reader);
			this.ReadNext(reader);
			Condition result = this.ParseSubCondition(reader);
			this.ReadNext(reader);
			this.VerifyEndTag(reader, "condition");
			return result;
		}

		private Condition ParseSubCondition(XmlReader reader)
		{
			if (reader.NodeType != XmlNodeType.Element)
			{
				throw new CompliancePolicyParserException("No COnditions tag found");
			}
			string name = reader.Name;
			string key;
			switch (key = name)
			{
			case "true":
				if (!reader.IsEmptyElement)
				{
					this.ReadNext(reader);
					this.VerifyEndTag(reader, name);
				}
				return Condition.True;
			case "false":
				if (!reader.IsEmptyElement)
				{
					this.ReadNext(reader);
					this.VerifyEndTag(reader, name);
				}
				return Condition.False;
			case "not":
			{
				this.VerifyNotEmptyTag(reader);
				this.ReadNext(reader);
				NotCondition result = new NotCondition(this.ParseSubCondition(reader));
				this.ReadNext(reader);
				this.VerifyEndTag(reader, name);
				return result;
			}
			case "and":
			{
				this.VerifyNotEmptyTag(reader);
				AndCondition andCondition = new AndCondition();
				this.ReadNext(reader);
				do
				{
					andCondition.SubConditions.Add(this.ParseSubCondition(reader));
					this.ReadNext(reader);
				}
				while (reader.NodeType == XmlNodeType.Element);
				this.VerifyEndTag(reader, name);
				return andCondition;
			}
			case "or":
			{
				this.VerifyNotEmptyTag(reader);
				OrCondition orCondition = new OrCondition();
				this.ReadNext(reader);
				do
				{
					orCondition.SubConditions.Add(this.ParseSubCondition(reader));
					this.ReadNext(reader);
				}
				while (reader.NodeType == XmlNodeType.Element);
				this.VerifyEndTag(reader, name);
				return orCondition;
			}
			case "queryMatch":
			{
				this.VerifyNotEmptyTag(reader);
				this.ReadNext(reader);
				QueryPredicate result2 = new QueryPredicate(this.ParseSubCondition(reader));
				this.ReadNext(reader);
				this.VerifyEndTag(reader, name);
				return result2;
			}
			}
			return this.CreateSubCondition(name, reader);
		}

		private Condition CreateSubCondition(string conditionName, XmlReader reader)
		{
			Property property = this.ParseProperty(reader);
			List<string> list = new List<string>();
			List<List<KeyValuePair<string, string>>> list2 = new List<List<KeyValuePair<string, string>>>();
			bool flag = false;
			if (!reader.IsEmptyElement)
			{
				this.ReadNext(reader);
				while (this.IsTag(reader, "keyValues") && reader.NodeType == XmlNodeType.Element)
				{
					flag = true;
					this.ReadNext(reader);
					List<KeyValuePair<string, string>> list3 = new List<KeyValuePair<string, string>>();
					while (this.IsTag(reader, "keyValue") && reader.NodeType == XmlNodeType.Element)
					{
						list3.Add(new KeyValuePair<string, string>(reader.GetAttribute("key"), reader.GetAttribute("value")));
						this.ReadNext(reader);
					}
					if (list3.Count == 0)
					{
						throw new CompliancePolicyParserException("Inconsistent value types in condition properties");
					}
					list2.Add(list3);
					this.VerifyEndTag(reader, "keyValues");
					this.ReadNext(reader);
				}
				if (!flag)
				{
					while (this.IsTag(reader, "value") && reader.NodeType == XmlNodeType.Element)
					{
						list.Add(this.ParseSimpleValue(reader));
						flag = false;
						this.ReadNext(reader);
					}
				}
				this.VerifyEndTag(reader, conditionName);
			}
			if (flag)
			{
				return this.CreatePredicate(conditionName, property, list2);
			}
			return this.CreatePredicate(conditionName, property, list);
		}

		private string ParseSimpleValue(XmlReader reader)
		{
			this.ReadNext(reader, false);
			if (reader.NodeType != XmlNodeType.Text && reader.NodeType != XmlNodeType.Whitespace)
			{
				throw new CompliancePolicyParserException("Value tag not found");
			}
			string value = reader.Value;
			this.ReadNext(reader);
			this.VerifyEndTag(reader, "value");
			return value;
		}

		private static readonly string[] MandatoryActions = new string[]
		{
			"Hold",
			"RetentionExpire",
			"RetentionRecycle",
			"BlockAccess",
			"GenerateIncidentReport",
			"NotifyAuthors"
		};

		private readonly IPolicyParserFactory policyParserFactory;
	}
}
