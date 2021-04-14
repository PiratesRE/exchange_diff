using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class SentToScopePredicate : BifurcationInfoPredicate, IEquatable<SentToScopePredicate>
	{
		public override int GetHashCode()
		{
			return this.Scope.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as SentToScopePredicate)));
		}

		public bool Equals(SentToScopePredicate other)
		{
			return this.Scope.Equals(other.Scope);
		}

		[ConditionParameterName("SentToScope")]
		[ExceptionParameterName("ExceptIfSentToScope")]
		[LocDisplayName(RulesTasksStrings.IDs.ToScopeDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.ToScopeDescription)]
		public ToUserScope Scope
		{
			get
			{
				return this.scope;
			}
			set
			{
				this.scope = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionSentToScope(LocalizedDescriptionAttribute.FromEnum(typeof(ToUserScope), this.Scope));
			}
		}

		[LocDisplayName(RulesTasksStrings.IDs.SubTypeDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.SubTypeDescription)]
		public override IEnumerable<RuleSubType> RuleSubTypes
		{
			get
			{
				return new RuleSubType[]
				{
					RuleSubType.None,
					RuleSubType.Dlp
				};
			}
		}

		internal static TransportRulePredicate CreatePredicateFromBifInfo(RuleBifurcationInfo bifInfo)
		{
			if (bifInfo.ADAttributesForTextMatch.Count > 0 || bifInfo.ADAttributes.Count > 0 || bifInfo.Managers.Count > 0 || bifInfo.Recipients.Count > 0 || bifInfo.Lists.Count > 0 || bifInfo.FromRecipients.Count > 0 || bifInfo.FromLists.Count > 0 || bifInfo.Partners.Count > 0 || bifInfo.RecipientAddressContainsWords.Count > 0 || bifInfo.RecipientDomainIs.Count > 0 || bifInfo.RecipientMatchesPatterns.Count > 0 || bifInfo.RecipientAttributeContains.Count > 0 || bifInfo.RecipientAttributeMatches.Count > 0 || bifInfo.SenderInRecipientList.Count > 0 || bifInfo.RecipientInSenderList.Count > 0)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(bifInfo.ManagementRelationship))
			{
				return null;
			}
			SentToScopePredicate sentToScopePredicate = new SentToScopePredicate();
			if (bifInfo.InternalRecipients && !bifInfo.ExternalRecipients && !bifInfo.ExternalPartnerRecipients && !bifInfo.ExternalNonPartnerRecipients)
			{
				sentToScopePredicate.Scope = ToUserScope.InOrganization;
				return sentToScopePredicate;
			}
			if (!bifInfo.InternalRecipients && bifInfo.ExternalRecipients && !bifInfo.ExternalPartnerRecipients && !bifInfo.ExternalNonPartnerRecipients)
			{
				sentToScopePredicate.Scope = ToUserScope.NotInOrganization;
				return sentToScopePredicate;
			}
			if (!bifInfo.InternalRecipients && !bifInfo.ExternalRecipients && bifInfo.ExternalPartnerRecipients && !bifInfo.ExternalNonPartnerRecipients)
			{
				sentToScopePredicate.Scope = ToUserScope.ExternalPartner;
				return sentToScopePredicate;
			}
			if (!bifInfo.InternalRecipients && !bifInfo.ExternalRecipients && !bifInfo.ExternalPartnerRecipients && bifInfo.ExternalNonPartnerRecipients)
			{
				sentToScopePredicate.Scope = ToUserScope.ExternalNonPartner;
				return sentToScopePredicate;
			}
			return null;
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return null;
		}

		internal override void Reset()
		{
			this.scope = ToUserScope.InOrganization;
			base.Reset();
		}

		internal override RuleBifurcationInfo ToRuleBifurcationInfo(out RuleBifurcationInfo additionalBifurcationInfo)
		{
			additionalBifurcationInfo = null;
			RuleBifurcationInfo ruleBifurcationInfo = new RuleBifurcationInfo();
			switch (this.scope)
			{
			case ToUserScope.InOrganization:
				ruleBifurcationInfo.InternalRecipients = true;
				break;
			case ToUserScope.NotInOrganization:
				ruleBifurcationInfo.ExternalRecipients = true;
				break;
			case ToUserScope.ExternalPartner:
				ruleBifurcationInfo.ExternalPartnerRecipients = true;
				break;
			case ToUserScope.ExternalNonPartner:
				ruleBifurcationInfo.ExternalNonPartnerRecipients = true;
				break;
			default:
				return null;
			}
			return ruleBifurcationInfo;
		}

		internal override string GetPredicateParameters()
		{
			return Enum.GetName(typeof(ToUserScope), this.Scope);
		}

		private ToUserScope scope;
	}
}
