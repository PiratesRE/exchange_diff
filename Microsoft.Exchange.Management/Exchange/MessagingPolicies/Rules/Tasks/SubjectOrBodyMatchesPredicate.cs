using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class SubjectOrBodyMatchesPredicate : MatchesPatternsPredicate, IEquatable<SubjectOrBodyMatchesPredicate>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Pattern>(this.Patterns);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as SubjectOrBodyMatchesPredicate)));
		}

		public bool Equals(SubjectOrBodyMatchesPredicate other)
		{
			if (this.Patterns == null)
			{
				return null == other.Patterns;
			}
			return this.Patterns.SequenceEqual(other.Patterns);
		}

		[LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[ConditionParameterName("SubjectOrBodyMatchesPatterns")]
		[ExceptionParameterName("ExceptIfSubjectOrBodyMatchesPatterns")]
		public override Pattern[] Patterns
		{
			get
			{
				return this.patterns;
			}
			set
			{
				this.patterns = value;
			}
		}

		internal override MatchesPatternsPredicate.LocalizedStringDescriptionDelegate LocalizedStringDescription
		{
			get
			{
				return new MatchesPatternsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionSubjectOrBodyMatches);
			}
		}

		public SubjectOrBodyMatchesPredicate() : this(true)
		{
		}

		public SubjectOrBodyMatchesPredicate(bool useLegacyRegex)
		{
			base.UseLegacyRegex = useLegacyRegex;
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
			if (!predicateCondition.Property.Name.Equals("Message.Subject") || !predicateCondition2.Property.Name.Equals("Message.Body"))
			{
				return null;
			}
			bool useLegacyRegex;
			if (predicateCondition.Name.Equals("matches") && predicateCondition2.Name.Equals("matches"))
			{
				useLegacyRegex = true;
			}
			else
			{
				if (!predicateCondition.Name.Equals("matchesRegex") || !predicateCondition2.Name.Equals("matchesRegex"))
				{
					return null;
				}
				useLegacyRegex = false;
			}
			if (predicateCondition.Value.RawValues.Count != predicateCondition2.Value.RawValues.Count)
			{
				return null;
			}
			Pattern[] array = new Pattern[predicateCondition.Value.RawValues.Count];
			for (int i = 0; i < predicateCondition.Value.RawValues.Count; i++)
			{
				if (predicateCondition.Value.RawValues[i] != predicateCondition2.Value.RawValues[i])
				{
					return null;
				}
				try
				{
					array[i] = new Pattern(predicateCondition.Value.RawValues[i], useLegacyRegex, false);
				}
				catch (ArgumentException)
				{
					return null;
				}
			}
			return new SubjectOrBodyMatchesPredicate(useLegacyRegex)
			{
				Patterns = array
			};
		}

		internal override Condition ToInternalCondition()
		{
			ShortList<string> shortList = new ShortList<string>();
			foreach (Pattern pattern in this.Patterns)
			{
				shortList.Add(pattern.ToString());
			}
			string name = base.UseLegacyRegex ? "matches" : "matchesRegex";
			PredicateCondition item = TransportRuleParser.Instance.CreatePredicate(name, TransportRuleParser.Instance.CreateProperty("Message.Subject"), shortList);
			PredicateCondition item2 = TransportRuleParser.Instance.CreatePredicate(name, TransportRuleParser.Instance.CreateProperty("Message.Body"), shortList);
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
