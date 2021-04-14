using System;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class SclOverPredicate : TransportRulePredicate, IEquatable<SclOverPredicate>
	{
		public override int GetHashCode()
		{
			return this.SclValue.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as SclOverPredicate)));
		}

		public bool Equals(SclOverPredicate other)
		{
			return this.SclValue.Equals(other.SclValue);
		}

		[LocDescription(RulesTasksStrings.IDs.SclValueDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.SclValueDisplayName)]
		[ConditionParameterName("SCLOver")]
		[ExceptionParameterName("ExceptIfSCLOver")]
		public SclValue SclValue
		{
			get
			{
				return this.sclValue;
			}
			set
			{
				this.sclValue = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionSclOver(this.SclValue.ToString());
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			if (condition.ConditionType != ConditionType.And)
			{
				return null;
			}
			AndCondition andCondition = (AndCondition)condition;
			if (andCondition.SubConditions.Count != 2 || andCondition.SubConditions[0].ConditionType != ConditionType.Predicate || andCondition.SubConditions[1].ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)andCondition.SubConditions[0];
			PredicateCondition predicateCondition2 = (PredicateCondition)andCondition.SubConditions[1];
			if (!predicateCondition.Name.Equals("exists") || !predicateCondition.Property.Name.Equals("Message.SclValue"))
			{
				return null;
			}
			if (!predicateCondition2.Name.Equals("greaterThanOrEqual") || !predicateCondition2.Property.Name.Equals("Message.SclValue") || predicateCondition2.Value.RawValues.Count != 1)
			{
				return null;
			}
			int input;
			if (!int.TryParse(predicateCondition2.Value.RawValues[0], out input))
			{
				return null;
			}
			SclOverPredicate sclOverPredicate = new SclOverPredicate();
			try
			{
				sclOverPredicate.SclValue = new SclValue(input);
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
			return sclOverPredicate;
		}

		internal override void Reset()
		{
			this.sclValue = new SclValue(0);
			base.Reset();
		}

		internal override Condition ToInternalCondition()
		{
			PredicateCondition item = TransportRuleParser.Instance.CreatePredicate("exists", TransportRuleParser.Instance.CreateProperty("Message.SclValue"), new ShortList<string>());
			PredicateCondition item2 = TransportRuleParser.Instance.CreatePredicate("greaterThanOrEqual", TransportRuleParser.Instance.CreateProperty("Message.SclValue"), new ShortList<string>
			{
				this.SclValue.ToString()
			});
			return new AndCondition
			{
				SubConditions = 
				{
					item,
					item2
				}
			};
		}

		internal override string GetPredicateParameters()
		{
			return this.SclValue.ToString();
		}

		private SclValue sclValue;
	}
}
