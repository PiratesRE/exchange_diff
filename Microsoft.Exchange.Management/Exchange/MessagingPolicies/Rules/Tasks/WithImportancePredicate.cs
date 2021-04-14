using System;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class WithImportancePredicate : TransportRulePredicate, IEquatable<WithImportancePredicate>
	{
		public override int GetHashCode()
		{
			return this.Importance.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as WithImportancePredicate)));
		}

		public bool Equals(WithImportancePredicate other)
		{
			return this.Importance.Equals(other.Importance);
		}

		[Microsoft.Exchange.Core.RuleTasks.LocDescription(RulesTasksStrings.IDs.ImportanceDescription)]
		[ExceptionParameterName("ExceptIfWithImportance")]
		[ConditionParameterName("WithImportance")]
		[Microsoft.Exchange.Core.RuleTasks.LocDisplayName(RulesTasksStrings.IDs.ImportanceDisplayName)]
		public Importance Importance
		{
			get
			{
				return this.importance;
			}
			set
			{
				this.importance = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionWithImportance(LocalizedDescriptionAttribute.FromEnum(typeof(Importance), this.Importance));
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			WithImportancePredicate withImportancePredicate = new WithImportancePredicate();
			if (WithImportancePredicate.IsImportanceCondition(condition, Importance.High))
			{
				withImportancePredicate.Importance = Importance.High;
				return withImportancePredicate;
			}
			if (WithImportancePredicate.IsImportanceCondition(condition, Importance.Low))
			{
				withImportancePredicate.Importance = Importance.Low;
				return withImportancePredicate;
			}
			if (condition.ConditionType == ConditionType.Not)
			{
				NotCondition notCondition = (NotCondition)condition;
				if (notCondition.SubCondition.ConditionType == ConditionType.Or)
				{
					OrCondition orCondition = (OrCondition)notCondition.SubCondition;
					if (orCondition.SubConditions.Count == 2 && WithImportancePredicate.IsImportanceCondition(orCondition.SubConditions[0], Importance.High) && WithImportancePredicate.IsImportanceCondition(orCondition.SubConditions[1], Importance.Low))
					{
						withImportancePredicate.Importance = Importance.Normal;
						return withImportancePredicate;
					}
				}
			}
			return null;
		}

		internal override void Reset()
		{
			this.importance = Importance.Normal;
			base.Reset();
		}

		private static bool IsImportanceCondition(Condition condition, Importance importance)
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return false;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			return predicateCondition.Name.Equals("is") && predicateCondition.Property is HeaderProperty && predicateCondition.Property.Name.Equals("Importance") && predicateCondition.Value.RawValues.Count == 1 && predicateCondition.Value.RawValues[0].Equals(importance.ToString());
		}

		internal override Condition ToInternalCondition()
		{
			if (this.Importance != Importance.Normal)
			{
				return WithImportancePredicate.CreateImportancePredicate(this.Importance);
			}
			PredicateCondition item = WithImportancePredicate.CreateImportancePredicate(Importance.High);
			PredicateCondition item2 = WithImportancePredicate.CreateImportancePredicate(Importance.Low);
			return new NotCondition(new OrCondition
			{
				SubConditions = 
				{
					item,
					item2
				}
			});
		}

		private static PredicateCondition CreateImportancePredicate(Importance importance)
		{
			return TransportRuleParser.Instance.CreatePredicate("is", TransportRuleParser.Instance.CreateProperty("Message.Headers:Importance"), new ShortList<string>
			{
				importance.ToString()
			});
		}

		internal override string GetPredicateParameters()
		{
			return Enum.GetName(typeof(Importance), this.Importance);
		}

		private Importance importance;
	}
}
