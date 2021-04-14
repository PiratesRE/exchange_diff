using System;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class SetAuditSeverityAction : TransportRuleAction, IEquatable<SetAuditSeverityAction>
	{
		public override int GetHashCode()
		{
			if (!string.IsNullOrEmpty(this.SeverityLevel))
			{
				return this.SeverityLevel.GetHashCode();
			}
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as SetAuditSeverityAction)));
		}

		public bool Equals(SetAuditSeverityAction other)
		{
			if (string.IsNullOrEmpty(this.SeverityLevel))
			{
				return string.IsNullOrEmpty(other.SeverityLevel);
			}
			return this.SeverityLevel.Equals(other.SeverityLevel);
		}

		[ActionParameterName("SetAuditSeverity")]
		[LocDisplayName(RulesTasksStrings.IDs.SetAuditSeverityDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.SetAuditSeverityDescription)]
		public string SeverityLevel { get; set; }

		internal override string Description
		{
			get
			{
				string severityLevel = string.Empty;
				if (!string.IsNullOrEmpty(this.SeverityLevel))
				{
					severityLevel = LocalizedDescriptionAttribute.FromEnum(typeof(AuditSeverityLevel), Enum.Parse(typeof(AuditSeverityLevel), this.SeverityLevel, false));
				}
				return RulesTasksStrings.RuleDescriptionSetAuditSeverity(severityLevel);
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "AuditSeverityLevel")
			{
				return null;
			}
			SetAuditSeverityAction setAuditSeverityAction = new SetAuditSeverityAction();
			TransportRuleAction result;
			try
			{
				setAuditSeverityAction.SeverityLevel = TransportRuleAction.GetStringValue(action.Arguments[0]);
				result = setAuditSeverityAction;
			}
			catch (ArgumentOutOfRangeException)
			{
				result = null;
			}
			return result;
		}

		internal override Action ToInternalAction()
		{
			ShortList<Argument> arguments = new ShortList<Argument>
			{
				new Value(this.SeverityLevel)
			};
			return TransportRuleParser.Instance.CreateAction("AuditSeverityLevel", arguments, Utils.GetActionName(this));
		}

		internal override string GetActionParameters()
		{
			return Utils.QuoteCmdletParameter(this.SeverityLevel);
		}
	}
}
