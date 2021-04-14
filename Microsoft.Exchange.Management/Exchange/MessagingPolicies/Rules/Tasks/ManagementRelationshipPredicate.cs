using System;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class ManagementRelationshipPredicate : BifurcationInfoPredicate, IEquatable<ManagementRelationshipPredicate>
	{
		public override int GetHashCode()
		{
			return this.ManagementRelationship.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as ManagementRelationshipPredicate)));
		}

		public bool Equals(ManagementRelationshipPredicate other)
		{
			return this.ManagementRelationship.Equals(other.ManagementRelationship);
		}

		[LocDisplayName(RulesTasksStrings.IDs.ManagementRelationshipDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.ManagementRelationshipDescription)]
		[ConditionParameterName("SenderManagementRelationship")]
		[ExceptionParameterName("ExceptIfSenderManagementRelationship")]
		public ManagementRelationship ManagementRelationship
		{
			get
			{
				return this.managementRelationship;
			}
			set
			{
				this.managementRelationship = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionManagementRelationship(LocalizedDescriptionAttribute.FromEnum(typeof(ManagementRelationship), this.ManagementRelationship));
			}
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return null;
		}

		internal static TransportRulePredicate CreatePredicateFromBifInfo(RuleBifurcationInfo bifInfo)
		{
			if (bifInfo.ADAttributesForTextMatch.Count > 0 || bifInfo.ADAttributes.Count > 0 || bifInfo.Managers.Count > 0 || bifInfo.Recipients.Count > 0 || bifInfo.Lists.Count > 0 || bifInfo.FromRecipients.Count > 0 || bifInfo.FromLists.Count > 0 || bifInfo.Partners.Count > 0 || bifInfo.RecipientAddressContainsWords.Count > 0 || bifInfo.RecipientDomainIs.Count > 0 || bifInfo.RecipientMatchesPatterns.Count > 0 || bifInfo.RecipientAttributeContains.Count > 0 || bifInfo.RecipientAttributeMatches.Count > 0 || bifInfo.SenderInRecipientList.Count > 0 || bifInfo.RecipientInSenderList.Count > 0)
			{
				return null;
			}
			if (bifInfo.InternalRecipients || bifInfo.ExternalRecipients || bifInfo.ExternalPartnerRecipients || bifInfo.ExternalNonPartnerRecipients)
			{
				return null;
			}
			if (string.IsNullOrEmpty(bifInfo.ManagementRelationship))
			{
				return null;
			}
			ManagementRelationshipPredicate managementRelationshipPredicate = new ManagementRelationshipPredicate();
			if (string.Equals(bifInfo.ManagementRelationship, ManagementRelationship.Manager.ToString(), StringComparison.InvariantCultureIgnoreCase))
			{
				managementRelationshipPredicate.ManagementRelationship = ManagementRelationship.Manager;
			}
			else
			{
				managementRelationshipPredicate.ManagementRelationship = ManagementRelationship.DirectReport;
			}
			return managementRelationshipPredicate;
		}

		internal override RuleBifurcationInfo ToRuleBifurcationInfo(out RuleBifurcationInfo additionalBifurcationInfo)
		{
			additionalBifurcationInfo = null;
			return new RuleBifurcationInfo
			{
				ManagementRelationship = this.ManagementRelationship.ToString()
			};
		}

		internal override string GetPredicateParameters()
		{
			return Enum.GetName(typeof(ManagementRelationship), this.ManagementRelationship);
		}

		private ManagementRelationship managementRelationship;
	}
}
