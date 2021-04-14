using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class SubjectMatchesPredicate : SinglePropertyMatchesPredicate, IEquatable<SubjectMatchesPredicate>
	{
		public SubjectMatchesPredicate() : this(true)
		{
		}

		public SubjectMatchesPredicate(bool useLegacyRegex) : base("Message.Subject", useLegacyRegex)
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Pattern>(this.Patterns);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as SubjectMatchesPredicate)));
		}

		public bool Equals(SubjectMatchesPredicate other)
		{
			if (this.Patterns == null)
			{
				return null == other.Patterns;
			}
			return this.Patterns.SequenceEqual(other.Patterns);
		}

		[LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[ConditionParameterName("SubjectMatchesPatterns")]
		[ExceptionParameterName("ExceptIfSubjectMatchesPatterns")]
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
				return new MatchesPatternsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionSubjectMatches);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SinglePropertyMatchesPredicate.CreateFromInternalCondition<SubjectMatchesPredicate>(condition, "Message.Subject");
		}

		private const string InternalPropertyName = "Message.Subject";
	}
}
