using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class AttachmentNameMatchesPredicate : SinglePropertyMatchesPredicate, IEquatable<AttachmentNameMatchesPredicate>
	{
		public AttachmentNameMatchesPredicate() : this(true)
		{
		}

		public AttachmentNameMatchesPredicate(bool useLegacyRegex) : base("Message.AttachmentNames", useLegacyRegex)
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Pattern>(this.Patterns);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AttachmentNameMatchesPredicate)));
		}

		public bool Equals(AttachmentNameMatchesPredicate other)
		{
			if (this.Patterns == null)
			{
				return null == other.Patterns;
			}
			return this.Patterns.SequenceEqual(other.Patterns);
		}

		[ExceptionParameterName("ExceptIfAttachmentNameMatchesPatterns")]
		[LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[ConditionParameterName("AttachmentNameMatchesPatterns")]
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
				return new MatchesPatternsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionAttachmentNameMatches);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SinglePropertyMatchesPredicate.CreateFromInternalCondition<AttachmentNameMatchesPredicate>(condition, "Message.AttachmentNames");
		}

		private const string InternalPropertyName = "Message.AttachmentNames";
	}
}
