using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.Services.Core.CssConverter
{
	public class CssRule
	{
		public CssRule(IList<string> selectors, IList<CssProperty> definitions)
		{
			this.Selectors = selectors.ToList<string>().ConvertAll<CssSelector>((string str) => new CssSelector(str));
			this.Definitions = definitions;
		}

		public IList<CssSelector> Selectors { get; set; }

		public IList<CssProperty> Definitions { get; set; }

		public bool IsDirective
		{
			get
			{
				return this.Selectors.Any((CssSelector s) => s.IsDirective);
			}
		}

		public bool ContainsUnsafePseudoClasses
		{
			get
			{
				return this.Selectors.Any((CssSelector s) => s.ContainsUnsafePseudoClasses);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (CssSelector value in this.Selectors)
			{
				if (!flag)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(value);
				flag = false;
			}
			stringBuilder.Append("\n{ ");
			foreach (CssProperty value2 in this.Definitions)
			{
				stringBuilder.Append(value2);
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}
	}
}
