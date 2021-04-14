using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	[Serializable]
	public class RuleDescription
	{
		internal string ActivationDescription { get; set; }

		internal string ExpiryDescription { get; set; }

		internal List<string> ActionDescriptions
		{
			get
			{
				return this.actionDescriptions;
			}
		}

		internal List<string> ConditionDescriptions
		{
			get
			{
				return this.conditionDescriptions;
			}
		}

		internal List<string> ExceptionDescriptions
		{
			get
			{
				return this.exceptionDescriptions;
			}
		}

		internal virtual string RuleDescriptionIf
		{
			get
			{
				return RulesStrings.RuleDescriptionIf;
			}
		}

		internal virtual string RuleDescriptionTakeActions
		{
			get
			{
				return RulesStrings.RuleDescriptionTakeActions;
			}
		}

		internal virtual string RuleDescriptionExceptIf
		{
			get
			{
				return RulesStrings.RuleDescriptionExceptIf;
			}
		}

		internal virtual string RuleDescriptionActivation
		{
			get
			{
				return RulesStrings.RuleDescriptionActivation;
			}
		}

		internal virtual string RuleDescriptionExpiry
		{
			get
			{
				return RulesStrings.RuleDescriptionExpiry;
			}
		}

		public static string BuildDescriptionStringFromStringArray(ICollection<string> stringValues, string delimiter, int maxDescriptionLength = 200)
		{
			if (stringValues == null || stringValues.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(40 * stringValues.Count);
			stringBuilder.Append("'");
			bool flag = true;
			foreach (string value in stringValues)
			{
				if (stringBuilder.Length > maxDescriptionLength)
				{
					stringBuilder.Append(delimiter);
					stringBuilder.Append("...");
					break;
				}
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(delimiter);
					stringBuilder.Append(" '");
				}
				stringBuilder.Append(value);
				stringBuilder.Append("'");
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			int num = this.ConditionDescriptions.Count + this.ActionDescriptions.Count + this.ExceptionDescriptions.Count + (string.IsNullOrEmpty(this.ActivationDescription) ? 0 : 1) + (string.IsNullOrEmpty(this.ExpiryDescription) ? 0 : 1);
			StringBuilder stringBuilder = new StringBuilder(num * 50);
			if (this.ConditionDescriptions.Any<string>())
			{
				stringBuilder.Append(this.RuleDescriptionIf);
				stringBuilder.Append("\r\n");
				this.BuildDescriptionStrings(stringBuilder, this.ConditionDescriptions, RulesStrings.RuleDescriptionAndDelimiter);
			}
			if (this.ActionDescriptions.Any<string>())
			{
				stringBuilder.Append(this.RuleDescriptionTakeActions);
				stringBuilder.Append("\r\n");
				this.BuildDescriptionStrings(stringBuilder, this.ActionDescriptions, RulesStrings.RuleDescriptionAndDelimiter);
			}
			if (this.ExceptionDescriptions.Any<string>())
			{
				stringBuilder.Append(this.RuleDescriptionExceptIf);
				stringBuilder.Append("\r\n");
				this.BuildDescriptionStrings(stringBuilder, this.ExceptionDescriptions, RulesStrings.RuleDescriptionOrDelimiter);
			}
			if (!string.IsNullOrEmpty(this.ActivationDescription))
			{
				stringBuilder.Append(this.RuleDescriptionActivation);
				stringBuilder.Append(" ");
				stringBuilder.Append(this.ActivationDescription);
				stringBuilder.Append("\r\n");
			}
			if (!string.IsNullOrEmpty(this.ExpiryDescription))
			{
				stringBuilder.Append(this.RuleDescriptionExpiry);
				stringBuilder.Append(" ");
				stringBuilder.Append(this.ExpiryDescription);
				stringBuilder.Append("\r\n");
			}
			return stringBuilder.ToString();
		}

		private void BuildDescriptionStrings(StringBuilder sb, List<string> descriptions, string delimiter)
		{
			if (descriptions == null || descriptions.Count == 0)
			{
				return;
			}
			bool flag = true;
			foreach (string value in descriptions)
			{
				sb.Append("\t");
				if (flag)
				{
					flag = false;
				}
				else
				{
					sb.Append(delimiter);
					sb.Append(" ");
				}
				sb.Append(value);
				sb.Append("\r\n");
			}
		}

		internal const string CrLfString = "\r\n";

		internal const string TabString = "\t";

		internal const string SpaceString = " ";

		public const int MaxDescriptionLength = 200;

		private readonly List<string> actionDescriptions = new List<string>();

		private readonly List<string> conditionDescriptions = new List<string>();

		private readonly List<string> exceptionDescriptions = new List<string>();
	}
}
