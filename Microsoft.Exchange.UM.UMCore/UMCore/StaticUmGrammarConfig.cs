using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class StaticUmGrammarConfig : UMGrammarConfig
	{
		internal StaticUmGrammarConfig(string name, string rule, string condition, ActivityManagerConfig managerConfig) : base(name, rule, condition, managerConfig)
		{
		}

		internal override UMGrammar GetGrammar(ActivityManager manager, CultureInfo culture)
		{
			string path = Path.Combine(Utils.GrammarPathFromCulture(culture), base.GrammarName);
			return new UMGrammar(path, base.GrammarRule, culture);
		}

		internal override void Validate()
		{
			UMGrammar grammar = this.GetGrammar(null, GlobCfg.VuiCultures[0]);
			this.ValidateGrammar(grammar.Path, grammar.RuleName);
		}

		private static bool ValidateRuleName(XmlReader xmlReader, string ruleName)
		{
			if (xmlReader.MoveToFirstAttribute())
			{
				while (string.Compare(xmlReader.Name, "id", StringComparison.Ordinal) != 0 || string.Compare(xmlReader.Value, ruleName, StringComparison.Ordinal) != 0)
				{
					if (!xmlReader.MoveToNextAttribute())
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private static bool ValidateRuleScope(XmlReader xmlReader)
		{
			if (xmlReader.MoveToFirstAttribute())
			{
				while (string.Compare(xmlReader.Name, "scope", StringComparison.Ordinal) != 0 || string.Compare(xmlReader.Value, "public", StringComparison.Ordinal) != 0)
				{
					if (!xmlReader.MoveToNextAttribute())
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private static bool ValidateRecoEventDeclaration(XmlReader xmlReader)
		{
			xmlReader.MoveToContent();
			xmlReader.Read();
			xmlReader.MoveToContent();
			return xmlReader.NodeType == XmlNodeType.Element && string.Compare(xmlReader.LocalName, "Tag", StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(xmlReader.ReadInnerXml(), "$.RecoEvent={};", StringComparison.Ordinal) == 0;
		}

		private static bool ValidateRecoEventTags(XmlReader xmlReader, out string invalidEventName)
		{
			invalidEventName = string.Empty;
			while (xmlReader.Read() && (xmlReader.NodeType != XmlNodeType.EndElement || string.Compare(xmlReader.Name, "rule", StringComparison.OrdinalIgnoreCase) != 0))
			{
				if (xmlReader.NodeType == XmlNodeType.Element && string.Compare(xmlReader.LocalName, "Tag", StringComparison.OrdinalIgnoreCase) == 0)
				{
					Match match = Regex.Match(xmlReader.ReadInnerXml(), "\\s*\\$\\.RecoEvent._value=\"(?<eventName>[^\"]*)\"");
					if (match.Success && !ConstantValidator.Instance.ValidateRecoEvent(match.Groups["eventName"].Value))
					{
						invalidEventName = match.Groups["eventName"].Value;
						return false;
					}
				}
			}
			return true;
		}

		private void ValidateGrammar(string path, string ruleName)
		{
			bool flag = false;
			bool flag2 = false;
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				using (XmlReader xmlReader = XmlReader.Create(fileStream))
				{
					while (xmlReader.Read())
					{
						if (xmlReader.NodeType == XmlNodeType.Element && string.Compare(xmlReader.LocalName, "rule", StringComparison.OrdinalIgnoreCase) == 0 && StaticUmGrammarConfig.ValidateRuleName(xmlReader, ruleName))
						{
							if (flag2)
							{
								throw new FsmConfigurationException(Strings.DuplicateGrammarRule(path, ruleName));
							}
							flag2 = true;
							if (!StaticUmGrammarConfig.ValidateRuleScope(xmlReader))
							{
								throw new FsmConfigurationException(Strings.RuleNotPublic(path, ruleName));
							}
							if (!StaticUmGrammarConfig.ValidateRecoEventDeclaration(xmlReader))
							{
								throw new FsmConfigurationException(Strings.InvalidRecoEventDeclaration(path, ruleName));
							}
							string name;
							if (!StaticUmGrammarConfig.ValidateRecoEventTags(xmlReader, out name))
							{
								throw new FsmConfigurationException(Strings.UndeclaredRecoEventName(path, ruleName, name));
							}
							flag = true;
						}
					}
				}
			}
			if (!flag)
			{
				throw new FsmConfigurationException(Strings.UnknownGrammarRule(path, ruleName));
			}
		}
	}
}
