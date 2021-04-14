using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class AttachmentMatchesPatternsPredicate : MatchesPatternsPredicate, IEquatable<AttachmentMatchesPatternsPredicate>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Pattern>(this.Patterns);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AttachmentMatchesPatternsPredicate)));
		}

		public bool Equals(AttachmentMatchesPatternsPredicate other)
		{
			if (this.Patterns == null)
			{
				return null == other.Patterns;
			}
			return this.Patterns.SequenceEqual(other.Patterns);
		}

		[ConditionParameterName("AttachmentMatchesPatterns")]
		[ExceptionParameterName("ExceptIfAttachmentMatchesPatterns")]
		[LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
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
				return new MatchesPatternsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionAttachmentMatchesPatterns);
			}
		}

		public AttachmentMatchesPatternsPredicate() : this(true)
		{
		}

		public AttachmentMatchesPatternsPredicate(bool useLegacyRegex)
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
			if (!predicateCondition.Name.Equals("attachmentMatchesPatterns") && !predicateCondition.Name.Equals("attachmentMatchesRegexPatterns"))
			{
				return null;
			}
			bool useLegacyRegex = !predicateCondition.Name.Equals("attachmentMatchesRegexPatterns");
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
			return new AttachmentMatchesPatternsPredicate(useLegacyRegex)
			{
				Patterns = array
			};
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			IEnumerable<ValidationError> enumerable = PatternValidator.ValidatePatterns(this.Patterns, base.Name, false);
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
			foreach (Pattern pattern in this.Patterns)
			{
				shortList.Add(pattern.ToString());
			}
			string name = base.UseLegacyRegex ? "attachmentMatchesPatterns" : "attachmentMatchesRegexPatterns";
			return TransportRuleParser.Instance.CreatePredicate(name, null, shortList);
		}
	}
}
