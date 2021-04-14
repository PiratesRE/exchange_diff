using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class OwaOptionClientStrings
	{
		static OwaOptionClientStrings()
		{
			OwaOptionClientStrings.stringIDs.Add(2915177744U, "TeamMailboxMembershipString4");
			OwaOptionClientStrings.stringIDs.Add(3044047780U, "TeamMailboxYourRoleMember");
			OwaOptionClientStrings.stringIDs.Add(1220496629U, "TeamMailboxSyncSuccess");
			OwaOptionClientStrings.stringIDs.Add(1210225031U, "TeamMailboxCreationFailedPageLinkBackToSiteDisabled");
			OwaOptionClientStrings.stringIDs.Add(3318462271U, "TeamMailboxMembershipString1");
			OwaOptionClientStrings.stringIDs.Add(1582897606U, "NewTeamMailboxFailed");
			OwaOptionClientStrings.stringIDs.Add(2554950321U, "TeamMailboxYourRoleOwner");
			OwaOptionClientStrings.stringIDs.Add(1347892332U, "TeamMailboxLifecycleStatusActiveString2");
			OwaOptionClientStrings.stringIDs.Add(3789638106U, "TeamMailboxStartedMaintenanceSync");
			OwaOptionClientStrings.stringIDs.Add(1971980913U, "TeamMailboxStartedMembershipSync");
			OwaOptionClientStrings.stringIDs.Add(1904041070U, "UpdateTimeZonePrompt");
			OwaOptionClientStrings.stringIDs.Add(319275761U, "Install");
			OwaOptionClientStrings.stringIDs.Add(3457463314U, "RetrieveInfo");
			OwaOptionClientStrings.stringIDs.Add(3735025842U, "TeamMailboxCreationCompletionPageHeadingText");
			OwaOptionClientStrings.stringIDs.Add(3477886725U, "TeamMailboxLifecycleClosed");
			OwaOptionClientStrings.stringIDs.Add(310858686U, "TeamMailboxLifecycleStatusClosedString2");
			OwaOptionClientStrings.stringIDs.Add(3896101628U, "TeamMailboxSyncError");
			OwaOptionClientStrings.stringIDs.Add(1336091985U, "ClearOmsAccountPrompt");
			OwaOptionClientStrings.stringIDs.Add(1752378330U, "TeamMailboxMembershipString2");
			OwaOptionClientStrings.stringIDs.Add(3861666036U, "SiteMailboxEmailMeDiagnosticsConfirmation");
			OwaOptionClientStrings.stringIDs.Add(4213595441U, "AgaveInstallTitle");
			OwaOptionClientStrings.stringIDs.Add(4121209658U, "TeamMailboxCreationCompletionPageDetailedText");
			OwaOptionClientStrings.stringIDs.Add(186294389U, "TeamMailboxMembershipString3");
			OwaOptionClientStrings.stringIDs.Add(2400383839U, "NewTeamMailboxInProgress");
			OwaOptionClientStrings.stringIDs.Add(767215867U, "TeamMailboxCreationCompletionPageLinkTrySiteMailbox");
			OwaOptionClientStrings.stringIDs.Add(2503542192U, "TeamMailboxSyncStatus");
			OwaOptionClientStrings.stringIDs.Add(2773017722U, "TeamMailboxSyncDate");
			OwaOptionClientStrings.stringIDs.Add(1347892331U, "TeamMailboxLifecycleStatusActiveString1");
			OwaOptionClientStrings.stringIDs.Add(1300295012U, "FileUploadFailed");
			OwaOptionClientStrings.stringIDs.Add(167704095U, "TeamMailboxLifecycleActive");
			OwaOptionClientStrings.stringIDs.Add(1138644365U, "TeamMailboxYourRoleNoAccess");
			OwaOptionClientStrings.stringIDs.Add(4287680888U, "PleaseConfirm");
			OwaOptionClientStrings.stringIDs.Add(1813773069U, "TeamMailboxSharePointConnectedTrue");
			OwaOptionClientStrings.stringIDs.Add(4124603460U, "TeamMailboxCreationCompletionPageLinkBackToSite");
			OwaOptionClientStrings.stringIDs.Add(3751931040U, "TeamMailboxStartedDocumentSync");
			OwaOptionClientStrings.stringIDs.Add(489758736U, "TeamMailboxSharePointConnectedFalse");
			OwaOptionClientStrings.stringIDs.Add(3476723026U, "TeamMailboxSyncNotAvailable");
		}

		public static string TeamMailboxMembershipString4
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxMembershipString4");
			}
		}

		public static string TeamMailboxYourRoleMember
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxYourRoleMember");
			}
		}

		public static string TeamMailboxSyncSuccess
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxSyncSuccess");
			}
		}

		public static string TeamMailboxCreationFailedPageLinkBackToSiteDisabled
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxCreationFailedPageLinkBackToSiteDisabled");
			}
		}

		public static string TeamMailboxMembershipString1
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxMembershipString1");
			}
		}

		public static string NewTeamMailboxFailed
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("NewTeamMailboxFailed");
			}
		}

		public static string TeamMailboxYourRoleOwner
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxYourRoleOwner");
			}
		}

		public static string TeamMailboxLifecycleStatusActiveString2
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxLifecycleStatusActiveString2");
			}
		}

		public static string AgaveVersion(string ver)
		{
			return string.Format(OwaOptionClientStrings.ResourceManager.GetString("AgaveVersion"), ver);
		}

		public static string TeamMailboxStartedMaintenanceSync
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxStartedMaintenanceSync");
			}
		}

		public static string TeamMailboxStartedMembershipSync
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxStartedMembershipSync");
			}
		}

		public static string UpdateTimeZonePrompt
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("UpdateTimeZonePrompt");
			}
		}

		public static string TeamMailboxLifecycleStatusClosedString1(string time)
		{
			return string.Format(OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxLifecycleStatusClosedString1"), time);
		}

		public static string Install
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("Install");
			}
		}

		public static string RetrieveInfo
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("RetrieveInfo");
			}
		}

		public static string TeamMailboxCreationCompletionPageHeadingText
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxCreationCompletionPageHeadingText");
			}
		}

		public static string TeamMailboxLifecycleClosed
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxLifecycleClosed");
			}
		}

		public static string TeamMailboxLifecycleStatusClosedString2
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxLifecycleStatusClosedString2");
			}
		}

		public static string AgaveProvider(string publisher)
		{
			return string.Format(OwaOptionClientStrings.ResourceManager.GetString("AgaveProvider"), publisher);
		}

		public static string TeamMailboxSyncError
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxSyncError");
			}
		}

		public static string AgaveDisplayName(string name)
		{
			return string.Format(OwaOptionClientStrings.ResourceManager.GetString("AgaveDisplayName"), name);
		}

		public static string ClearOmsAccountPrompt
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("ClearOmsAccountPrompt");
			}
		}

		public static string TeamMailboxMembershipString2
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxMembershipString2");
			}
		}

		public static string AgaveRequirementsValue(string details)
		{
			return string.Format(OwaOptionClientStrings.ResourceManager.GetString("AgaveRequirementsValue"), details);
		}

		public static string SiteMailboxEmailMeDiagnosticsConfirmation
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("SiteMailboxEmailMeDiagnosticsConfirmation");
			}
		}

		public static string AgaveInstallTitle
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("AgaveInstallTitle");
			}
		}

		public static string TeamMailboxCreationCompletionPageDetailedText
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxCreationCompletionPageDetailedText");
			}
		}

		public static string TeamMailboxMembershipString3
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxMembershipString3");
			}
		}

		public static string NewTeamMailboxInProgress
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("NewTeamMailboxInProgress");
			}
		}

		public static string TeamMailboxCreationCompletionPageLinkTrySiteMailbox
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxCreationCompletionPageLinkTrySiteMailbox");
			}
		}

		public static string TeamMailboxSyncStatus
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxSyncStatus");
			}
		}

		public static string TeamMailboxSyncDate
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxSyncDate");
			}
		}

		public static string SendPasscodeSucceededFormat(string phone)
		{
			return string.Format(OwaOptionClientStrings.ResourceManager.GetString("SendPasscodeSucceededFormat"), phone);
		}

		public static string TeamMailboxLifecycleStatusActiveString1
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxLifecycleStatusActiveString1");
			}
		}

		public static string FileUploadFailed
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("FileUploadFailed");
			}
		}

		public static string TeamMailboxLifecycleActive
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxLifecycleActive");
			}
		}

		public static string TeamMailboxYourRoleNoAccess
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxYourRoleNoAccess");
			}
		}

		public static string PleaseConfirm
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("PleaseConfirm");
			}
		}

		public static string TeamMailboxSharePointConnectedTrue
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxSharePointConnectedTrue");
			}
		}

		public static string TeamMailboxCreationCompletionPageLinkBackToSite
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxCreationCompletionPageLinkBackToSite");
			}
		}

		public static string TeamMailboxStartedDocumentSync
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxStartedDocumentSync");
			}
		}

		public static string TeamMailboxSharePointConnectedFalse
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxSharePointConnectedFalse");
			}
		}

		public static string TeamMailboxSyncNotAvailable
		{
			get
			{
				return OwaOptionClientStrings.ResourceManager.GetString("TeamMailboxSyncNotAvailable");
			}
		}

		public static string GetLocalizedString(OwaOptionClientStrings.IDs key)
		{
			return OwaOptionClientStrings.ResourceManager.GetString(OwaOptionClientStrings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(37);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.Management.ControlPanel.OwaOptionClientStrings", typeof(OwaOptionClientStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			TeamMailboxMembershipString4 = 2915177744U,
			TeamMailboxYourRoleMember = 3044047780U,
			TeamMailboxSyncSuccess = 1220496629U,
			TeamMailboxCreationFailedPageLinkBackToSiteDisabled = 1210225031U,
			TeamMailboxMembershipString1 = 3318462271U,
			NewTeamMailboxFailed = 1582897606U,
			TeamMailboxYourRoleOwner = 2554950321U,
			TeamMailboxLifecycleStatusActiveString2 = 1347892332U,
			TeamMailboxStartedMaintenanceSync = 3789638106U,
			TeamMailboxStartedMembershipSync = 1971980913U,
			UpdateTimeZonePrompt = 1904041070U,
			Install = 319275761U,
			RetrieveInfo = 3457463314U,
			TeamMailboxCreationCompletionPageHeadingText = 3735025842U,
			TeamMailboxLifecycleClosed = 3477886725U,
			TeamMailboxLifecycleStatusClosedString2 = 310858686U,
			TeamMailboxSyncError = 3896101628U,
			ClearOmsAccountPrompt = 1336091985U,
			TeamMailboxMembershipString2 = 1752378330U,
			SiteMailboxEmailMeDiagnosticsConfirmation = 3861666036U,
			AgaveInstallTitle = 4213595441U,
			TeamMailboxCreationCompletionPageDetailedText = 4121209658U,
			TeamMailboxMembershipString3 = 186294389U,
			NewTeamMailboxInProgress = 2400383839U,
			TeamMailboxCreationCompletionPageLinkTrySiteMailbox = 767215867U,
			TeamMailboxSyncStatus = 2503542192U,
			TeamMailboxSyncDate = 2773017722U,
			TeamMailboxLifecycleStatusActiveString1 = 1347892331U,
			FileUploadFailed = 1300295012U,
			TeamMailboxLifecycleActive = 167704095U,
			TeamMailboxYourRoleNoAccess = 1138644365U,
			PleaseConfirm = 4287680888U,
			TeamMailboxSharePointConnectedTrue = 1813773069U,
			TeamMailboxCreationCompletionPageLinkBackToSite = 4124603460U,
			TeamMailboxStartedDocumentSync = 3751931040U,
			TeamMailboxSharePointConnectedFalse = 489758736U,
			TeamMailboxSyncNotAvailable = 3476723026U
		}

		private enum ParamIDs
		{
			AgaveVersion,
			TeamMailboxLifecycleStatusClosedString1,
			AgaveProvider,
			AgaveDisplayName,
			AgaveRequirementsValue,
			SendPasscodeSucceededFormat
		}
	}
}
