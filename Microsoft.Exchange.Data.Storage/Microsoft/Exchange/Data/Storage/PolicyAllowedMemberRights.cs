using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PolicyAllowedMemberRights
	{
		public static MemberRights GetAllowed(SharingPolicyAction allowedActions, StoreObjectType storeObjectType)
		{
			EnumValidator.ThrowIfInvalid<StoreObjectType>(storeObjectType, "storeObjectType");
			EnumValidator.ThrowIfInvalid<SharingPolicyAction>(allowedActions, "allowedActions");
			MemberRights memberRights = MemberRights.None;
			foreach (PolicyAllowedMemberRights policyAllowedMemberRights in PolicyAllowedMemberRights.Rights)
			{
				if (policyAllowedMemberRights.storeObjectType == storeObjectType && (allowedActions & policyAllowedMemberRights.action) == policyAllowedMemberRights.action)
				{
					memberRights |= policyAllowedMemberRights.allowedRights;
				}
			}
			return memberRights;
		}

		public static bool IsAllowedOnFolder(StoreObjectType storeObjectType)
		{
			EnumValidator.ThrowIfInvalid<StoreObjectType>(storeObjectType, "storeObjectType");
			foreach (PolicyAllowedMemberRights policyAllowedMemberRights in PolicyAllowedMemberRights.Rights)
			{
				if (policyAllowedMemberRights.storeObjectType == storeObjectType)
				{
					return true;
				}
			}
			return false;
		}

		private PolicyAllowedMemberRights(StoreObjectType storeObjectType, SharingPolicyAction action, MemberRights allowedRights)
		{
			this.storeObjectType = storeObjectType;
			this.action = action;
			this.allowedRights = allowedRights;
		}

		private static readonly PolicyAllowedMemberRights[] Rights = new PolicyAllowedMemberRights[]
		{
			new PolicyAllowedMemberRights(StoreObjectType.CalendarFolder, SharingPolicyAction.CalendarSharingFreeBusySimple, MemberRights.Visible | MemberRights.FreeBusySimple),
			new PolicyAllowedMemberRights(StoreObjectType.CalendarFolder, SharingPolicyAction.CalendarSharingFreeBusyDetail, MemberRights.Visible | MemberRights.FreeBusySimple | MemberRights.FreeBusyDetailed),
			new PolicyAllowedMemberRights(StoreObjectType.CalendarFolder, SharingPolicyAction.CalendarSharingFreeBusyReviewer, MemberRights.ReadAny | MemberRights.Visible | MemberRights.FreeBusySimple | MemberRights.FreeBusyDetailed),
			new PolicyAllowedMemberRights(StoreObjectType.ContactsFolder, SharingPolicyAction.ContactsSharing, MemberRights.ReadAny | MemberRights.Visible)
		};

		private StoreObjectType storeObjectType;

		private SharingPolicyAction action;

		private MemberRights allowedRights;
	}
}
