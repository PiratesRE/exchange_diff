using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class RecipientAttributeMatchesPredicate : BifurcationInfoMatchesPatternsPredicate, IEquatable<RecipientAttributeMatchesPredicate>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Pattern>(this.Patterns);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as RecipientAttributeMatchesPredicate)));
		}

		public bool Equals(RecipientAttributeMatchesPredicate other)
		{
			if (this.Patterns == null)
			{
				return null == other.Patterns;
			}
			return this.Patterns.SequenceEqual(other.Patterns);
		}

		[LocDescription(RulesTasksStrings.IDs.TextPatternsDescription)]
		[ExceptionParameterName("ExceptIfRecipientADAttributeMatchesPatterns")]
		[LocDisplayName(RulesTasksStrings.IDs.TextPatternsDisplayName)]
		[ConditionParameterName("RecipientADAttributeMatchesPatterns")]
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
				return new BifurcationInfoMatchesPatternsPredicate.LocalizedStringDescriptionDelegate(RulesTasksStrings.RuleDescriptionRecipientAttributeMatches);
			}
		}

		public RecipientAttributeMatchesPredicate() : this(false)
		{
		}

		internal RecipientAttributeMatchesPredicate(bool useLegacyRegex)
		{
			base.UseLegacyRegex = useLegacyRegex;
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			IEnumerable<ValidationError> enumerable = PatternValidator.ValidateAdAttributePatterns(this.Patterns, base.Name, base.UseLegacyRegex);
			if (enumerable != null)
			{
				errors.AddRange(enumerable);
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
			if (bifInfo.ADAttributesForTextMatch.Count > 0 || bifInfo.ADAttributes.Count > 0 || bifInfo.Managers.Count > 0 || bifInfo.Recipients.Count > 0 || bifInfo.Lists.Count > 0 || bifInfo.FromRecipients.Count > 0 || bifInfo.FromLists.Count > 0 || bifInfo.Partners.Count > 0 || bifInfo.RecipientAddressContainsWords.Count > 0 || bifInfo.RecipientDomainIs.Count > 0 || bifInfo.RecipientMatchesPatterns.Count > 0 || bifInfo.RecipientAttributeContains.Count > 0 || (bifInfo.RecipientAttributeMatches.Count == 0 && bifInfo.RecipientAttributeMatchesRegex.Count == 0) || bifInfo.SenderInRecipientList.Count > 0 || bifInfo.RecipientInSenderList.Count > 0)
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
			bool flag = bifInfo.RecipientAttributeMatches.Count > 0;
			RecipientAttributeMatchesPredicate recipientAttributeMatchesPredicate = new RecipientAttributeMatchesPredicate(flag);
			List<string> list = flag ? bifInfo.RecipientAttributeMatches : bifInfo.RecipientAttributeMatchesRegex;
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
			recipientAttributeMatchesPredicate.patterns = array;
			return recipientAttributeMatchesPredicate;
		}

		internal override RuleBifurcationInfo ToRuleBifurcationInfo(out RuleBifurcationInfo additionalBifurcationInfo)
		{
			additionalBifurcationInfo = null;
			RuleBifurcationInfo ruleBifurcationInfo = new RuleBifurcationInfo();
			foreach (Pattern pattern in this.patterns)
			{
				if (base.UseLegacyRegex)
				{
					ruleBifurcationInfo.RecipientAttributeMatches.Add(pattern.ToString());
				}
				else
				{
					ruleBifurcationInfo.RecipientAttributeMatchesRegex.Add(pattern.ToString());
				}
				ruleBifurcationInfo.Patterns.Add(pattern.ToString());
			}
			return ruleBifurcationInfo;
		}

		internal override void SuppressPiiData()
		{
			this.Patterns = Utils.RedactNameValuePairPatterns(this.Patterns);
		}
	}
}
