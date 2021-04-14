using System;
using System.Globalization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "RuleType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class EwsRule
	{
		[XmlElement(Order = 0)]
		public string RuleId { get; set; }

		[XmlIgnore]
		public bool RuleIdSpecified { get; set; }

		[XmlElement(Order = 1)]
		public string DisplayName { get; set; }

		[XmlElement(Order = 2)]
		public int Priority { get; set; }

		[XmlElement(Order = 3)]
		public bool IsEnabled { get; set; }

		[XmlElement(Order = 4)]
		public bool IsNotSupported { get; set; }

		[XmlIgnore]
		public bool IsNotSupportedSpecified { get; set; }

		[XmlElement(Order = 5)]
		public bool IsInError { get; set; }

		[XmlIgnore]
		public bool IsInErrorSpecified { get; set; }

		[XmlElement(Order = 6)]
		public RulePredicates Conditions { get; set; }

		[XmlElement(Order = 7)]
		public RulePredicates Exceptions { get; set; }

		[XmlElement(Order = 8)]
		public RuleActions Actions { get; set; }

		[XmlIgnore]
		internal Rule ServerRule { get; set; }

		internal static EwsRule CreateFromServerRule(Rule serverRule, int hashCode, MailboxSession session, CultureInfo clientCulture)
		{
			EwsRule ewsRule = new EwsRule();
			ewsRule.RuleId = serverRule.Id.ToBase64String();
			ewsRule.RuleIdSpecified = true;
			ewsRule.DisplayName = serverRule.Name;
			ewsRule.Priority = serverRule.Sequence - 10 + 1;
			ewsRule.IsEnabled = serverRule.IsEnabled;
			ewsRule.IsNotSupported = serverRule.IsNotSupported;
			ewsRule.IsInError = serverRule.IsParameterInError;
			if (ewsRule.IsInError)
			{
				ewsRule.IsInErrorSpecified = true;
			}
			if (!ewsRule.IsNotSupported && 0 < serverRule.Conditions.Count)
			{
				ewsRule.Conditions = RulePredicates.CreateFromServerRuleConditions(serverRule.Conditions, ewsRule, hashCode, clientCulture);
			}
			if (!ewsRule.IsNotSupported && 0 < serverRule.Exceptions.Count)
			{
				ewsRule.Exceptions = RulePredicates.CreateFromServerRuleConditions(serverRule.Exceptions, ewsRule, hashCode, clientCulture);
			}
			if (!ewsRule.IsNotSupported && 0 < serverRule.Actions.Count)
			{
				ewsRule.Actions = RuleActions.CreateFromServerRuleActions(serverRule.Actions, ewsRule, hashCode, session);
			}
			if (ewsRule.IsNotSupported)
			{
				ewsRule.IsNotSupportedSpecified = true;
			}
			return ewsRule;
		}

		internal bool HasActions()
		{
			return this.Actions != null && this.Actions.HasActions();
		}

		internal const int BaseSequence = 10;
	}
}
