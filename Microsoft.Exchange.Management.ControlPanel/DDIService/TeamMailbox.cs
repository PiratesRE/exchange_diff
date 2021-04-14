using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.DDIService
{
	public sealed class TeamMailbox
	{
		public static void NewTeamMailboxPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			TeamMailbox.TeamMailboxPostAction(inputRow, dataTable, store);
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				dataRow["Owners"] = new ADObjectId[]
				{
					RbacPrincipal.Current.ExecutingUserId
				};
			}
		}

		public static void TeamMailboxPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				dataRow["IsOwner"] = TeamMailbox.IsExecutingUserAnOwner(dataRow["Owners"]);
				if (dataRow["Active"] is bool)
				{
					if ((bool)dataRow["Active"])
					{
						dataRow["Status"] = OwaOptionClientStrings.TeamMailboxLifecycleActive;
					}
					else
					{
						dataRow["Status"] = OwaOptionClientStrings.TeamMailboxLifecycleClosed;
						dataRow["ClosedTime"] = ((DateTime)dataRow["ClosedTime"]).UtcToUserDateString();
					}
					dataRow["IsEditable"] = ((bool)dataRow["IsOwner"] && (bool)dataRow["Active"]);
				}
				int userCount = TeamMailbox.GetUserCount(dataRow["Members"]);
				if (userCount == 0)
				{
					dataRow["MembersNumber"] = "0";
				}
				else
				{
					dataRow["MembersNumber"] = userCount;
				}
				dataRow["SPSiteLinked"] = ((dataRow["SharePointLinkedBy"] is ADObjectId) ? OwaOptionClientStrings.TeamMailboxSharePointConnectedTrue : OwaOptionClientStrings.TeamMailboxSharePointConnectedFalse);
				if (dataRow["MyRole"] is string)
				{
					string a;
					if ((a = (string)dataRow["MyRole"]) != null)
					{
						if (a == "Owner")
						{
							dataRow["MyRole"] = OwaOptionClientStrings.TeamMailboxYourRoleOwner;
							goto IL_1B1;
						}
						if (a == "Member")
						{
							dataRow["MyRole"] = OwaOptionClientStrings.TeamMailboxYourRoleMember;
							goto IL_1B1;
						}
					}
					dataRow["MyRole"] = OwaOptionClientStrings.TeamMailboxYourRoleNoAccess;
				}
				IL_1B1:
				dataRow["ExecutingUserId"] = RbacPrincipal.Current.ExecutingUserId;
				dataRow["MailToPrimarySmtpAddress"] = "mailto:" + dataRow["PrimarySmtpAddress"];
			}
		}

		public static void TeamMailboxDiagnosticsPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			TeamMailboxDiagnosticsInfo teamMailboxDiagnosticsInfo = store.GetDataObject("TeamMailboxDiagnosticsInfo") as TeamMailboxDiagnosticsInfo;
			if (dataTable.Rows.Count == 1 && teamMailboxDiagnosticsInfo != null)
			{
				if (teamMailboxDiagnosticsInfo.Status == TeamMailboxSyncStatus.NotAvailable)
				{
					dataTable.Rows[0]["DocumentSyncStatus"] = OwaOptionClientStrings.TeamMailboxSyncNotAvailable;
					dataTable.Rows[0]["MembershipSyncStatus"] = OwaOptionClientStrings.TeamMailboxSyncNotAvailable;
					dataTable.Rows[0]["MaintenanceSyncStatus"] = OwaOptionClientStrings.TeamMailboxSyncNotAvailable;
				}
				else
				{
					dataTable.Rows[0]["DocumentSyncStatus"] = ((teamMailboxDiagnosticsInfo.HierarchySyncInfo == null) ? OwaOptionClientStrings.TeamMailboxSyncNotAvailable : OwaOptionClientStrings.TeamMailboxSyncSuccess);
					dataTable.Rows[0]["MembershipSyncStatus"] = ((teamMailboxDiagnosticsInfo.MembershipSyncInfo == null) ? OwaOptionClientStrings.TeamMailboxSyncNotAvailable : OwaOptionClientStrings.TeamMailboxSyncSuccess);
					dataTable.Rows[0]["MaintenanceSyncStatus"] = ((teamMailboxDiagnosticsInfo.MaintenanceSyncInfo == null) ? OwaOptionClientStrings.TeamMailboxSyncNotAvailable : OwaOptionClientStrings.TeamMailboxSyncSuccess);
				}
				if (teamMailboxDiagnosticsInfo.Status == TeamMailboxSyncStatus.Failed || teamMailboxDiagnosticsInfo.Status == TeamMailboxSyncStatus.DocumentSyncFailureOnly || teamMailboxDiagnosticsInfo.Status == TeamMailboxSyncStatus.DocumentAndMembershipSyncFailure || teamMailboxDiagnosticsInfo.Status == TeamMailboxSyncStatus.DocumentAndMaintenanceSyncFailure)
				{
					dataTable.Rows[0]["DocumentSyncStatus"] = OwaOptionClientStrings.TeamMailboxSyncError;
				}
				if (teamMailboxDiagnosticsInfo.Status == TeamMailboxSyncStatus.Failed || teamMailboxDiagnosticsInfo.Status == TeamMailboxSyncStatus.MembershipSyncFailureOnly || teamMailboxDiagnosticsInfo.Status == TeamMailboxSyncStatus.DocumentAndMembershipSyncFailure || teamMailboxDiagnosticsInfo.Status == TeamMailboxSyncStatus.MembershipAndMaintenanceSyncFailure)
				{
					dataTable.Rows[0]["MembershipSyncStatus"] = OwaOptionClientStrings.TeamMailboxSyncError;
				}
				if (teamMailboxDiagnosticsInfo.Status == TeamMailboxSyncStatus.Failed || teamMailboxDiagnosticsInfo.Status == TeamMailboxSyncStatus.MaintenanceSyncFailureOnly || teamMailboxDiagnosticsInfo.Status == TeamMailboxSyncStatus.MembershipAndMaintenanceSyncFailure || teamMailboxDiagnosticsInfo.Status == TeamMailboxSyncStatus.DocumentAndMaintenanceSyncFailure)
				{
					dataTable.Rows[0]["MaintenanceSyncStatus"] = OwaOptionClientStrings.TeamMailboxSyncError;
				}
				dataTable.Rows[0]["MembershipSyncDate"] = ((teamMailboxDiagnosticsInfo.MembershipSyncInfo == null) ? OwaOptionClientStrings.TeamMailboxSyncNotAvailable : ((DateTime)teamMailboxDiagnosticsInfo.MembershipSyncInfo.LastAttemptedSyncTime.Value).UtcToUserDateTimeString());
				dataTable.Rows[0]["MaintenanceSyncDate"] = ((teamMailboxDiagnosticsInfo.MaintenanceSyncInfo == null) ? OwaOptionClientStrings.TeamMailboxSyncNotAvailable : ((DateTime)teamMailboxDiagnosticsInfo.MaintenanceSyncInfo.LastAttemptedSyncTime.Value).UtcToUserDateTimeString());
				dataTable.Rows[0]["DocumentSyncDate"] = ((teamMailboxDiagnosticsInfo.HierarchySyncInfo == null) ? OwaOptionClientStrings.TeamMailboxSyncNotAvailable : ((DateTime)teamMailboxDiagnosticsInfo.HierarchySyncInfo.LastAttemptedSyncTime.Value).UtcToUserDateTimeString());
				dataTable.Rows[0]["SynchronizationDetails"] = teamMailboxDiagnosticsInfo.ToString();
				dataTable.Rows[0]["MembershipSyncStatus"] = OwaOptionClientStrings.TeamMailboxSyncStatus + dataTable.Rows[0]["MembershipSyncStatus"];
				dataTable.Rows[0]["MembershipSyncDate"] = OwaOptionClientStrings.TeamMailboxSyncDate + dataTable.Rows[0]["MembershipSyncDate"];
				dataTable.Rows[0]["MaintenanceSyncStatus"] = OwaOptionClientStrings.TeamMailboxSyncStatus + dataTable.Rows[0]["MaintenanceSyncStatus"];
				dataTable.Rows[0]["MaintenanceSyncDate"] = OwaOptionClientStrings.TeamMailboxSyncDate + dataTable.Rows[0]["MaintenanceSyncDate"];
				dataTable.Rows[0]["DocumentSyncStatus"] = OwaOptionClientStrings.TeamMailboxSyncStatus + dataTable.Rows[0]["DocumentSyncStatus"];
				dataTable.Rows[0]["DocumentSyncDate"] = OwaOptionClientStrings.TeamMailboxSyncDate + dataTable.Rows[0]["DocumentSyncDate"];
				dataTable.Rows[0]["TeamMailboxMembershipString1"] = OwaOptionClientStrings.TeamMailboxMembershipString1;
				dataTable.Rows[0]["TeamMailboxMembershipString2"] = OwaOptionClientStrings.TeamMailboxMembershipString2;
				dataTable.Rows[0]["TeamMailboxMembershipString3"] = OwaOptionClientStrings.TeamMailboxMembershipString3;
				dataTable.Rows[0]["TeamMailboxMembershipString4"] = OwaOptionClientStrings.TeamMailboxMembershipString4;
				dataTable.Rows[0]["TeamMailboxStartedMembershipSync"] = OwaOptionClientStrings.TeamMailboxStartedMembershipSync;
				dataTable.Rows[0]["TeamMailboxStartedMaintenanceSync"] = OwaOptionClientStrings.TeamMailboxStartedMaintenanceSync;
				dataTable.Rows[0]["TeamMailboxStartedDocumentSync"] = OwaOptionClientStrings.TeamMailboxStartedDocumentSync;
			}
		}

		private static bool IsExecutingUserAnOwner(object value)
		{
			if (value is IEnumerable<ADObjectId>)
			{
				IEnumerable<ADObjectId> enumerable = (IEnumerable<ADObjectId>)value;
				foreach (ADObjectId adobjectId in enumerable)
				{
					if (adobjectId.Equals(RbacPrincipal.Current.ExecutingUserId))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private static int GetUserCount(object value)
		{
			int num = 0;
			if (value is IEnumerable<ADObjectId>)
			{
				IEnumerable<ADObjectId> enumerable = (IEnumerable<ADObjectId>)value;
				foreach (ADObjectId adobjectId in enumerable)
				{
					num++;
				}
			}
			return num;
		}

		private const string TeamMailboxDiagnosticsInfo = "TeamMailboxDiagnosticsInfo";

		private const string MembershipSyncStatus = "MembershipSyncStatus";

		private const string MaintenanceSyncStatus = "MaintenanceSyncStatus";

		private const string DocumentSyncStatus = "DocumentSyncStatus";

		private const string MembershipSyncDate = "MembershipSyncDate";

		private const string MaintenanceSyncDate = "MaintenanceSyncDate";

		private const string DocumentSyncDate = "DocumentSyncDate";

		private const string SynchronizationDetails = "SynchronizationDetails";

		private const string TeamMailboxMembershipString1 = "TeamMailboxMembershipString1";

		private const string TeamMailboxMembershipString2 = "TeamMailboxMembershipString2";

		private const string TeamMailboxMembershipString3 = "TeamMailboxMembershipString3";

		private const string TeamMailboxMembershipString4 = "TeamMailboxMembershipString4";

		private const string TeamMailboxStartedMembershipSync = "TeamMailboxStartedMembershipSync";

		private const string TeamMailboxStartedMaintenanceSync = "TeamMailboxStartedMaintenanceSync";

		private const string TeamMailboxStartedDocumentSync = "TeamMailboxStartedDocumentSync";
	}
}
