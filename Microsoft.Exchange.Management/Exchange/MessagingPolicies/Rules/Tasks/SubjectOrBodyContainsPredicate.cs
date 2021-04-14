using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class SubjectOrBodyContainsPredicate : ContainsWordsPredicate, IEquatable<SubjectOrBodyContainsPredicate>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Word>(this.Words);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as SubjectOrBodyContainsPredicate)));
		}

		public bool Equals(SubjectOrBodyContainsPredicate other)
		{
			if (this.Words == null)
			{
				return null == other.Words;
			}
			return this.Words.SequenceEqual(other.Words);
		}

		[LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[ExceptionParameterName("ExceptIfSubjectOrBodyContainsWords")]
		[ConditionParameterName("SubjectOrBodyContainsWords")]
		[LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		public override Word[] Words
		{
			get
			{
				return this.words;
			}
			set
			{
				this.words = value;
			}
		}

		protected override ContainsWordsPredicate.LocalizedStringDescriptionDelegate LocalizedStringDescription
		{
			get
			{
				return new ContainsWordsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionSubjectOrBodyContains);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			if (condition.ConditionType != ConditionType.Or)
			{
				return null;
			}
			OrCondition orCondition = (OrCondition)condition;
			if (orCondition.SubConditions.Count != 2)
			{
				return null;
			}
			if (orCondition.SubConditions[0].ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			if (orCondition.SubConditions[1].ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)orCondition.SubConditions[0];
			PredicateCondition predicateCondition2 = (PredicateCondition)orCondition.SubConditions[1];
			if (!predicateCondition.Name.Equals("contains") || !predicateCondition2.Name.Equals("contains") || !predicateCondition.Property.Name.Equals("Message.Subject") || !predicateCondition2.Property.Name.Equals("Message.Body"))
			{
				return null;
			}
			if (predicateCondition.Value.RawValues.Count != predicateCondition2.Value.RawValues.Count)
			{
				return null;
			}
			Word[] array = new Word[predicateCondition.Value.RawValues.Count];
			for (int i = 0; i < predicateCondition.Value.RawValues.Count; i++)
			{
				if (predicateCondition.Value.RawValues[i] != predicateCondition2.Value.RawValues[i])
				{
					return null;
				}
				try
				{
					array[i] = new Word(predicateCondition.Value.RawValues[i]);
				}
				catch (ArgumentOutOfRangeException)
				{
					return null;
				}
			}
			return new SubjectOrBodyContainsPredicate
			{
				Words = array
			};
		}

		internal override Condition ToInternalCondition()
		{
			ShortList<string> shortList = new ShortList<string>();
			foreach (Word word in this.Words)
			{
				shortList.Add(word.ToString());
			}
			PredicateCondition item = TransportRuleParser.Instance.CreatePredicate("contains", TransportRuleParser.Instance.CreateProperty("Message.Subject"), shortList);
			PredicateCondition item2 = TransportRuleParser.Instance.CreatePredicate("contains", TransportRuleParser.Instance.CreateProperty("Message.Body"), shortList);
			return new OrCondition
			{
				SubConditions = 
				{
					item,
					item2
				}
			};
		}
	}
}
