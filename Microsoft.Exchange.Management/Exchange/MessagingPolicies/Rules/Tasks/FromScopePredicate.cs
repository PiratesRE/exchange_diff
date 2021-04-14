using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class FromScopePredicate : TransportRulePredicate, IEquatable<FromScopePredicate>
	{
		public override int GetHashCode()
		{
			return this.Scope.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as FromScopePredicate)));
		}

		public bool Equals(FromScopePredicate other)
		{
			return this.Scope.Equals(other.Scope);
		}

		[LocDescription(RulesTasksStrings.IDs.FromScopeDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.FromScopeDisplayName)]
		[ConditionParameterName("FromScope")]
		[ExceptionParameterName("ExceptIfFromScope")]
		public FromUserScope Scope
		{
			get
			{
				return this.scope;
			}
			set
			{
				this.scope = value;
			}
		}

		[LocDisplayName(RulesTasksStrings.IDs.SubTypeDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.SubTypeDescription)]
		public override IEnumerable<RuleSubType> RuleSubTypes
		{
			get
			{
				return new RuleSubType[]
				{
					RuleSubType.None,
					RuleSubType.Dlp
				};
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			bool flag = false;
			if (condition.ConditionType == ConditionType.Not)
			{
				condition = ((NotCondition)condition).SubCondition;
				flag = true;
			}
			if (condition.ConditionType != ConditionType.And)
			{
				return null;
			}
			AndCondition andCondition = (AndCondition)condition;
			if (andCondition.SubConditions.Count != 2 || andCondition.SubConditions[0].ConditionType != ConditionType.Not || andCondition.SubConditions[1].ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			NotCondition notCondition = (NotCondition)andCondition.SubConditions[0];
			if (notCondition.SubCondition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)notCondition.SubCondition;
			PredicateCondition predicateCondition2 = (PredicateCondition)andCondition.SubConditions[1];
			if (!predicateCondition.Name.Equals("is") || !predicateCondition.Property.Name.Equals("Message.Auth") || predicateCondition.Value.RawValues.Count != 1 || !predicateCondition.Value.RawValues[0].Equals("<>"))
			{
				return null;
			}
			if (!predicateCondition2.Name.Equals("isInternal") || !predicateCondition2.Property.Name.Equals("Message.From") || predicateCondition2.Value.RawValues.Count != 0)
			{
				return null;
			}
			FromScopePredicate fromScopePredicate = new FromScopePredicate();
			if (flag)
			{
				fromScopePredicate.Scope = FromUserScope.NotInOrganization;
			}
			else
			{
				fromScopePredicate.Scope = FromUserScope.InOrganization;
			}
			return fromScopePredicate;
		}

		internal override void Reset()
		{
			this.scope = FromUserScope.InOrganization;
			base.Reset();
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionFromScope(LocalizedDescriptionAttribute.FromEnum(typeof(FromUserScope), this.Scope));
			}
		}

		internal override Condition ToInternalCondition()
		{
			ShortList<string> valueEntries = new ShortList<string>
			{
				"<>"
			};
			PredicateCondition subCondition = TransportRuleParser.Instance.CreatePredicate("is", TransportRuleParser.Instance.CreateProperty("Message.Auth"), valueEntries);
			valueEntries = new ShortList<string>();
			NotCondition item = new NotCondition(subCondition);
			PredicateCondition item2 = TransportRuleParser.Instance.CreatePredicate("isInternal", TransportRuleParser.Instance.CreateProperty("Message.From"), valueEntries);
			AndCondition andCondition = new AndCondition();
			andCondition.SubConditions.Add(item);
			andCondition.SubConditions.Add(item2);
			if (this.Scope == FromUserScope.NotInOrganization)
			{
				return new NotCondition(andCondition);
			}
			return andCondition;
		}

		internal override string GetPredicateParameters()
		{
			return Enum.GetName(typeof(FromUserScope), this.Scope);
		}

		private FromUserScope scope;
	}
}
