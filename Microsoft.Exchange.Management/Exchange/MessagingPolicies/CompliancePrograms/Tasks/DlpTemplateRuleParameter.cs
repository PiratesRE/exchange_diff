using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	[Serializable]
	public class DlpTemplateRuleParameter
	{
		public string Type { get; set; }

		public bool Required { get; set; }

		public string Token { get; set; }

		public CultureInfo CurrentCulture
		{
			set
			{
				this.currentCulture = value;
			}
		}

		public Dictionary<string, string> LocalizedDescriptions { get; set; }

		public string Description
		{
			get
			{
				return DlpPolicyTemplateMetaData.GetLocalizedStringValue(this.LocalizedDescriptions, this.currentCulture);
			}
		}

		public string ToString(CultureInfo culture)
		{
			CultureInfo cultureInfo = this.currentCulture;
			this.currentCulture = culture;
			string result = this.ToString();
			this.CurrentCulture = cultureInfo;
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(DlpTemplateRuleParameter.EscapeString(this.Type));
			stringBuilder.Append(";");
			stringBuilder.Append(this.Required);
			stringBuilder.Append(";");
			stringBuilder.Append(DlpTemplateRuleParameter.EscapeString(this.Token));
			stringBuilder.Append(";");
			stringBuilder.Append(DlpTemplateRuleParameter.EscapeString(this.Description));
			return stringBuilder.ToString();
		}

		private static string EscapeString(string input)
		{
			string text = input.Replace("\\", "\\\\");
			return text.Replace(";", "\\;");
		}

		private const string separator = ";";

		private const string escapeCharacter = "\\";

		private CultureInfo currentCulture = DlpPolicyTemplateMetaData.DefaultCulture;
	}
}
