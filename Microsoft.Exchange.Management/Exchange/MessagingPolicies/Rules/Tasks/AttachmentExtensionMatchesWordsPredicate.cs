using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class AttachmentExtensionMatchesWordsPredicate : TransportRulePredicate, IEquatable<AttachmentExtensionMatchesWordsPredicate>
	{
		public AttachmentExtensionMatchesWordsPredicate()
		{
		}

		public AttachmentExtensionMatchesWordsPredicate(Word[] words)
		{
			this.Words = words;
		}

		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Word>(this.Words);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AttachmentExtensionMatchesWordsPredicate)));
		}

		public bool Equals(AttachmentExtensionMatchesWordsPredicate other)
		{
			if (this.Words == null)
			{
				return null == other.Words;
			}
			return this.Words.SequenceEqual(other.Words);
		}

		[LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[ExceptionParameterName("ExceptIfAttachmentExtensionMatchesWords")]
		[LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[ConditionParameterName("AttachmentExtensionMatchesWords")]
		public Word[] Words
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

		internal override string Description
		{
			get
			{
				string text = RuleDescription.BuildDescriptionStringFromStringArray(Utils.BuildWordStringList(this.Words), RulesTasksStrings.RuleDescriptionOrDelimiter, base.MaxDescriptionListLength);
				return RulesTasksStrings.RuleDescriptionAttachmentExtensionMatchesWords(text);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if (!predicateCondition.Name.Equals("is") || !predicateCondition.Property.Name.Equals("Message.AttachmentExtensions"))
			{
				return null;
			}
			Word[] array = (from value in predicateCondition.Value.RawValues
			select new Word(value)).ToArray<Word>();
			return new AttachmentExtensionMatchesWordsPredicate(array);
		}

		internal override Condition ToInternalCondition()
		{
			ShortList<string> valueEntries = new ShortList<string>(from word in this.words
			select word.Value);
			return TransportRuleParser.Instance.CreatePredicate("is", TransportRuleParser.Instance.CreateProperty("Message.AttachmentExtensions"), valueEntries);
		}

		internal override void Reset()
		{
			this.Words = null;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.Words == null || this.Words.Length == 0)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			base.ValidateRead(errors);
		}

		internal override string GetPredicateParameters()
		{
			return string.Join(", ", (from w in this.Words
			select Utils.QuoteCmdletParameter(w.ToString())).ToArray<string>());
		}

		internal override void SuppressPiiData()
		{
			this.Words = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.AttachmentExtensionMatchesWords, this.Words);
		}

		private Word[] words;
	}
}
