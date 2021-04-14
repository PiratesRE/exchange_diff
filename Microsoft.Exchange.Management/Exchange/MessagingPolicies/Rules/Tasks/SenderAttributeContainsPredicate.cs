using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class SenderAttributeContainsPredicate : ContainsWordsPredicate, IEquatable<SenderAttributeContainsPredicate>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Word>(this.Words);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as SenderAttributeContainsPredicate)));
		}

		public bool Equals(SenderAttributeContainsPredicate other)
		{
			if (this.Words == null)
			{
				return null == other.Words;
			}
			return this.Words.SequenceEqual(other.Words);
		}

		[ExceptionParameterName("ExceptIfSenderADAttributeContainsWords")]
		[LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[ConditionParameterName("SenderADAttributeContainsWords")]
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
				return new ContainsWordsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionSenderAttributeContains);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if (!predicateCondition.Name.Equals("senderAttributeContains"))
			{
				return null;
			}
			Word[] array = new Word[predicateCondition.Value.RawValues.Count];
			for (int i = 0; i < predicateCondition.Value.RawValues.Count; i++)
			{
				try
				{
					array[i] = new Word(predicateCondition.Value.RawValues[i]);
				}
				catch (ArgumentOutOfRangeException)
				{
					return null;
				}
			}
			return new SenderAttributeContainsPredicate
			{
				Words = array
			};
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.Words == null || this.Words.Length == 0)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			Word[] words = this.Words;
			int i = 0;
			while (i < words.Length)
			{
				Word word = words[i];
				string value = word.Value;
				int index;
				if (!string.IsNullOrEmpty(value) && !Utils.CheckIsUnicodeStringWellFormed(value, out index))
				{
					errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.CommentsHaveInvalidChars((int)value[index]), base.Name));
				}
				else
				{
					int num = value.IndexOf(':');
					if (num >= 0)
					{
						string text = value.Substring(0, num).Trim().ToLowerInvariant();
						if (TransportUtils.GetDisclaimerMacroLookupTable().ContainsKey(text))
						{
							i++;
							continue;
						}
						errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.InvalidMacroName(text), base.Name));
					}
					else
					{
						errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.MacroNameNotSpecified(value), base.Name));
					}
				}
				return;
			}
			base.ValidateRead(errors);
		}

		internal override Condition ToInternalCondition()
		{
			ShortList<string> shortList = new ShortList<string>();
			foreach (Word word in this.words)
			{
				shortList.Add(word.ToString());
			}
			return TransportRuleParser.Instance.CreatePredicate("senderAttributeContains", null, shortList);
		}

		internal override void SuppressPiiData()
		{
			this.Words = Utils.RedactNameValuePairWords(this.Words);
		}
	}
}
