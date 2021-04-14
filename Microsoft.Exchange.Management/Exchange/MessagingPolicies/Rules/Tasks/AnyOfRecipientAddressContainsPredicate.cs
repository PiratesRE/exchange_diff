using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[ExceptionParameterName("ExceptIfAnyOfRecipientAddressContainsWords")]
	[ConditionParameterName("AnyOfRecipientAddressContainsWords")]
	[Serializable]
	public class AnyOfRecipientAddressContainsPredicate : SinglePropertyContainsPredicate, IEquatable<AnyOfRecipientAddressContainsPredicate>
	{
		public AnyOfRecipientAddressContainsPredicate() : base("Message.EnvelopeRecipients")
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Word>(this.Words);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AnyOfRecipientAddressContainsPredicate)));
		}

		public bool Equals(AnyOfRecipientAddressContainsPredicate other)
		{
			if (this.Words == null)
			{
				return null == other.Words;
			}
			return this.Words.SequenceEqual(other.Words);
		}

		[LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[ExceptionParameterName("ExceptIfAnyOfRecipientAddressContainsWords")]
		[LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[ConditionParameterName("AnyOfRecipientAddressContainsWords")]
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
				return new ContainsWordsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionAnyOfRecipientAddressContains);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SinglePropertyContainsPredicate.CreateFromInternalCondition<AnyOfRecipientAddressContainsPredicate>(condition, "Message.EnvelopeRecipients");
		}

		private const string InternalPropertyName = "Message.EnvelopeRecipients";
	}
}
