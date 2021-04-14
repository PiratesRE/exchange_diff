using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class AttachmentPropertyContainsWordsPredicate : ContainsWordsPredicate, IEquatable<AttachmentPropertyContainsWordsPredicate>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Word>(this.Words);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as AttachmentPropertyContainsWordsPredicate)));
		}

		public bool Equals(AttachmentPropertyContainsWordsPredicate other)
		{
			if (this.Words == null)
			{
				return null == other.Words;
			}
			return this.Words.SequenceEqual(other.Words);
		}

		[LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[ConditionParameterName("AttachmentPropertyContainsWords")]
		[ExceptionParameterName("ExceptIfAttachmentPropertyContainsWords")]
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
				return new ContainsWordsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionAttachmentPropertyContainsWords);
			}
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.Words == null || this.Words.Length == 0)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			List<string> list = (from w in this.Words
			select w.ToString()).ToList<string>();
			List<KeyValuePair<string, List<string>>> source = AttachmentPropertyContainsPredicate.ParsePredicateParameters(list);
			if (!source.Any<KeyValuePair<string, List<string>>>())
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.AttachmentMetadataPropertyNotSpecified(string.Join(", ", list)), base.Name));
				return;
			}
			Word[] words = this.Words;
			int i = 0;
			while (i < words.Length)
			{
				Word word = words[i];
				string value = word.Value;
				string[] array = value.Split(new char[]
				{
					':'
				});
				if (array.Length < 2 || (array.Length >= 2 && (string.IsNullOrWhiteSpace(array[0]) || string.IsNullOrWhiteSpace(array[1]))))
				{
					errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.AttachmentMetadataPropertyNotSpecified(value), base.Name));
				}
				else
				{
					string[] source2 = array[1].Trim().Split(new char[]
					{
						','
					});
					if (!source2.Any(new Func<string, bool>(string.IsNullOrWhiteSpace)))
					{
						i++;
						continue;
					}
					errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.AttachmentMetadataParameterContainsEmptyWords(value), base.Name));
				}
				return;
			}
			base.ValidateRead(errors);
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if (!predicateCondition.Name.Equals("attachmentPropertyContains"))
			{
				return null;
			}
			AttachmentPropertyContainsWordsPredicate attachmentPropertyContainsWordsPredicate = new AttachmentPropertyContainsWordsPredicate();
			attachmentPropertyContainsWordsPredicate.Words = (from w in predicateCondition.Value.RawValues
			select new Word(w)).ToArray<Word>();
			return attachmentPropertyContainsWordsPredicate;
		}

		internal override Condition ToInternalCondition()
		{
			ShortList<string> shortList = new ShortList<string>();
			foreach (Word word in this.words)
			{
				shortList.Add(word.ToString());
			}
			return TransportRuleParser.Instance.CreatePredicate("attachmentPropertyContains", null, shortList);
		}
	}
}
