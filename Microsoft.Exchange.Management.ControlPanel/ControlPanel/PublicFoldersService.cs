using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Permission;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.DDIService;
using Microsoft.Exchange.Management.RecipientTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class PublicFoldersService
	{
		public static string CurrentPath
		{
			get
			{
				return PublicFoldersService.currentPath;
			}
			set
			{
				PublicFoldersService.currentPath = value;
			}
		}

		public static void GetListPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			List<string> list = new List<string>();
			string value = "\\";
			if (inputRow["SearchText"] != DBNull.Value && !string.IsNullOrWhiteSpace((string)inputRow["SearchText"]))
			{
				value = (((string)inputRow["SearchText"]).StartsWith("\\") ? ((string)inputRow["SearchText"]) : ((string)inputRow["SearchText"]).Insert(0, "\\"));
				inputRow["Identity"] = value;
				list.Add("Identity");
			}
			PublicFoldersService.CurrentPath = value;
			if (list.Count > 0)
			{
				store.SetModifiedColumns(list);
			}
		}

		public static void GetObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			PublicFolder publicFolder = store.GetDataObject("PublicFolder") as PublicFolder;
			PublicFolderStatistics publicFolderStatistics = store.GetDataObject("PublicFolderStatistics") as PublicFolderStatistics;
			if (dataTable.Rows.Count == 1)
			{
				DataRow dataRow = dataTable.Rows[0];
				if (publicFolder != null)
				{
					dataRow["IsStorageQuotasSet"] = (publicFolder.IssueWarningQuota == null || (publicFolder.IssueWarningQuota.Value.IsUnlimited && publicFolder.ProhibitPostQuota.Value.IsUnlimited && publicFolder.MaxItemSize.Value.IsUnlimited));
					dataRow["IssueWarningQuota"] = MailboxPropertiesHelper.UnlimitedByteQuantifiedSizeToString(publicFolder.IssueWarningQuota);
					dataRow["ProhibitPostQuota"] = MailboxPropertiesHelper.UnlimitedByteQuantifiedSizeToString(publicFolder.ProhibitPostQuota);
					dataRow["MaxItemSize"] = MailboxPropertiesHelper.UnlimitedByteQuantifiedSizeToString(publicFolder.MaxItemSize);
					dataRow["IsRetainDeletedItemsForSet"] = (publicFolder.RetainDeletedItemsFor == null);
					dataRow["RetainDeletedItemsFor"] = ((publicFolder.RetainDeletedItemsFor != null) ? publicFolder.RetainDeletedItemsFor.Value.Days.ToString() : "5");
					dataRow["IsAgeLimitSet"] = (publicFolder.AgeLimit == null);
					dataRow["AgeLimit"] = ((publicFolder.AgeLimit != null && publicFolder.AgeLimit != null) ? publicFolder.AgeLimit.Value.Days.ToString() : "5");
				}
				if (publicFolderStatistics != null)
				{
					dataRow["TotalItemSize"] = publicFolderStatistics.TotalItemSize.ToMB().ToString();
					dataRow["TotalAssociatedItemSize"] = ((!publicFolderStatistics.TotalAssociatedItemSize.IsNullValue()) ? publicFolderStatistics.TotalAssociatedItemSize.ToMB() : 0UL);
					dataRow["TotalDeletedItemSize"] = ((!publicFolderStatistics.TotalDeletedItemSize.IsNullValue()) ? publicFolderStatistics.TotalDeletedItemSize.ToMB() : 0UL);
				}
			}
		}

		public static void SetObjectPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			List<string> list = new List<string>();
			if (DBNull.Value != inputRow["IssueWarningQuota"] || DBNull.Value != inputRow["ProhibitPostQuota"] || DBNull.Value != inputRow["MaxItemSize"] || DBNull.Value != inputRow["IsStorageQuotasSet"])
			{
				if (DBNull.Value != inputRow["IsStorageQuotasSet"] && (bool)inputRow["IsStorageQuotasSet"])
				{
					dataRow["IssueWarningQuota"] = Unlimited<ByteQuantifiedSize>.UnlimitedString;
					dataRow["ProhibitPostQuota"] = Unlimited<ByteQuantifiedSize>.UnlimitedString;
					dataRow["MaxItemSize"] = Unlimited<ByteQuantifiedSize>.UnlimitedString;
				}
				MailboxPropertiesHelper.SaveQuotaProperty(dataRow, "IsStorageQuotasSet", "IssueWarningQuota", list);
				MailboxPropertiesHelper.SaveQuotaProperty(dataRow, "IsStorageQuotasSet", "ProhibitPostQuota", list);
				MailboxPropertiesHelper.SaveQuotaProperty(dataRow, "IsStorageQuotasSet", "MaxItemSize", list);
			}
			if (DBNull.Value != inputRow["RetainDeletedItemsFor"] || DBNull.Value != inputRow["IsRetainDeletedItemsForSet"])
			{
				if (DBNull.Value != inputRow["IsRetainDeletedItemsForSet"] && (bool)inputRow["IsRetainDeletedItemsForSet"])
				{
					dataRow["RetainDeletedItemsFor"] = null;
				}
				else
				{
					dataRow["RetainDeletedItemsFor"] = EnhancedTimeSpan.Parse((string)dataRow["RetainDeletedItemsFor"]);
				}
				list.Add("RetainDeletedItemsFor");
				list.Add("IsRetainDeletedItemsForSet");
			}
			if (DBNull.Value != inputRow["AgeLimit"] || DBNull.Value != inputRow["IsAgeLimitSet"])
			{
				if (DBNull.Value != inputRow["IsAgeLimitSet"] && (bool)inputRow["IsAgeLimitSet"])
				{
					dataRow["AgeLimit"] = null;
				}
				else
				{
					dataRow["AgeLimit"] = EnhancedTimeSpan.Parse((string)dataRow["AgeLimit"]);
				}
				list.Add("AgeLimit");
				list.Add("IsAgeLimitSet");
			}
			MailboxPropertiesHelper.SetMaxSendReceiveSize(inputRow, dataTable, store);
			if (list.Count > 0)
			{
				store.SetModifiedColumns(list);
			}
		}

		public static void MailFlowSettingsPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			MailboxPropertiesHelper.GetMaxSendReceiveSize(inputRow, dataTable, store);
			MailboxPropertiesHelper.GetAcceptRejectSendersOrMembers(inputRow, dataTable, store);
		}

		public static void GetEmailAddresses(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			if (dataRow["SimpleEmailAddresses"] != DBNull.Value)
			{
				dataRow["EmailAddresses"] = EmailAddressList.FromProxyAddressCollection((ProxyAddressCollection)dataRow["SimpleEmailAddresses"]);
			}
		}

		public static void FilterEntSendAsPermission(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			IEnumerable<object> permissions = store.GetDataObject("ADPermissions") as IEnumerable<object>;
			dataTable.Rows[0]["SendAsPermissions"] = PublicFoldersService.FindRecipientsWithSendAsPermissionEnt(permissions, store);
		}

		internal static IEnumerable<AcePermissionRecipientRow> FindRecipientsWithSendAsPermissionEnt(IEnumerable<object> permissions, DataObjectStore store)
		{
			List<SecurityPrincipalIdParameter> permissionsHelper = PublicFoldersService.GetPermissionsHelper(permissions, new IsExpectedPermission(PublicFoldersService.IsSendAsPermission), store);
			return RecipientObjectResolver.Instance.ResolveSecurityPrincipalId(permissionsHelper);
		}

		internal static bool IsSendAsPermission(AcePresentationObject aceObject)
		{
			bool result = false;
			ADAcePresentationObject adacePresentationObject = aceObject as ADAcePresentationObject;
			if (adacePresentationObject.ExtendedRights != null)
			{
				foreach (ExtendedRightIdParameter extendedRightIdParameter in adacePresentationObject.ExtendedRights)
				{
					if (string.Compare(extendedRightIdParameter.ToString(), "send-as", true, CultureInfo.InvariantCulture) == 0)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		internal static List<SecurityPrincipalIdParameter> GetPermissionsHelper(IEnumerable<object> permissions, IsExpectedPermission isExpectedDelegate, DataObjectStore store)
		{
			List<SecurityIdentifier> list = new List<SecurityIdentifier>();
			List<SecurityIdentifier> list2 = new List<SecurityIdentifier>();
			List<SecurityPrincipalIdParameter> list3 = new List<SecurityPrincipalIdParameter>();
			if (store != null)
			{
				store.GetDataObject("MailPublicFolder");
			}
			if (permissions != null)
			{
				foreach (object obj in permissions)
				{
					AcePresentationObject acePresentationObject = obj as AcePresentationObject;
					if (acePresentationObject != null)
					{
						SecurityIdentifier securityIdentifier = acePresentationObject.User.SecurityIdentifier;
						if (!list2.Contains(securityIdentifier) && !list.Contains(securityIdentifier) && isExpectedDelegate(acePresentationObject))
						{
							if (acePresentationObject.Deny)
							{
								list2.Add(securityIdentifier);
							}
							else
							{
								list.Add(securityIdentifier);
								list3.Add(acePresentationObject.User);
							}
						}
					}
				}
			}
			return list3;
		}

		public static void FilterCloudSendAsPermission(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			dataTable.Rows[0]["SendAsPermissions"] = PublicFoldersService.FindRecipientsWithSendAsPermissionCloud(store.GetDataObject("RecipientPermission") as IEnumerable<object>);
		}

		internal static List<object> FindRecipientsWithSendAsPermissionCloud(IEnumerable<object> permissions)
		{
			List<object> list = new List<object>();
			foreach (object obj in permissions)
			{
				RecipientPermission recipientPermission = obj as RecipientPermission;
				if (recipientPermission.AccessRights.Contains(RecipientAccessRight.SendAs))
				{
					list.Add(new TrusteeRow(recipientPermission.Trustee));
				}
			}
			return list;
		}

		internal const string IsRetainDeletedItemsForSetColumnName = "IsRetainDeletedItemsForSet";

		internal const string RetainDeletedItemsForColumnName = "RetainDeletedItemsFor";

		internal const string IsAgeLimitSetColumnName = "IsAgeLimitSet";

		internal const string AgeLimitColumnName = "AgeLimit";

		internal const string IsStorageQuotasSetColumnName = "IsStorageQuotasSet";

		internal const string StorageQuotasColumnName = "StorageQuotas";

		internal const string MaxItemSizeColumnName = "MaxItemSize";

		internal const string ProhibitPostQuotaColumnName = "ProhibitPostQuota";

		internal const string IssueWarningQuotaColumnName = "IssueWarningQuota";

		internal const string TotalAssociatedItemSizeColumnName = "TotalAssociatedItemSize";

		internal const string TotalDeletedItemSizeColumnName = "TotalDeletedItemSize";

		internal const string TotalItemSizeColumnName = "TotalItemSize";

		internal const string SearchTextColumnName = "SearchText";

		internal const string DefaultAgeLimit = "5";

		internal const string DefaultRetainDeletedItemsForDays = "5";

		private static string currentPath = "\\";
	}
}
