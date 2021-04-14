using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class ClientAccessRulesUsernamePatternProperty : Property
	{
		public ClientAccessRulesUsernamePatternProperty(string propertyName, Type type) : base(propertyName, type)
		{
		}

		protected override object OnGetValue(RulesEvaluationContext baseContext)
		{
			ClientAccessRulesEvaluationContext clientAccessRulesEvaluationContext = (ClientAccessRulesEvaluationContext)baseContext;
			return clientAccessRulesEvaluationContext.UserName;
		}

		public static string GetDisplayValue(Regex regex)
		{
			string text = regex.ToString();
			text = text.Substring(1, text.Length - 2);
			text = text.Replace(".*", "*");
			return Regex.Unescape(text);
		}

		public static Regex GetWildcardPatternRegex(string pattern)
		{
			pattern = Regex.Escape(pattern);
			pattern = pattern.Replace("\\*", ".*");
			return new Regex(string.Format("^{0}$", pattern), RegexOptions.IgnoreCase);
		}

		public const string PropertyName = "UsernamePatternProperty";
	}
}
