using System;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class SubjectContainsPredicate : SinglePropertyContainsPredicate, IEquatable<SubjectContainsPredicate>
	{
		public SubjectContainsPredicate() : base("Message.Subject")
		{
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Word>(this.Words);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as SubjectContainsPredicate)));
		}

		public bool Equals(SubjectContainsPredicate other)
		{
			if (this.Words == null)
			{
				return null == other.Words;
			}
			return this.Words.SequenceEqual(other.Words);
		}

		[LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[ConditionParameterName("SubjectContainsWords")]
		[ExceptionParameterName("ExceptIfSubjectContainsWords")]
		[LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
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
				return new ContainsWordsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionSubjectContains);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return SinglePropertyContainsPredicate.CreateFromInternalCondition<SubjectContainsPredicate>(condition, "Message.Subject");
		}

		private const string InternalPropertyName = "Message.Subject";
	}
}
