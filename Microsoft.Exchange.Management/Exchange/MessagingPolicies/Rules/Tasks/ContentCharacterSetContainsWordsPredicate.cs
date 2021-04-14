using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class ContentCharacterSetContainsWordsPredicate : SinglePropertyContainsPredicate, IEquatable<ContentCharacterSetContainsWordsPredicate>
	{
		public ContentCharacterSetContainsWordsPredicate() : base("Message.ContentCharacterSets")
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Word>(this.Words);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as ContentCharacterSetContainsWordsPredicate)));
		}

		public bool Equals(ContentCharacterSetContainsWordsPredicate other)
		{
			if (this.Words == null)
			{
				return null == other.Words;
			}
			return this.Words.SequenceEqual(other.Words);
		}

		[LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[ConditionParameterName("ContentCharacterSetContainsWords")]
		[ExceptionParameterName("ExceptIfContentCharacterSetContainsWords")]
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
				return new ContainsWordsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionContentCharacterSetContainsWords);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SinglePropertyContainsPredicate.CreateFromInternalCondition<ContentCharacterSetContainsWordsPredicate>(condition, "Message.ContentCharacterSets");
		}

		private const string InternalPropertyName = "Message.ContentCharacterSets";
	}
}
