using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class FromAddressMatchesPredicate : SinglePropertyMatchesPredicate, IEquatable<FromAddressMatchesPredicate>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Pattern>(this.Patterns);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as FromAddressMatchesPredicate)));
		}

		public bool Equals(FromAddressMatchesPredicate other)
		{
			if (this.Patterns == null)
			{
				return null == other.Patterns;
			}
			return this.Patterns.SequenceEqual(other.Patterns);
		}

		[LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[ExceptionParameterName("ExceptIfFromAddressMatchesPatterns")]
		[LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[ConditionParameterName("FromAddressMatchesPatterns")]
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
				return new MatchesPatternsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionFromAddressMatches);
			}
		}

		public FromAddressMatchesPredicate() : this(true)
		{
		}

		public FromAddressMatchesPredicate(bool useLegacyRegex) : base("Message.From", useLegacyRegex)
		{
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SinglePropertyMatchesPredicate.CreateFromInternalCondition<FromAddressMatchesPredicate>(condition, "Message.From");
		}

		private const string InternalPropertyName = "Message.From";
	}
}
