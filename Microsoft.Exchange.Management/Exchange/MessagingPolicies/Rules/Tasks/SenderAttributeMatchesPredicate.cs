using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class SenderAttributeMatchesPredicate : MatchesPatternsPredicate, IEquatable<SenderAttributeMatchesPredicate>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Pattern>(this.Patterns);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as SenderAttributeMatchesPredicate)));
		}

		public bool Equals(SenderAttributeMatchesPredicate other)
		{
			if (this.Patterns == null)
			{
				return null == other.Patterns;
			}
			return this.Patterns.SequenceEqual(other.Patterns);
		}

		[ConditionParameterName("SenderADAttributeMatchesPatterns")]
		[LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[ExceptionParameterName("ExceptIfSenderADAttributeMatchesPatterns")]
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
				return new MatchesPatternsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionSenderAttributeMatches);
			}
		}

		public SenderAttributeMatchesPredicate() : this(true)
		{
		}

		public SenderAttributeMatchesPredicate(bool useLegacyRegex)
		{
			base.UseLegacyRegex = useLegacyRegex;
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if (!predicateCondition.Name.Equals("senderAttributeMatches") && !predicateCondition.Name.Equals("senderAttributeMatchesRegex"))
			{
				return null;
			}
			bool useLegacyRegex = !predicateCondition.Name.Equals("senderAttributeMatchesRegex");
			Pattern[] array = new Pattern[predicateCondition.Value.RawValues.Count];
			for (int i = 0; i < predicateCondition.Value.RawValues.Count; i++)
			{
				try
				{
					array[i] = new Pattern(predicateCondition.Value.RawValues[i], useLegacyRegex, false);
				}
				catch (ArgumentException)
				{
					return null;
				}
			}
			return new SenderAttributeMatchesPredicate(useLegacyRegex)
			{
				Patterns = array
			};
		}

		internal override void Reset()
		{
			this.patterns = null;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			IEnumerable<ValidationError> enumerable = PatternValidator.ValidateAdAttributePatterns(this.Patterns, base.Name, base.UseLegacyRegex);
			if (enumerable != null)
			{
				errors.AddRange(enumerable);
				return;
			}
			base.ValidateRead(errors);
		}

		internal override Condition ToInternalCondition()
		{
			ShortList<string> shortList = new ShortList<string>();
			foreach (Pattern pattern in this.patterns)
			{
				shortList.Add(pattern.ToString());
			}
			string name = base.UseLegacyRegex ? "senderAttributeMatches" : "senderAttributeMatchesRegex";
			return TransportRuleParser.Instance.CreatePredicate(name, null, shortList);
		}

		internal override void SuppressPiiData()
		{
			this.Patterns = Utils.RedactNameValuePairPatterns(this.Patterns);
		}
	}
}
