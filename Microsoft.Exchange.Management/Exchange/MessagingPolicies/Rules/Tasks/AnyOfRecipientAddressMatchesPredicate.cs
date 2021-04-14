using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class AnyOfRecipientAddressMatchesPredicate : SinglePropertyMatchesPredicate, IEquatable<AnyOfRecipientAddressMatchesPredicate>
	{
		public AnyOfRecipientAddressMatchesPredicate() : this(true)
		{
		}

		public AnyOfRecipientAddressMatchesPredicate(bool useLegacyRegex) : base("Message.EnvelopeRecipients", useLegacyRegex)
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Pattern>(this.Patterns);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AnyOfRecipientAddressMatchesPredicate)));
		}

		public bool Equals(AnyOfRecipientAddressMatchesPredicate other)
		{
			if (this.Patterns == null)
			{
				return null == other.Patterns;
			}
			return this.Patterns.SequenceEqual(other.Patterns);
		}

		[ExceptionParameterName("ExceptIfAnyOfRecipientAddressMatchesPatterns")]
		[LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[ConditionParameterName("AnyOfRecipientAddressMatchesPatterns")]
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
				return new MatchesPatternsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionAnyOfRecipientAddressMatches);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SinglePropertyMatchesPredicate.CreateFromInternalCondition<AnyOfRecipientAddressMatchesPredicate>(condition, "Message.EnvelopeRecipients");
		}

		private const string InternalPropertyName = "Message.EnvelopeRecipients";
	}
}
