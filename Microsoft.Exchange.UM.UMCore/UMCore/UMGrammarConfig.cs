using System;
using System.Globalization;
using System.Xml;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class UMGrammarConfig
	{
		internal UMGrammarConfig(string name, string rule, string conditionString, ActivityManagerConfig managerConfig)
		{
			this.grammarName = name;
			this.grammarRule = rule;
			if (conditionString.Length > 0)
			{
				this.conditionTree = ConditionParser.Instance.Parse(conditionString, managerConfig);
			}
		}

		internal ExpressionParser.Expression Condition
		{
			get
			{
				return this.conditionTree;
			}
		}

		protected internal string GrammarName
		{
			internal get
			{
				return this.grammarName;
			}
			set
			{
				this.grammarName = value;
			}
		}

		protected internal string GrammarRule
		{
			internal get
			{
				return this.grammarRule;
			}
			set
			{
				this.grammarRule = value;
			}
		}

		internal static UMGrammarConfig Create(XmlNode node, string conditionString, ActivityManagerConfig managerConfig)
		{
			string value = node.Attributes["name"].Value;
			string value2 = node.Attributes["type"].Value;
			string value3 = node.Attributes["condition"].Value;
			string value4 = node.Attributes["rule"].Value;
			return UMGrammarConfig.Create(value, value2, value3, value4, conditionString, managerConfig);
		}

		internal static UMGrammarConfig Create(string name, string type, string condition, string rule, string conditionString, ActivityManagerConfig managerConfig)
		{
			UMGrammarConfig umgrammarConfig = null;
			if (type != null)
			{
				if (!(type == "static"))
				{
					if (type == "dynamic")
					{
						umgrammarConfig = new DynamicUmGrammarConfig(name, rule, condition, managerConfig);
					}
				}
				else
				{
					umgrammarConfig = new StaticUmGrammarConfig(name, rule, condition, managerConfig);
				}
			}
			foreach (CultureInfo cultureInfo in GlobCfg.VuiCultures)
			{
				umgrammarConfig.Validate();
			}
			return umgrammarConfig;
		}

		internal abstract UMGrammar GetGrammar(ActivityManager manager, CultureInfo culture);

		internal abstract void Validate();

		private string grammarName;

		private string grammarRule;

		private ExpressionParser.Expression conditionTree;
	}
}
