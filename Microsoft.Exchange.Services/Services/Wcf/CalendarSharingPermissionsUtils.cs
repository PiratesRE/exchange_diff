using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal static class CalendarSharingPermissionsUtils
	{
		public static bool IsPrincipalInteresting(PermissionSecurityPrincipal.SecurityPrincipalType type)
		{
			return type == PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal || type.IsInternalUserPrincipal();
		}

		internal static bool IsInternalUserPrincipal(this PermissionSecurityPrincipal.SecurityPrincipalType type)
		{
			return type == PermissionSecurityPrincipal.SecurityPrincipalType.ADRecipientPrincipal;
		}

		public static bool ShouldSkipPermission(CalendarFolderPermission permission, bool isSharingDefaultCalendar)
		{
			if (!isSharingDefaultCalendar)
			{
				if (permission.Principal.Type.IsInternalUserPrincipal())
				{
					return permission.PermissionLevel != PermissionLevel.Reviewer && permission.PermissionLevel != PermissionLevel.Editor;
				}
				return permission.PermissionLevel != PermissionLevel.Reviewer;
			}
			else
			{
				if (permission.PermissionLevel == PermissionLevel.Reviewer || permission.PermissionLevel == PermissionLevel.Editor)
				{
					return false;
				}
				if (permission.PermissionLevel == PermissionLevel.Custom || permission.PermissionLevel == PermissionLevel.None)
				{
					return permission.FreeBusyAccess == FreeBusyAccess.None;
				}
				return CalendarSharingPermissionsUtils.IsPrincipalInteresting(permission.Principal.Type) || true;
			}
		}

		public static DetailLevelEnumType GetMaximumDetailLevel(SharingPolicy policy, CalendarFolderPermission permission)
		{
			DetailLevelEnumType result = DetailLevelEnumType.AvailabilityOnly;
			if (permission.Principal.Type == PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal)
			{
				SharingPolicyAction sharingAction;
				if (permission.Principal.ExternalUser.IsReachUser)
				{
					sharingAction = policy.GetAllowedForAnonymousCalendarSharing();
				}
				else
				{
					sharingAction = policy.GetAllowed(permission.Principal.ExternalUser.OriginalSmtpAddress.Domain);
				}
				CalendarSharingPermissionsUtils.TryGetMaximumDetailLevel(sharingAction, out result);
			}
			else if (permission.Principal.Type.IsInternalUserPrincipal())
			{
				result = DetailLevelEnumType.Editor;
			}
			return result;
		}

		public static bool TryGetMaximumDetailLevel(SharingPolicyAction sharingAction, out DetailLevelEnumType maxLevel)
		{
			maxLevel = DetailLevelEnumType.AvailabilityOnly;
			if (sharingAction > (SharingPolicyAction)0)
			{
				maxLevel = (DetailLevelEnumType)PolicyAllowedDetailLevel.GetMaxAllowed(sharingAction);
				return true;
			}
			return false;
		}

		public static DetailLevelEnumType GetAdjustedDetailLevel(SharingPolicyAction sharingAction, DetailLevelEnumType desiredLevel)
		{
			DetailLevelEnumType result = desiredLevel;
			DetailLevelEnumType detailLevelEnumType;
			CalendarSharingPermissionsUtils.TryGetMaximumDetailLevel(sharingAction, out detailLevelEnumType);
			if (desiredLevel > detailLevelEnumType)
			{
				result = detailLevelEnumType;
			}
			return result;
		}

		public static bool CheckIfRecipientDomainIsInternal(OrganizationId organizationId, string domain)
		{
			CachedOrganizationConfiguration instance = CachedOrganizationConfiguration.GetInstance(organizationId, CachedOrganizationConfiguration.ConfigurationTypes.All);
			return instance.Domains.IsInternal(domain);
		}

		internal static string[] CalculateAllowedDetailLevels(DetailLevelEnumType maxDetailLevel, bool isSharingDefaultCalendar, PermissionSecurityPrincipal.SecurityPrincipalType principalType)
		{
			List<string> list = new List<string>();
			CalendarSharingDetailLevel calendarSharingDetailLevel = CalendarSharingDetailLevel.AvailabilityOnly;
			if (!isSharingDefaultCalendar && principalType == PermissionSecurityPrincipal.SecurityPrincipalType.ADRecipientPrincipal)
			{
				calendarSharingDetailLevel = CalendarSharingDetailLevel.FullDetails;
			}
			CalendarSharingDetailLevel calendarSharingDetailLevel2 = CalendarSharingPermissionsUtils.ConvertToCalendarSharingDetailLevelEnum(maxDetailLevel, isSharingDefaultCalendar);
			for (CalendarSharingDetailLevel calendarSharingDetailLevel3 = calendarSharingDetailLevel; calendarSharingDetailLevel3 <= calendarSharingDetailLevel2; calendarSharingDetailLevel3++)
			{
				list.Add(calendarSharingDetailLevel3.ToString());
			}
			return list.ToArray();
		}

		internal static CalendarSharingDetailLevel ConvertToCalendarSharingDetailLevelEnum(DetailLevelEnumType detailLevel, bool isSharingDefaultCalendar)
		{
			CalendarSharingDetailLevel result = CalendarSharingDetailLevel.AvailabilityOnly;
			if (detailLevel == DetailLevelEnumType.Editor && isSharingDefaultCalendar)
			{
				result = CalendarSharingDetailLevel.Delegate;
			}
			else if (detailLevel == DetailLevelEnumType.Editor)
			{
				result = CalendarSharingDetailLevel.Editor;
			}
			else if (detailLevel == DetailLevelEnumType.FullDetails)
			{
				result = CalendarSharingDetailLevel.FullDetails;
			}
			else if (detailLevel == DetailLevelEnumType.LimitedDetails)
			{
				result = CalendarSharingDetailLevel.LimitedDetails;
			}
			else if (detailLevel == DetailLevelEnumType.AvailabilityOnly)
			{
				result = CalendarSharingDetailLevel.AvailabilityOnly;
			}
			return result;
		}

		internal static bool GetSharingDetailLevelFromPermissionLevel(CalendarFolderPermission permission, bool isSharingDefaultCalendar, DelegateUserCollectionHandler delegateUserCollectionHandler, out CalendarSharingDetailLevel sharingDetailLevel, out bool canViewPrivateItems)
		{
			bool result = false;
			canViewPrivateItems = false;
			sharingDetailLevel = CalendarSharingDetailLevel.AvailabilityOnly;
			if (permission.PermissionLevel == PermissionLevel.Editor)
			{
				if (permission.Principal.Type.IsInternalUserPrincipal())
				{
					sharingDetailLevel = CalendarSharingDetailLevel.Editor;
					if (isSharingDefaultCalendar)
					{
						DelegateUser delegateUser = delegateUserCollectionHandler.GetDelegateUser(permission.Principal.ADRecipient);
						if (delegateUser != null)
						{
							sharingDetailLevel = CalendarSharingDetailLevel.Delegate;
							canViewPrivateItems = delegateUser.CanViewPrivateItems;
						}
					}
					result = true;
				}
			}
			else if (permission.PermissionLevel == PermissionLevel.Reviewer)
			{
				sharingDetailLevel = CalendarSharingDetailLevel.FullDetails;
				result = true;
			}
			else if (permission.FreeBusyAccess == FreeBusyAccess.Details)
			{
				if (isSharingDefaultCalendar || (!isSharingDefaultCalendar && !permission.Principal.Type.IsInternalUserPrincipal()))
				{
					sharingDetailLevel = CalendarSharingDetailLevel.LimitedDetails;
					result = true;
				}
			}
			else if (permission.FreeBusyAccess == FreeBusyAccess.Basic && (isSharingDefaultCalendar || (!isSharingDefaultCalendar && !permission.Principal.Type.IsInternalUserPrincipal())))
			{
				sharingDetailLevel = CalendarSharingDetailLevel.AvailabilityOnly;
				result = true;
			}
			return result;
		}
	}
}
