using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	[Serializable]
	internal class ClientAccessRule : Rule
	{
		public ClientAccessRule(string name) : base(name, null)
		{
		}

		public ObjectId Identity { get; set; }

		public int Priority
		{
			get
			{
				return this.priority;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(RulesTasksStrings.NegativePriority, "Priority");
				}
				this.priority = value;
			}
		}

		public bool DatacenterAdminsOnly { get; set; }

		public IPRange[] AnyOfClientIPAddressesOrRanges { get; set; }

		public IPRange[] ExceptAnyOfClientIPAddressesOrRanges { get; set; }

		public IntRange[] AnyOfSourceTcpPortNumbers { get; set; }

		public IntRange[] ExceptAnyOfSourceTcpPortNumbers { get; set; }

		public string[] UsernameMatchesAnyOfPatterns { get; set; }

		public string[] ExceptUsernameMatchesAnyOfPatterns { get; set; }

		public string[] UserIsMemberOf { get; set; }

		public string[] ExceptUserIsMemberOf { get; set; }

		public ClientAccessAuthenticationMethod[] AnyOfAuthenticationTypes { get; set; }

		public ClientAccessAuthenticationMethod[] ExceptAnyOfAuthenticationTypes { get; set; }

		public ClientAccessProtocol[] AnyOfProtocols { get; set; }

		public ClientAccessProtocol[] ExceptAnyOfProtocols { get; set; }

		public ClientAccessRulesAction Action { get; set; }

		public string UserRecipientFilter { get; set; }

		public string Xml
		{
			get
			{
				this.PopulateRuleConditionAndAction();
				return ClientAccessRuleSerializer.Instance.SaveRuleToString(this);
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				Version version = ClientAccessRule.ClientAccessRuleBaseVersion;
				foreach (Microsoft.Exchange.MessagingPolicies.Rules.Action action in base.Actions)
				{
					version = ((version > action.MinimumVersion) ? version : action.MinimumVersion);
				}
				if (base.Condition.MinimumVersion > version)
				{
					return base.Condition.MinimumVersion;
				}
				return version;
			}
		}

		public override int GetEstimatedSize()
		{
			int num = 5;
			return num + base.GetEstimatedSize();
		}

		public static ClientAccessRule FromADProperties(string xml, ObjectId identity, string name, int priority, bool enabled, bool datacenterAdminsOnly, bool populatePresentationProperties)
		{
			ClientAccessRule clientAccessRule = (ClientAccessRule)ClientAccessRuleParser.Instance.GetRule(xml);
			clientAccessRule.Identity = identity;
			clientAccessRule.Name = name;
			clientAccessRule.Priority = priority;
			clientAccessRule.Enabled = (enabled ? RuleState.Enabled : RuleState.Disabled);
			clientAccessRule.DatacenterAdminsOnly = datacenterAdminsOnly;
			if (populatePresentationProperties)
			{
				if (clientAccessRule.Actions.Count > 0)
				{
					clientAccessRule.Action = (ClientAccessRulesAction)Enum.Parse(typeof(ClientAccessRulesAction), clientAccessRule.Actions[0].ExternalName);
				}
				ClientAccessRule.FillParametersFromCondition(clientAccessRule, clientAccessRule.Condition, false);
			}
			return clientAccessRule;
		}

		private void AddPredicateToCondition<T>(List<Condition> subConditions, string conditionTag, string propertyName, bool negateCondition, T[] values)
		{
			if (values == null || values.Length == 0)
			{
				return;
			}
			PredicateCondition predicateCondition = ClientAccessRuleParser.Instance.CreatePredicate(conditionTag, ClientAccessRuleParser.Instance.CreateProperty(propertyName), from value in values
			select value.ToString());
			if (negateCondition)
			{
				subConditions.Add(new NotCondition(predicateCondition));
				return;
			}
			subConditions.Add(predicateCondition);
		}

		private void PopulateRuleConditionAndAction()
		{
			AndCondition andCondition = new AndCondition();
			andCondition.SubConditions.Add(Condition.True);
			this.AddPredicateToCondition<IPRange>(andCondition.SubConditions, "anyOfClientIPAddressesOrRangesPredicate", "ClientIpProperty", false, this.AnyOfClientIPAddressesOrRanges);
			this.AddPredicateToCondition<IntRange>(andCondition.SubConditions, "anyOfSourceTcpPortNumbersPredicate", "SourceTcpPortNumberProperty", false, this.AnyOfSourceTcpPortNumbers);
			this.AddPredicateToCondition<ClientAccessProtocol>(andCondition.SubConditions, "anyOfProtocolsPredicate", "ProtocolProperty", false, this.AnyOfProtocols);
			this.AddPredicateToCondition<string>(andCondition.SubConditions, "usernameMatchesAnyOfPatternsPredicate", "UsernamePatternProperty", false, this.UsernameMatchesAnyOfPatterns);
			this.AddPredicateToCondition<ClientAccessAuthenticationMethod>(andCondition.SubConditions, "anyOfAuthenticationTypesPredicate", "AuthenticationTypeProperty", false, this.AnyOfAuthenticationTypes);
			if (!string.IsNullOrEmpty(this.UserRecipientFilter))
			{
				this.AddPredicateToCondition<string>(andCondition.SubConditions, "UserRecipientFilterPredicate", "UserRecipientFilterProperty", false, new string[]
				{
					this.UserRecipientFilter
				});
			}
			this.AddPredicateToCondition<IPRange>(andCondition.SubConditions, "anyOfClientIPAddressesOrRangesPredicate", "ClientIpProperty", true, this.ExceptAnyOfClientIPAddressesOrRanges);
			this.AddPredicateToCondition<IntRange>(andCondition.SubConditions, "anyOfSourceTcpPortNumbersPredicate", "SourceTcpPortNumberProperty", true, this.ExceptAnyOfSourceTcpPortNumbers);
			this.AddPredicateToCondition<ClientAccessProtocol>(andCondition.SubConditions, "anyOfProtocolsPredicate", "ProtocolProperty", true, this.ExceptAnyOfProtocols);
			this.AddPredicateToCondition<string>(andCondition.SubConditions, "usernameMatchesAnyOfPatternsPredicate", "UsernamePatternProperty", true, this.ExceptUsernameMatchesAnyOfPatterns);
			this.AddPredicateToCondition<ClientAccessAuthenticationMethod>(andCondition.SubConditions, "anyOfAuthenticationTypesPredicate", "AuthenticationTypeProperty", true, this.ExceptAnyOfAuthenticationTypes);
			base.Condition = andCondition;
			base.Actions.Clear();
			switch (this.Action)
			{
			case ClientAccessRulesAction.AllowAccess:
				base.Actions.Add(ClientAccessRuleParser.Instance.CreateAction("AllowAccess", new ShortList<Argument>(), this.Action.ToString()));
				return;
			case ClientAccessRulesAction.DenyAccess:
				base.Actions.Add(ClientAccessRuleParser.Instance.CreateAction("DenyAccess", new ShortList<Argument>(), this.Action.ToString()));
				return;
			default:
				throw new ClientAccessRuleActionNotSupportedException(ClientAccessRulesAction.DenyAccess.ToString());
			}
		}

		private static void FillParametersFromCondition(ClientAccessRule rule, Condition condition, bool wasNegated)
		{
			if (condition is AndCondition)
			{
				AndCondition andCondition = condition as AndCondition;
				foreach (Condition condition2 in andCondition.SubConditions)
				{
					ClientAccessRule.FillParametersFromCondition(rule, condition2, wasNegated);
				}
			}
			if (condition is NotCondition)
			{
				ClientAccessRule.FillParametersFromCondition(rule, ((NotCondition)condition).SubCondition, !wasNegated);
			}
			if (condition is AnyOfClientIPAddressesOrRangesPredicate)
			{
				AnyOfClientIPAddressesOrRangesPredicate anyOfClientIPAddressesOrRangesPredicate = condition as AnyOfClientIPAddressesOrRangesPredicate;
				if (wasNegated)
				{
					rule.ExceptAnyOfClientIPAddressesOrRanges = anyOfClientIPAddressesOrRangesPredicate.TargetIpRanges.ToArray<IPRange>();
				}
				else
				{
					rule.AnyOfClientIPAddressesOrRanges = anyOfClientIPAddressesOrRangesPredicate.TargetIpRanges.ToArray<IPRange>();
				}
			}
			if (condition is AnyOfSourceTcpPortNumbersPredicate)
			{
				AnyOfSourceTcpPortNumbersPredicate anyOfSourceTcpPortNumbersPredicate = condition as AnyOfSourceTcpPortNumbersPredicate;
				if (wasNegated)
				{
					rule.ExceptAnyOfSourceTcpPortNumbers = anyOfSourceTcpPortNumbersPredicate.TargetPortRanges.ToArray<IntRange>();
				}
				else
				{
					rule.AnyOfSourceTcpPortNumbers = anyOfSourceTcpPortNumbersPredicate.TargetPortRanges.ToArray<IntRange>();
				}
			}
			if (condition is AnyOfProtocolsPredicate)
			{
				AnyOfProtocolsPredicate anyOfProtocolsPredicate = condition as AnyOfProtocolsPredicate;
				if (wasNegated)
				{
					rule.ExceptAnyOfProtocols = anyOfProtocolsPredicate.ProtocolList.ToArray<ClientAccessProtocol>();
				}
				else
				{
					rule.AnyOfProtocols = anyOfProtocolsPredicate.ProtocolList.ToArray<ClientAccessProtocol>();
				}
			}
			if (condition is UsernameMatchesAnyOfPatternsPredicate)
			{
				UsernameMatchesAnyOfPatternsPredicate usernameMatchesAnyOfPatternsPredicate = condition as UsernameMatchesAnyOfPatternsPredicate;
				if (wasNegated)
				{
					rule.ExceptUsernameMatchesAnyOfPatterns = usernameMatchesAnyOfPatternsPredicate.RegexPatterns.Select(new Func<Regex, string>(ClientAccessRulesUsernamePatternProperty.GetDisplayValue)).ToArray<string>();
				}
				else
				{
					rule.UsernameMatchesAnyOfPatterns = usernameMatchesAnyOfPatternsPredicate.RegexPatterns.Select(new Func<Regex, string>(ClientAccessRulesUsernamePatternProperty.GetDisplayValue)).ToArray<string>();
				}
			}
			if (condition is AnyOfAuthenticationTypesPredicate)
			{
				AnyOfAuthenticationTypesPredicate anyOfAuthenticationTypesPredicate = condition as AnyOfAuthenticationTypesPredicate;
				if (wasNegated)
				{
					rule.ExceptAnyOfAuthenticationTypes = anyOfAuthenticationTypesPredicate.AuthenticationTypeList.ToArray<ClientAccessAuthenticationMethod>();
				}
				else
				{
					rule.AnyOfAuthenticationTypes = anyOfAuthenticationTypesPredicate.AuthenticationTypeList.ToArray<ClientAccessAuthenticationMethod>();
				}
			}
			if (condition is UserRecipientFilterPredicate)
			{
				UserRecipientFilterPredicate userRecipientFilterPredicate = condition as UserRecipientFilterPredicate;
				if (!wasNegated)
				{
					rule.UserRecipientFilter = userRecipientFilterPredicate.UserRecipientFilter;
				}
			}
		}

		private static readonly Version ClientAccessRuleBaseVersion = new Version("15.00.0008.00");

		private int priority;
	}
}
