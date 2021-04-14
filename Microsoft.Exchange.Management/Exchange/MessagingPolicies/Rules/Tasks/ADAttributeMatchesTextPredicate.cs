using System;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class ADAttributeMatchesTextPredicate : BifurcationInfoPredicate, IEquatable<ADAttributeMatchesTextPredicate>
	{
		public override int GetHashCode()
		{
			return this.EvaluatedUser.GetHashCode() ^ (string.IsNullOrEmpty(this.AttributeValue) ? 0 : this.AttributeValue.GetHashCode()) ^ this.ADAttribute.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as ADAttributeMatchesTextPredicate)));
		}

		public bool Equals(ADAttributeMatchesTextPredicate other)
		{
			if (string.IsNullOrEmpty(this.AttributeValue))
			{
				return this.EvaluatedUser.Equals(other.EvaluatedUser) && string.IsNullOrEmpty(other.AttributeValue) && this.ADAttribute.Equals(other.ADAttribute);
			}
			return this.EvaluatedUser.Equals(other.EvaluatedUser) && this.AttributeValue.Equals(other.AttributeValue) && this.ADAttribute.Equals(other.ADAttribute);
		}

		[LocDescription(RulesTasksStrings.IDs.EvaluatedUserDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.EvaluatedUserDisplayName)]
		public EvaluatedUser EvaluatedUser
		{
			get
			{
				return this.evaluatedUser;
			}
			set
			{
				this.evaluatedUser = value;
			}
		}

		[LocDisplayName(RulesTasksStrings.IDs.AttributeValueDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.AttributeValueDescription)]
		public string AttributeValue
		{
			get
			{
				return this.attributeValue;
			}
			set
			{
				this.attributeValue = value;
			}
		}

		[LocDescription(RulesTasksStrings.IDs.ADAttributeDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.ADAttributeDisplayName)]
		public ADAttribute ADAttribute
		{
			get
			{
				return this.adAttribute;
			}
			set
			{
				this.adAttribute = value;
			}
		}

		[LocDescription(RulesTasksStrings.IDs.ADAttributeEvaluationTypeDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.ADAttributeEvaluationTypeDisplayName)]
		public ADAttributeEvaluationType ADAttributeEvaluationType
		{
			get
			{
				return this.adAttributeEvaluationType;
			}
			set
			{
				this.adAttributeEvaluationType = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionADAttributeMatchesText(LocalizedDescriptionAttribute.FromEnum(typeof(EvaluatedUser), this.EvaluatedUser), LocalizedDescriptionAttribute.FromEnum(typeof(ADAttribute), this.ADAttribute), LocalizedDescriptionAttribute.FromEnum(typeof(ADAttributeEvaluationType), this.ADAttributeEvaluationType), this.AttributeValue);
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return null;
		}

		internal static TransportRulePredicate CreatePredicateFromBifInfo(RuleBifurcationInfo bifInfo)
		{
			if (bifInfo.ADAttributesForTextMatch.Count == 0 || bifInfo.ADAttributes.Count > 0 || bifInfo.Managers.Count > 0 || bifInfo.Recipients.Count > 0 || bifInfo.Lists.Count > 0 || bifInfo.FromRecipients.Count > 0 || bifInfo.FromLists.Count > 0 || bifInfo.Partners.Count > 0 || bifInfo.RecipientAddressContainsWords.Count > 0 || bifInfo.RecipientDomainIs.Count > 0 || bifInfo.RecipientMatchesPatterns.Count > 0 || bifInfo.RecipientAttributeContains.Count > 0 || bifInfo.RecipientAttributeMatches.Count > 0 || bifInfo.SenderInRecipientList.Count > 0 || bifInfo.RecipientInSenderList.Count > 0)
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
			ADAttributeMatchesTextPredicate adattributeMatchesTextPredicate = new ADAttributeMatchesTextPredicate
			{
				ADAttribute = (ADAttribute)Enum.Parse(typeof(ADAttribute), bifInfo.ADAttributesForTextMatch[0]),
				AttributeValue = bifInfo.ADAttributeValue
			};
			if (bifInfo.CheckADAttributeEquality)
			{
				adattributeMatchesTextPredicate.ADAttributeEvaluationType = ADAttributeEvaluationType.Equals;
			}
			else
			{
				adattributeMatchesTextPredicate.ADAttributeEvaluationType = ADAttributeEvaluationType.Contains;
			}
			if (bifInfo.IsSenderEvaluation)
			{
				adattributeMatchesTextPredicate.EvaluatedUser = EvaluatedUser.Sender;
			}
			else
			{
				adattributeMatchesTextPredicate.EvaluatedUser = EvaluatedUser.Recipient;
			}
			return adattributeMatchesTextPredicate;
		}

		internal override RuleBifurcationInfo ToRuleBifurcationInfo(out RuleBifurcationInfo additionalBifurcationInfo)
		{
			additionalBifurcationInfo = null;
			return new RuleBifurcationInfo
			{
				ADAttributesForTextMatch = 
				{
					this.ADAttribute.ToString()
				},
				CheckADAttributeEquality = (this.ADAttributeEvaluationType == ADAttributeEvaluationType.Equals),
				IsSenderEvaluation = (this.EvaluatedUser == EvaluatedUser.Sender),
				ADAttributeValue = this.AttributeValue
			};
		}

		private ADAttribute adAttribute;

		private string attributeValue;

		private EvaluatedUser evaluatedUser;

		private ADAttributeEvaluationType adAttributeEvaluationType;
	}
}
