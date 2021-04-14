using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class RecipientInSenderListPredicate : BifurcationInfoPredicate, IEquatable<RecipientInSenderListPredicate>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Word>(this.Lists);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as RecipientInSenderListPredicate)));
		}

		public bool Equals(RecipientInSenderListPredicate other)
		{
			if (this.Lists == null)
			{
				return null == other.Lists;
			}
			return this.Lists.SequenceEqual(other.Lists);
		}

		[ExceptionParameterName("ExceptIfRecipientInSenderList")]
		[LocDisplayName(RulesTasksStrings.IDs.ListsDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.ListsDescription)]
		[ConditionParameterName("RecipientInSenderList")]
		public Word[] Lists
		{
			get
			{
				return this.lists;
			}
			set
			{
				this.lists = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionRecipientInSenderList(RuleDescription.BuildDescriptionStringFromStringArray(Utils.BuildWordStringList(this.Lists), RulesTasksStrings.RuleDescriptionOrDelimiter, base.MaxDescriptionListLength));
			}
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.Lists == null || this.Lists.Length == 0)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
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
			if (bifInfo.ADAttributesForTextMatch.Count > 0 || bifInfo.ADAttributes.Count > 0 || bifInfo.Managers.Count > 0 || bifInfo.Recipients.Count > 0 || bifInfo.Lists.Count > 0 || bifInfo.FromRecipients.Count > 0 || bifInfo.FromLists.Count > 0 || bifInfo.Partners.Count > 0 || bifInfo.RecipientAddressContainsWords.Count > 0 || bifInfo.RecipientDomainIs.Count > 0 || bifInfo.RecipientMatchesPatterns.Count > 0 || bifInfo.RecipientAttributeContains.Count > 0 || bifInfo.RecipientAttributeMatches.Count > 0 || bifInfo.SenderInRecipientList.Count > 0 || bifInfo.RecipientInSenderList.Count == 0)
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
			RecipientInSenderListPredicate recipientInSenderListPredicate = new RecipientInSenderListPredicate();
			Word[] array = new Word[bifInfo.RecipientInSenderList.Count];
			for (int i = 0; i < bifInfo.RecipientInSenderList.Count; i++)
			{
				array[i] = new Word(bifInfo.RecipientInSenderList[i]);
			}
			recipientInSenderListPredicate.Lists = array;
			return recipientInSenderListPredicate;
		}

		internal override RuleBifurcationInfo ToRuleBifurcationInfo(out RuleBifurcationInfo additionalBifurcationInfo)
		{
			additionalBifurcationInfo = null;
			RuleBifurcationInfo ruleBifurcationInfo = new RuleBifurcationInfo();
			foreach (Word word in this.lists)
			{
				ruleBifurcationInfo.RecipientInSenderList.Add(word.ToString());
			}
			return ruleBifurcationInfo;
		}

		internal override string GetPredicateParameters()
		{
			return string.Join(", ", (from w in this.Lists
			select Utils.QuoteCmdletParameter(w.ToString())).ToArray<string>());
		}

		internal override void SuppressPiiData()
		{
			this.Lists = SuppressingPiiProperty.TryRedactValue<Word[]>(RuleSchema.SenderInRecipientList, this.Lists);
		}

		private Word[] lists;
	}
}
