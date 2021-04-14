using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class RuleSerializer
	{
		public void SaveRulesToStream(RuleCollection rules, Stream stream)
		{
			XmlWriterSettings settings = new XmlWriterSettings
			{
				CloseOutput = false,
				Indent = false
			};
			if (stream.CanSeek)
			{
				stream.Seek(0L, SeekOrigin.Begin);
			}
			XmlWriter xmlWriter = XmlWriter.Create(stream, settings);
			this.SaveRules(xmlWriter, rules);
		}

		public string SaveRuleToString(Rule rule)
		{
			XmlWriterSettings settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = false
			};
			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings);
				this.SaveRule(xmlWriter, rule);
				xmlWriter.Close();
				result = stringWriter.ToString();
			}
			return result;
		}

		protected virtual void SaveRuleSubElements(XmlWriter writer, Rule rule)
		{
		}

		protected virtual void SaveProperty(XmlWriter xmlWriter, Property property)
		{
			xmlWriter.WriteAttributeString("property", property.Name);
		}

		protected virtual void SaveValue(XmlWriter xmlWriter, Value value)
		{
			if (value != null)
			{
				if (value.ParsedValue is ShortList<ShortList<KeyValuePair<string, string>>>)
				{
					ShortList<ShortList<KeyValuePair<string, string>>> shortList = (ShortList<ShortList<KeyValuePair<string, string>>>)value.ParsedValue;
					using (ShortList<ShortList<KeyValuePair<string, string>>>.Enumerator enumerator = shortList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ShortList<KeyValuePair<string, string>> shortList2 = enumerator.Current;
							xmlWriter.WriteStartElement("keyValues");
							foreach (KeyValuePair<string, string> keyValuePair in shortList2)
							{
								xmlWriter.WriteStartElement("keyValue");
								xmlWriter.WriteAttributeString("key", keyValuePair.Key);
								xmlWriter.WriteAttributeString("value", keyValuePair.Value);
								xmlWriter.WriteEndElement();
							}
							xmlWriter.WriteEndElement();
						}
						return;
					}
				}
				foreach (string value2 in value.RawValues)
				{
					xmlWriter.WriteStartElement("value");
					xmlWriter.WriteValue(value2);
					xmlWriter.WriteEndElement();
				}
			}
		}

		protected virtual void SaveRuleAttributes(XmlWriter xmlWriter, Rule rule)
		{
			if (!string.IsNullOrEmpty(rule.Comments))
			{
				xmlWriter.WriteAttributeString("comments", rule.Comments);
			}
			if (rule.Enabled != RuleState.Enabled)
			{
				xmlWriter.WriteAttributeString("enabled", RuleConstants.StringFromRuleState(rule.Enabled));
			}
			if (rule.ExpiryDate != null)
			{
				xmlWriter.WriteAttributeString("expiryDate", RuleUtils.DateTimeToUtcString(rule.ExpiryDate.Value));
			}
			if (rule.ActivationDate != null)
			{
				xmlWriter.WriteAttributeString("activationDate", RuleUtils.DateTimeToUtcString(rule.ActivationDate.Value));
			}
			if (rule.ImmutableId != Guid.Empty)
			{
				xmlWriter.WriteAttributeString("id", rule.ImmutableId.ToString());
			}
			if (rule.Mode != RuleMode.Enforce)
			{
				xmlWriter.WriteAttributeString("mode", Enum.GetName(typeof(RuleMode), rule.Mode));
			}
			if (rule.SubType != RuleSubType.None)
			{
				xmlWriter.WriteAttributeString("subType", Enum.GetName(typeof(RuleSubType), rule.SubType));
			}
			if (rule.ErrorAction != RuleErrorAction.Ignore)
			{
				xmlWriter.WriteAttributeString("errorAction", Enum.GetName(typeof(RuleErrorAction), rule.ErrorAction));
			}
		}

		private void SaveRules(XmlWriter xmlWriter, RuleCollection rules)
		{
			xmlWriter.WriteStartElement("rules");
			xmlWriter.WriteAttributeString("name", rules.Name);
			foreach (Rule rule in rules)
			{
				this.SaveRule(xmlWriter, rule);
			}
			xmlWriter.WriteEndElement();
			xmlWriter.Close();
		}

		private void SaveRule(XmlWriter xmlWriter, Rule rule)
		{
			xmlWriter.WriteStartElement("rule");
			xmlWriter.WriteAttributeString("name", rule.Name);
			this.SaveRuleAttributes(xmlWriter, rule);
			xmlWriter.WriteStartElement("version");
			xmlWriter.WriteAttributeString("requiredMinVersion", rule.MinimumVersion.ToString());
			this.SaveRuleSubElements(xmlWriter, rule);
			this.SaveTags(xmlWriter, rule.GetTags());
			this.SaveCondition(xmlWriter, rule.Condition);
			foreach (Action action in rule.Actions)
			{
				this.SaveAction(xmlWriter, action);
			}
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}

		private void SaveTags(XmlWriter xmlWriter, IEnumerable<RuleTag> ruleTags)
		{
			if (ruleTags.Any<RuleTag>())
			{
				xmlWriter.WriteStartElement("tags");
				foreach (RuleTag ruleTag in ruleTags)
				{
					xmlWriter.WriteStartElement("tag");
					xmlWriter.WriteAttributeString("name", ruleTag.Name);
					xmlWriter.WriteAttributeString("type", ruleTag.TagType);
					foreach (KeyValuePair<string, string> keyValuePair in ruleTag.Data)
					{
						xmlWriter.WriteAttributeString(keyValuePair.Key, keyValuePair.Value);
					}
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
			}
		}

		private void SaveCondition(XmlWriter xmlWriter, Condition condition)
		{
			xmlWriter.WriteStartElement("condition");
			this.SaveSubCondition(xmlWriter, condition);
			xmlWriter.WriteEndElement();
		}

		private void SaveSubCondition(XmlWriter xmlWriter, Condition condition)
		{
			switch (condition.ConditionType)
			{
			case ConditionType.And:
				xmlWriter.WriteStartElement("and");
				using (List<Condition>.Enumerator enumerator = ((AndCondition)condition).SubConditions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Condition condition2 = enumerator.Current;
						this.SaveSubCondition(xmlWriter, condition2);
					}
					goto IL_134;
				}
				break;
			case ConditionType.Or:
				break;
			case ConditionType.Not:
				xmlWriter.WriteStartElement("not");
				this.SaveSubCondition(xmlWriter, ((NotCondition)condition).SubCondition);
				goto IL_134;
			case ConditionType.True:
				xmlWriter.WriteStartElement("true");
				goto IL_134;
			case ConditionType.False:
				xmlWriter.WriteStartElement("false");
				goto IL_134;
			case ConditionType.Predicate:
				goto IL_FF;
			default:
				throw new NotSupportedException();
			}
			xmlWriter.WriteStartElement("or");
			using (List<Condition>.Enumerator enumerator2 = ((OrCondition)condition).SubConditions.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Condition condition3 = enumerator2.Current;
					this.SaveSubCondition(xmlWriter, condition3);
				}
				goto IL_134;
			}
			IL_FF:
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			xmlWriter.WriteStartElement(predicateCondition.Name);
			this.SaveProperty(xmlWriter, predicateCondition.Property);
			this.SaveValue(xmlWriter, predicateCondition.Value);
			IL_134:
			xmlWriter.WriteEndElement();
		}

		private void SaveAction(XmlWriter xmlWriter, Action action)
		{
			xmlWriter.WriteStartElement("action");
			xmlWriter.WriteAttributeString("name", action.Name);
			if (action.HasExternalName)
			{
				xmlWriter.WriteAttributeString("externalName", action.ExternalName);
			}
			foreach (Argument argument in action.Arguments)
			{
				this.SaveActionArgument(xmlWriter, argument);
			}
			xmlWriter.WriteEndElement();
		}

		private void SaveActionArgument(XmlWriter xmlWriter, Argument argument)
		{
			if (!(argument is Value))
			{
				if (argument is Property)
				{
					xmlWriter.WriteStartElement("argument");
					this.SaveProperty(xmlWriter, argument as Property);
					xmlWriter.WriteEndElement();
				}
				return;
			}
			Value value = argument as Value;
			xmlWriter.WriteStartElement("argument");
			if (value.RawValues.Count != 1)
			{
				throw new InvalidOperationException("Action argument can only have one value!");
			}
			xmlWriter.WriteAttributeString("value", value.RawValues[0]);
			xmlWriter.WriteEndElement();
		}
	}
}
