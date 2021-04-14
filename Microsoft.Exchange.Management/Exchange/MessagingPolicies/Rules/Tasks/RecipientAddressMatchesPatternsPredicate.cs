using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class RecipientAddressMatchesPatternsPredicate : BifurcationInfoMatchesPatternsPredicate, IEquatable<RecipientAddressMatchesPatternsPredicate>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Pattern>(this.Patterns);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as RecipientAddressMatchesPatternsPredicate)));
		}

		public bool Equals(RecipientAddressMatchesPatternsPredicate other)
		{
			if (this.Patterns == null)
			{
				return null == other.Patterns;
			}
			return this.Patterns.SequenceEqual(other.Patterns);
		}

		[LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[ConditionParameterName("RecipientAddressMatchesPatterns")]
		[ExceptionParameterName("ExceptIfRecipientAddressMatchesPatterns")]
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

		internal override BifurcationInfoMatchesPatternsPredicate.LocalizedStringDescriptionDelegate LocalizedStringDescription
		{
			get
			{
				return new BifurcationInfoMatchesPatternsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionRecipientAddressMatches);
			}
		}

		public RecipientAddressMatchesPatternsPredicate() : this(false)
		{
		}

		public RecipientAddressMatchesPatternsPredicate(bool useLegacyRegex)
		{
			base.UseLegacyRegex = useLegacyRegex;
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return null;
		}

		internal static TransportRulePredicate CreatePredicateFromBifInfo(RuleBifurcationInfo bifInfo)
		{
			if (bifInfo.ADAttributesForTextMatch.Count > 0 || bifInfo.ADAttributes.Count > 0 || bifInfo.Managers.Count > 0 || bifInfo.Recipients.Count > 0 || bifInfo.Lists.Count > 0 || bifInfo.FromRecipients.Count > 0 || bifInfo.FromLists.Count > 0 || bifInfo.Partners.Count > 0 || bifInfo.RecipientAddressContainsWords.Count > 0 || bifInfo.RecipientDomainIs.Count > 0 || (bifInfo.RecipientMatchesPatterns.Count == 0 && bifInfo.RecipientMatchesRegexPatterns.Count == 0) || bifInfo.RecipientAttributeContains.Count > 0 || bifInfo.RecipientAttributeMatches.Count > 0 || bifInfo.SenderInRecipientList.Count > 0 || bifInfo.RecipientInSenderList.Count > 0)
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
			bool flag = bifInfo.RecipientMatchesPatterns.Count > 0;
			RecipientAddressMatchesPatternsPredicate recipientAddressMatchesPatternsPredicate = new RecipientAddressMatchesPatternsPredicate(flag);
			List<string> list = flag ? bifInfo.RecipientMatchesPatterns : bifInfo.RecipientMatchesRegexPatterns;
			Pattern[] array = new Pattern[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				try
				{
					array[i] = new Pattern(list[i], flag, false);
				}
				catch (ArgumentException)
				{
					return null;
				}
			}
			recipientAddressMatchesPatternsPredicate.Patterns = array;
			return recipientAddressMatchesPatternsPredicate;
		}

		internal override RuleBifurcationInfo ToRuleBifurcationInfo(out RuleBifurcationInfo additionalBifurcationInfo)
		{
			additionalBifurcationInfo = null;
			RuleBifurcationInfo ruleBifurcationInfo = new RuleBifurcationInfo();
			foreach (Pattern pattern in this.Patterns)
			{
				if (base.UseLegacyRegex)
				{
					ruleBifurcationInfo.RecipientMatchesPatterns.Add(pattern.ToString());
				}
				else
				{
					ruleBifurcationInfo.RecipientMatchesRegexPatterns.Add(pattern.ToString());
				}
				ruleBifurcationInfo.Patterns.Add(pattern.ToString());
			}
			return ruleBifurcationInfo;
		}
	}
}
