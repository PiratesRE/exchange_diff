using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class RecipientAttributeContainsPredicate : BifurcationInfoContainsWordsPredicate, IEquatable<RecipientAttributeContainsPredicate>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Word>(this.Words);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as RecipientAttributeContainsPredicate)));
		}

		public bool Equals(RecipientAttributeContainsPredicate other)
		{
			if (this.Words == null)
			{
				return null == other.Words;
			}
			return this.Words.SequenceEqual(other.Words);
		}

		[LocDisplayName(RulesTasksStrings.IDs.WordsDisplayName)]
		[ExceptionParameterName("ExceptIfRecipientADAttributeContainsWords")]
		[LocDescription(RulesTasksStrings.IDs.WordsDescription)]
		[ConditionParameterName("RecipientADAttributeContainsWords")]
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

		internal override BifurcationInfoContainsWordsPredicate.LocalizedStringDescriptionDelegate LocalizedStringDescription
		{
			get
			{
				return new BifurcationInfoContainsWordsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionRecipientAttributeContains);
			}
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

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return null;
		}

		internal static TransportRulePredicate CreatePredicateFromBifInfo(RuleBifurcationInfo bifInfo)
		{
			if (bifInfo.ADAttributesForTextMatch.Count > 0 || bifInfo.ADAttributes.Count > 0 || bifInfo.Managers.Count > 0 || bifInfo.Recipients.Count > 0 || bifInfo.Lists.Count > 0 || bifInfo.FromRecipients.Count > 0 || bifInfo.FromLists.Count > 0 || bifInfo.Partners.Count > 0 || bifInfo.RecipientAddressContainsWords.Count > 0 || bifInfo.RecipientDomainIs.Count > 0 || bifInfo.RecipientMatchesPatterns.Count > 0 || bifInfo.RecipientAttributeContains.Count == 0 || bifInfo.RecipientAttributeMatches.Count > 0 || bifInfo.SenderInRecipientList.Count > 0 || bifInfo.RecipientInSenderList.Count > 0)
			{
				return null;
			}
			if (bifInfo.InternalRecipients || bifInfo.ExternalRecipients || bifInfo.ExternalPartnerRecipients || bifInfo.ExternalNonPartnerRecipients)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(bifInfo.ManagementRelationship))
			{
				return null;
			}
			RecipientAttributeContainsPredicate recipientAttributeContainsPredicate = new RecipientAttributeContainsPredicate();
			Word[] array = new Word[bifInfo.RecipientAttributeContains.Count];
			for (int i = 0; i < bifInfo.RecipientAttributeContains.Count; i++)
			{
				try
				{
					array[i] = new Word(bifInfo.RecipientAttributeContains[i]);
				}
				catch (ArgumentOutOfRangeException)
				{
					return null;
				}
			}
			recipientAttributeContainsPredicate.Words = array;
			return recipientAttributeContainsPredicate;
		}

		internal override RuleBifurcationInfo ToRuleBifurcationInfo(out RuleBifurcationInfo additionalBifurcationInfo)
		{
			additionalBifurcationInfo = null;
			RuleBifurcationInfo ruleBifurcationInfo = new RuleBifurcationInfo();
			foreach (Word word in this.words)
			{
				ruleBifurcationInfo.RecipientAttributeContains.Add(word.ToString());
				ruleBifurcationInfo.Patterns.Add(word.ToString());
			}
			return ruleBifurcationInfo;
		}

		internal override void SuppressPiiData()
		{
			this.Words = Utils.RedactNameValuePairWords(this.Words);
		}
	}
}
