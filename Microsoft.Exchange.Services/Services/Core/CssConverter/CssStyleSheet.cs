using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Services.Core.CssConverter
{
	public class CssStyleSheet
	{
		public CssStyleSheet(List<CssRule> rules)
		{
			this.Rules = rules;
		}

		public List<CssRule> Rules { get; private set; }

		public void ScopeRulesToClass(string className)
		{
			foreach (CssRule cssRule in this.Rules)
			{
				foreach (CssSelector cssSelector in cssRule.Selectors)
				{
					if (!cssSelector.IsDirective)
					{
						cssSelector.PrependClass(className);
					}
				}
			}
		}

		public void SanitizeRules()
		{
			List<CssRule> list = new List<CssRule>();
			foreach (CssRule cssRule in this.Rules)
			{
				if (!cssRule.IsDirective)
				{
					if (cssRule.ContainsUnsafePseudoClasses)
					{
						List<CssSelector> list2 = new List<CssSelector>();
						foreach (CssSelector cssSelector in cssRule.Selectors)
						{
							if (!cssSelector.ContainsUnsafePseudoClasses)
							{
								list2.Add(cssSelector);
							}
						}
						cssRule.Selectors = list2;
					}
					if (cssRule.Selectors.Count > 0)
					{
						list.Add(cssRule);
					}
				}
			}
			this.Rules = list;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (CssRule value in this.Rules)
			{
				stringBuilder.Append(value);
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}
	}
}
