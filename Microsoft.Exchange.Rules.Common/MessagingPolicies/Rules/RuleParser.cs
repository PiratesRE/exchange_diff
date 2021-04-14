using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public abstract class RuleParser
	{
		public static string GetDisabledRuleXml(string inputRule)
		{
			string outerXml;
			try
			{
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.LoadXml(inputRule);
				XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("enabled");
				xmlAttribute.Value = RuleConstants.StringFromRuleState(RuleState.Disabled);
				XmlAttributeCollection attributes = xmlDocument.DocumentElement.Attributes;
				XmlAttribute refNode = (XmlAttribute)attributes.GetNamedItem("name");
				attributes.Remove(attributes["enabled"]);
				attributes.InsertAfter(xmlAttribute, refNode);
				outerXml = xmlDocument.OuterXml;
			}
			catch (XmlException e)
			{
				throw new ParserException(e);
			}
			return outerXml;
		}

		public Rule GetRule(string ruleString)
		{
			return this.GetRule(ruleString, new RulesCreationContext());
		}

		public Rule GetRule(string ruleString, RulesCreationContext creationContext)
		{
			XmlTextReader xmlTextReader = null;
			XmlReader xmlReader = null;
			Rule result = null;
			StringReader stringReader = new StringReader(ruleString);
			try
			{
				xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(stringReader);
				xmlReader = XmlReader.Create(xmlTextReader, new XmlReaderSettings
				{
					ConformanceLevel = ConformanceLevel.Auto,
					IgnoreComments = true,
					DtdProcessing = DtdProcessing.Prohibit,
					XmlResolver = null
				});
				this.ReadNext(xmlReader);
				this.VerifyTag(xmlReader, "rule");
				result = this.ParseRule(xmlReader, creationContext);
			}
			catch (XmlException e)
			{
				throw new ParserException(e);
			}
			catch (RulesValidationException e2)
			{
				throw new ParserException(e2, xmlReader);
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

		public RuleCollection LoadStream(Stream stream)
		{
			return this.LoadStream(stream, new RulesCreationContext());
		}

		public RuleCollection LoadStream(Stream stream, RulesCreationContext creationContext)
		{
			XmlTextReader xmlTextReader = null;
			XmlReader xmlReader = null;
			RuleCollection result = null;
			try
			{
				xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(stream);
				xmlReader = XmlReader.Create(xmlTextReader, new XmlReaderSettings
				{
					ConformanceLevel = ConformanceLevel.Auto,
					IgnoreComments = true,
					DtdProcessing = DtdProcessing.Prohibit,
					XmlResolver = null
				});
				this.ReadNext(xmlReader);
				if (xmlReader.NodeType == XmlNodeType.XmlDeclaration)
				{
					this.ReadNext(xmlReader);
				}
				this.VerifyTag(xmlReader, "rules");
				result = this.ParseRules(xmlReader, creationContext);
			}
			catch (XmlException e)
			{
				throw new ParserException(e);
			}
			catch (RulesValidationException e2)
			{
				throw new ParserException(e2, xmlReader);
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
			ShortList<Argument> arguments = new ShortList<Argument>();
			return this.CreateAction(actionName, arguments, externalName);
		}

		public abstract Action CreateAction(string actionName, ShortList<Argument> arguments, string externalName = null);

		public abstract Property CreateProperty(string propertyName, string typeName);

		public abstract Property CreateProperty(string propertyName);

		public virtual Rule CreateRule(string ruleName)
		{
			return new Rule(ruleName);
		}

		public virtual RuleCollection CreateRuleCollection(string ruleName)
		{
			return new RuleCollection(ruleName);
		}

		protected virtual void CreateRuleSubElements(Rule rule, XmlReader reader, RulesCreationContext creationContext)
		{
		}

		public virtual PredicateCondition CreatePredicate(string name, Property property, ShortList<ShortList<KeyValuePair<string, string>>> valueEntries)
		{
			return this.CreatePredicate(name, property, valueEntries, new RulesCreationContext());
		}

		public virtual PredicateCondition CreatePredicate(string name, Property property, ShortList<ShortList<KeyValuePair<string, string>>> valueEntries, RulesCreationContext creationContext)
		{
			throw new RulesValidationException(RulesStrings.InvalidCondition(name), null);
		}

		public virtual PredicateCondition CreatePredicate(string name, Property property, ShortList<string> valueEntries)
		{
			return this.CreatePredicate(name, property, valueEntries, new RulesCreationContext());
		}

		public virtual PredicateCondition CreatePredicate(string name, Property property, ShortList<string> valueEntries, RulesCreationContext creationContext)
		{
			switch (name)
			{
			case "greaterThan":
				return new GreaterThanPredicate(property, valueEntries, creationContext);
			case "lessThan":
				return new LessThanPredicate(property, valueEntries, creationContext);
			case "greaterThanOrEqual":
				return new GreaterThanOrEqualPredicate(property, valueEntries, creationContext);
			case "lessThanOrEqual":
				return new LessThanOrEqualPredicate(property, valueEntries, creationContext);
			case "equal":
				return new EqualPredicate(property, valueEntries, creationContext);
			case "notEqual":
				return new NotEqualPredicate(property, valueEntries, creationContext);
			case "is":
				return new IsPredicate(property, valueEntries, creationContext);
			case "contains":
				return new ContainsPredicate(property, valueEntries, creationContext);
			case "matches":
				return new LegacyMatchesPredicate(property, valueEntries, creationContext);
			case "matchesRegex":
				return new MatchesRegexPredicate(property, valueEntries, creationContext);
			case "exists":
				return new ExistsPredicate(property, valueEntries, creationContext);
			case "notExists":
				return new NotExistsPredicate(property, valueEntries, creationContext);
			}
			throw new RulesValidationException(RulesStrings.InvalidCondition(name), null);
		}

		protected virtual Rule ParseRuleAttributes(XmlReader reader)
		{
			this.VerifyTag(reader, "rule");
			string requiredAttribute = this.GetRequiredAttribute(reader, "name");
			string attribute = reader.GetAttribute("comments");
			string attribute2 = reader.GetAttribute("enabled");
			string attribute3 = reader.GetAttribute("id");
			Guid empty = Guid.Empty;
			if (attribute3 != null && !Guid.TryParse(attribute3, out empty))
			{
				throw new ParserException(RulesStrings.InvalidAttribute("id", "rule", attribute3), reader);
			}
			RuleState enabled;
			if (string.IsNullOrEmpty(attribute2))
			{
				enabled = RuleState.Enabled;
			}
			else if (!RuleConstants.TryParseEnabled(attribute2, out enabled))
			{
				throw new ParserException(RulesStrings.InvalidAttribute("enabled", "rule", attribute2), reader);
			}
			string attribute4 = reader.GetAttribute("expiryDate");
			DateTime? expiryDate;
			if (!RuleUtils.TryParseNullableDateTimeUtc(attribute4, out expiryDate))
			{
				throw new ParserException(RulesStrings.InvalidAttribute("expiryDate", "rule", attribute4), reader);
			}
			string attribute5 = reader.GetAttribute("activationDate");
			DateTime? activationDate;
			if (!RuleUtils.TryParseNullableDateTimeUtc(attribute5, out activationDate))
			{
				throw new ParserException(RulesStrings.InvalidAttribute("activationDate", "rule", attribute5), reader);
			}
			string attribute6 = reader.GetAttribute("mode");
			RuleMode mode = RuleConstants.TryParseEnum<RuleMode>(attribute6, RuleMode.Enforce);
			string attribute7 = reader.GetAttribute("subType");
			RuleSubType subType = RuleConstants.TryParseEnum<RuleSubType>(attribute7, RuleSubType.None);
			string attribute8 = reader.GetAttribute("errorAction");
			RuleErrorAction errorAction = RuleConstants.TryParseEnum<RuleErrorAction>(attribute8, RuleErrorAction.Ignore);
			Rule rule = this.CreateRule(requiredAttribute);
			rule.SupportGetEstimatedSize = true;
			rule.IsTooAdvancedToParse = false;
			rule.Comments = attribute;
			rule.Enabled = enabled;
			rule.ExpiryDate = expiryDate;
			rule.ActivationDate = activationDate;
			rule.Mode = mode;
			rule.SubType = subType;
			rule.ImmutableId = empty;
			rule.ErrorAction = errorAction;
			return rule;
		}

		private Property ParseProperty(XmlReader reader)
		{
			string requiredAttribute = this.GetRequiredAttribute(reader, "property");
			string attribute = reader.GetAttribute("type");
			if (requiredAttribute.Equals(string.Empty))
			{
				return null;
			}
			if (string.IsNullOrEmpty(attribute))
			{
				return this.CreateProperty(requiredAttribute);
			}
			return this.CreateProperty(requiredAttribute, attribute);
		}

		private RuleCollection ParseRules(XmlReader reader, RulesCreationContext creationContext)
		{
			string requiredAttribute = this.GetRequiredAttribute(reader, "name");
			RuleCollection ruleCollection = this.CreateRuleCollection(requiredAttribute);
			if (reader.IsEmptyElement)
			{
				return ruleCollection;
			}
			this.ReadNext(reader);
			while (this.IsTag(reader, "rule"))
			{
				Rule rule = this.ParseRule(reader, creationContext);
				if (ruleCollection[rule.Name] != null)
				{
					throw new ParserException(RulesStrings.RuleNameExists(rule.Name), reader);
				}
				ruleCollection.Add(rule);
				this.ReadNext(reader);
			}
			this.VerifyEndTag(reader, "rules");
			return ruleCollection;
		}

		private Rule ParseRule(XmlReader reader, RulesCreationContext creationContext)
		{
			this.VerifyNotEmptyTag(reader);
			Rule rule = this.ParseRuleAttributes(reader);
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
				catch (ArgumentException e)
				{
					throw new ParserException(e, reader);
				}
				catch (FormatException e2)
				{
					throw new ParserException(e2, reader);
				}
				rule.IsTooAdvancedToParse = (v > Rule.HighestHonoredVersion);
				if (rule.IsTooAdvancedToParse)
				{
					this.Skip(reader);
				}
				else
				{
					this.ReadNext(reader);
				}
			}
			if (!rule.IsTooAdvancedToParse)
			{
				this.CreateRuleSubElements(rule, reader, creationContext);
				if (this.IsTag(reader, "tags"))
				{
					this.ParseRuleTags(reader, rule);
				}
				this.VerifyTag(reader, "condition");
				rule.Condition = this.ParseCondition(reader, creationContext);
				this.ReadNext(reader);
				while (this.IsTag(reader, "action"))
				{
					Action action = this.ParseAction(reader);
					creationContext.ConditionAndActionSize += action.GetEstimatedSize();
					rule.Actions.Add(action);
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
			rule.ConditionAndActionSize = creationContext.ConditionAndActionSize;
			creationContext.ConditionAndActionSize = 0;
			return rule;
		}

		private void ParseRuleTags(XmlReader reader, Rule rule)
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
			ShortList<Argument> shortList = new ShortList<Argument>();
			if (!reader.IsEmptyElement)
			{
				this.ReadNext(reader);
				while (this.IsTag(reader, "argument"))
				{
					shortList.Add(this.ParseArgument(reader));
					this.ReadNext(reader);
				}
				this.VerifyEndTag(reader, "action");
			}
			return this.CreateAction(requiredAttribute, shortList, attribute);
		}

		private Condition ParseCondition(XmlReader reader, RulesCreationContext creationContext)
		{
			this.VerifyNotEmptyTag(reader);
			this.ReadNext(reader);
			Condition result = this.ParseSubCondition(reader, creationContext);
			this.ReadNext(reader);
			this.VerifyEndTag(reader, "condition");
			return result;
		}

		private Condition ParseSubCondition(XmlReader reader, RulesCreationContext creationContext)
		{
			if (reader.NodeType != XmlNodeType.Element)
			{
				throw new ParserException(RulesStrings.ConditionTagNotFound, reader);
			}
			string name = reader.Name;
			string a;
			if ((a = name) != null)
			{
				if (a == "true")
				{
					if (!reader.IsEmptyElement)
					{
						this.ReadNext(reader);
						this.VerifyEndTag(reader, name);
					}
					creationContext.ConditionAndActionSize += 18;
					return Condition.True;
				}
				if (a == "false")
				{
					if (!reader.IsEmptyElement)
					{
						this.ReadNext(reader);
						this.VerifyEndTag(reader, name);
					}
					creationContext.ConditionAndActionSize += 18;
					return Condition.False;
				}
				if (a == "not")
				{
					this.VerifyNotEmptyTag(reader);
					this.ReadNext(reader);
					NotCondition result = new NotCondition(this.ParseSubCondition(reader, creationContext));
					this.ReadNext(reader);
					this.VerifyEndTag(reader, name);
					creationContext.ConditionAndActionSize += 18;
					return result;
				}
				if (a == "and")
				{
					this.VerifyNotEmptyTag(reader);
					AndCondition andCondition = new AndCondition();
					this.ReadNext(reader);
					do
					{
						andCondition.SubConditions.Add(this.ParseSubCondition(reader, creationContext));
						this.ReadNext(reader);
					}
					while (reader.NodeType == XmlNodeType.Element);
					this.VerifyEndTag(reader, name);
					creationContext.ConditionAndActionSize += 18;
					return andCondition;
				}
				if (a == "or")
				{
					this.VerifyNotEmptyTag(reader);
					OrCondition orCondition = new OrCondition();
					this.ReadNext(reader);
					do
					{
						orCondition.SubConditions.Add(this.ParseSubCondition(reader, creationContext));
						this.ReadNext(reader);
					}
					while (reader.NodeType == XmlNodeType.Element);
					this.VerifyEndTag(reader, name);
					creationContext.ConditionAndActionSize += 18;
					return orCondition;
				}
			}
			return this.CreateSubCondition(name, reader, creationContext);
		}

		private Condition CreateSubCondition(string conditionName, XmlReader reader, RulesCreationContext creationContext)
		{
			Property property = this.ParseProperty(reader);
			ShortList<string> shortList = new ShortList<string>();
			ShortList<ShortList<KeyValuePair<string, string>>> shortList2 = new ShortList<ShortList<KeyValuePair<string, string>>>();
			bool flag = false;
			if (!reader.IsEmptyElement)
			{
				this.ReadNext(reader);
				while (this.IsTag(reader, "keyValues") && reader.NodeType == XmlNodeType.Element)
				{
					flag = true;
					this.ReadNext(reader);
					ShortList<KeyValuePair<string, string>> shortList3 = new ShortList<KeyValuePair<string, string>>();
					while (this.IsTag(reader, "keyValue") && reader.NodeType == XmlNodeType.Element)
					{
						shortList3.Add(new KeyValuePair<string, string>(reader.GetAttribute("key"), reader.GetAttribute("value")));
						this.ReadNext(reader);
					}
					if (shortList3.Count == 0)
					{
						throw new ParserException(RulesStrings.InconsistentValueTypesInConditionProperties, reader);
					}
					shortList2.Add(shortList3);
					this.VerifyEndTag(reader, "keyValues");
					this.ReadNext(reader);
				}
				if (!flag)
				{
					while (this.IsTag(reader, "value") && reader.NodeType == XmlNodeType.Element)
					{
						shortList.Add(this.ParseSimpleValue(reader));
						flag = false;
						this.ReadNext(reader);
					}
				}
				this.VerifyEndTag(reader, conditionName);
			}
			if (flag)
			{
				return this.CreatePredicate(conditionName, property, shortList2, creationContext);
			}
			return this.CreatePredicate(conditionName, property, shortList, creationContext);
		}

		private string ParseSimpleValue(XmlReader reader)
		{
			this.ReadNext(reader, false);
			if (reader.NodeType != XmlNodeType.Text && reader.NodeType != XmlNodeType.Whitespace)
			{
				throw new ParserException(RulesStrings.ValueTextNotFound, reader);
			}
			string value = reader.Value;
			this.ReadNext(reader);
			this.VerifyEndTag(reader, "value");
			return value;
		}

		private Argument ParseArgument(XmlReader reader)
		{
			string attribute = reader.GetAttribute("value");
			string attribute2 = reader.GetAttribute("property");
			if (attribute != null && attribute2 != null)
			{
				throw new ParserException(RulesStrings.ArgumentIncorrect, reader);
			}
			Argument result;
			if (attribute != null)
			{
				result = new Value(attribute);
			}
			else
			{
				string attribute3 = reader.GetAttribute("type");
				if (string.IsNullOrEmpty(attribute3))
				{
					result = this.CreateProperty(attribute2);
				}
				else
				{
					result = this.CreateProperty(attribute2, attribute3);
				}
			}
			if (!reader.IsEmptyElement)
			{
				this.ReadNext(reader);
				this.VerifyEndTag(reader, "argument");
			}
			return result;
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
					throw new ParserException(RulesStrings.EndOfStream, reader);
				}
			}
		}

		protected void ReadNext(XmlReader reader, bool ignoreWhiteSpace)
		{
			if (!reader.Read())
			{
				throw new ParserException(RulesStrings.EndOfStream, reader);
			}
			while (ignoreWhiteSpace && reader.NodeType == XmlNodeType.Whitespace)
			{
				if (!reader.Read())
				{
					throw new ParserException(RulesStrings.EndOfStream, reader);
				}
			}
		}

		protected void VerifyEndTag(XmlReader reader, string tagName)
		{
			if (reader.NodeType != XmlNodeType.EndElement || !reader.Name.Equals(tagName))
			{
				throw new ParserException(RulesStrings.EndTagNotFound(tagName), reader);
			}
		}

		protected void VerifyTag(XmlReader reader, string tagName)
		{
			if (reader.NodeType != XmlNodeType.Element || !reader.Name.Equals(tagName))
			{
				throw new ParserException(RulesStrings.TagNotFound(tagName), reader);
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
				throw new ParserException(RulesStrings.EmptyTag(reader.Name), reader);
			}
		}

		protected string GetRequiredAttribute(XmlReader reader, string attributeName)
		{
			string attribute = reader.GetAttribute(attributeName);
			if (attribute == null)
			{
				throw new ParserException(RulesStrings.AttributeNotFound(attributeName, reader.Name), reader);
			}
			return attribute;
		}

		protected void VerifyAttributeValue(XmlReader reader, string attributeName, string attributeValue)
		{
			if (string.IsNullOrWhiteSpace(attributeValue))
			{
				throw new ParserException(RulesStrings.AttributeNotFound(attributeName, reader.Name), reader);
			}
		}
	}
}
