using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.MessagingPolicies;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class RuleDataService : DataSourceService
	{
		public abstract int RuleNameMaxLength { get; }

		public string TaskNoun
		{
			get
			{
				return this.taskNoun;
			}
		}

		public abstract RulePhrase[] SupportedConditions { get; }

		public abstract RulePhrase[] SupportedActions { get; }

		public abstract RulePhrase[] SupportedExceptions { get; }

		protected RuleDataService(string taskNoun)
		{
			this.taskNoun = taskNoun;
		}

		protected PowerShellResults ChangePriority<T>(Identity[] identities, int offset, WebServiceParameters parameters) where T : RuleRow
		{
			identities.FaultIfNotExactlyOne();
			PowerShellResults<T> @object = base.GetObject<T>("Get-" + this.TaskNoun, identities[0]);
			if (@object.Failed)
			{
				return @object;
			}
			int num = @object.Output[0].Priority + offset;
			if (num < 0)
			{
				return new PowerShellResults();
			}
			PSCommand pscommand = new PSCommand().AddCommand("Set-" + this.TaskNoun);
			pscommand.AddParameter("Priority", num);
			PowerShellResults powerShellResults = base.Invoke(pscommand, identities, parameters);
			if (powerShellResults.ErrorRecords != null && powerShellResults.ErrorRecords.Length == 1 && powerShellResults.ErrorRecords[0].Exception is InvalidPriorityException)
			{
				powerShellResults.ErrorRecords = new ErrorRecord[0];
			}
			return powerShellResults;
		}

		protected string GetUniqueRuleName(string ruleName, RuleRow[] existingRules)
		{
			if (string.IsNullOrEmpty(ruleName))
			{
				return ruleName;
			}
			int ruleNameMaxLength = this.RuleNameMaxLength;
			if (Array.FindIndex<RuleRow>(existingRules, (RuleRow x) => x.Name == ruleName) == -1 && ruleName.Length <= ruleNameMaxLength)
			{
				return ruleName;
			}
			string text = ruleName;
			string text2 = string.Empty;
			if (ruleName.EndsWith("..."))
			{
				text = ruleName.Substring(0, ruleName.Length - 3);
				text2 = "...";
			}
			StringBuilder stringBuilder = new StringBuilder(ruleNameMaxLength);
			if (ruleName.Length > ruleNameMaxLength)
			{
				stringBuilder.Append(text.SurrogateSubstring(0, ruleNameMaxLength - text2.Length));
				stringBuilder.Append(text2);
				ruleName = stringBuilder.ToString().TrimEnd(new char[0]);
			}
			int num = 2;
			for (;;)
			{
				if (Array.FindIndex<RuleRow>(existingRules, (RuleRow x) => x.Name == ruleName) < 0)
				{
					break;
				}
				string text3 = num.ToString();
				int num2 = 1 + (RtlUtil.IsRtl ? 1 : 0);
				if (text.Length + text2.Length + text3.Length + num2 > ruleNameMaxLength)
				{
					text = text.SurrogateSubstring(0, ruleNameMaxLength - text2.Length - text3.Length - num2);
				}
				stringBuilder.Length = 0;
				stringBuilder.Append(text);
				stringBuilder.Append(text2);
				stringBuilder.Append(' ');
				if (RtlUtil.IsRtl)
				{
					stringBuilder.Append(RtlUtil.DecodedRtlDirectionMark);
				}
				stringBuilder.Append(text3);
				ruleName = stringBuilder.ToString();
				num++;
			}
			return ruleName;
		}

		internal const string Conditions = "Conditions";

		internal const string Actions = "Actions";

		internal const string Exceptions = "Exceptions";

		private string taskNoun;
	}
}
