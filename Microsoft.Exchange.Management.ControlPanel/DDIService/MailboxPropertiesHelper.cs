using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Permission;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.Management.RecipientTasks;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class MailboxPropertiesHelper
	{
		static MailboxPropertiesHelper()
		{
			if (EacEnvironment.Instance.IsDataCenter)
			{
				MailboxPropertiesHelper.GetListPropertySet = "PrimarySmtpAddress,DisplayName,RecipientTypeDetails,ArchiveGuid,AuthenticationType,Identity";
				MailboxPropertiesHelper.GetListWorkflowOutput = (DDIHelper.IsFFO() ? "DisplayName,PrimarySmtpAddress,Identity,MailboxType,RecipientTypeDetails,AuthenticationType,ArchiveGuid,IsUserManaged,IsKeepWindowsLiveIdAllowed,IsUserFederated,IsLinkedMailbox,IsRemoteMailbox,IsSharedMailbox,LocRecipientTypeDetails" : "DisplayName,PrimarySmtpAddress,Identity,MailboxType,RecipientTypeDetails,AuthenticationType,ArchiveGuid,IsUserManaged,IsKeepWindowsLiveIdAllowed,IsUserFederated,IsLinkedMailbox,IsRemoteMailbox,IsSharedMailbox");
				return;
			}
			MailboxPropertiesHelper.GetListPropertySet = "PrimarySmtpAddress,DisplayName,RecipientTypeDetails,ArchiveGuid,Identity";
			MailboxPropertiesHelper.GetListWorkflowOutput = "DisplayName,PrimarySmtpAddress,Identity,MailboxType,RecipientTypeDetails,IsLinkedMailbox,IsRemoteMailbox,IsSharedMailbox";
		}

		public static string UnlimitedByteQuantifiedSizeToString(object val)
		{
			if (!(val is Unlimited<ByteQuantifiedSize>))
			{
				return string.Empty;
			}
			Unlimited<ByteQuantifiedSize> unlimited = (Unlimited<ByteQuantifiedSize>)val;
			if (!unlimited.IsUnlimited)
			{
				return unlimited.Value.ToBytes().ToString();
			}
			return "unlimited";
		}

		public static MultiValuedPropertyBase ArrayToMvp(object val)
		{
			Array array = val as Array;
			if (array == null)
			{
				throw new ArgumentException("The value should be an array.", "val");
			}
			MultiValuedProperty<object> multiValuedProperty = new MultiValuedProperty<object>();
			foreach (object item in array)
			{
				multiValuedProperty.Add(item);
			}
			return multiValuedProperty;
		}

		public static void MailboxUsageGetObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			Mailbox mailbox = store.GetDataObject("Mailbox") as Mailbox;
			if (mailbox != null)
			{
				MailboxStatistics mailboxStatistics = store.GetDataObject("MailboxStatistics") as MailboxStatistics;
				MailboxDatabase mailboxDatabase = store.GetDataObject("MailboxDatabase") as MailboxDatabase;
				MailboxStatistics archiveStatistics = store.GetDataObject("ArchiveStatistics") as MailboxStatistics;
				MailboxPropertiesHelper.MailboxUsage mailboxUsage = new MailboxPropertiesHelper.MailboxUsage(mailbox, mailboxDatabase, mailboxStatistics, archiveStatistics);
				dataRow["MailboxUsage"] = new StatisticsBarData(mailboxUsage.MailboxUsagePercentage, mailboxUsage.MailboxUsageState, mailboxUsage.MailboxUsageText);
				if ((mailbox.UseDatabaseQuotaDefaults != null && mailbox.UseDatabaseQuotaDefaults.Value && mailboxDatabase != null && !Util.IsDataCenter) || !mailbox.ProhibitSendQuota.IsUnlimited)
				{
					dataRow["IsMailboxUsageUnlimited"] = false;
				}
				else
				{
					dataRow["IsMailboxUsageUnlimited"] = true;
				}
				dataRow["IssueWarningQuota"] = MailboxPropertiesHelper.UnlimitedByteQuantifiedSizeToString(mailboxUsage.IssueWarningQuota);
				dataRow["ProhibitSendQuota"] = MailboxPropertiesHelper.UnlimitedByteQuantifiedSizeToString(mailboxUsage.ProhibitSendQuota);
				dataRow["ProhibitSendReceiveQuota"] = MailboxPropertiesHelper.UnlimitedByteQuantifiedSizeToString(mailboxUsage.ProhibitSendReceiveQuota);
				dataRow["RetainDeletedItemsFor"] = mailboxUsage.RetainDeletedItemsFor.Days.ToString();
				dataRow["RetainDeletedItemsUntilBackup"] = mailboxUsage.RetainDeletedItemsUntilBackup;
			}
		}

		public static void MailboxFeaturePostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			Mailbox mailbox = store.GetDataObject("Mailbox") as Mailbox;
			if (mailbox != null)
			{
				MailboxStatistics mailboxStatistics = store.GetDataObject("MailboxStatistics") as MailboxStatistics;
				MailboxDatabase mbxDatabase = store.GetDataObject("MailboxDatabase") as MailboxDatabase;
				MailboxStatistics archiveStatistics = store.GetDataObject("ArchiveStatistics") as MailboxStatistics;
				new MailboxPropertiesHelper.MailboxUsage(mailbox, mbxDatabase, mailboxStatistics, archiveStatistics);
				MailboxPropertiesHelper.GetMailboxPostAction(inputRow, dataTable, store);
				MailboxPropertiesHelper.GetArchiveStatisticsPostAction(inputRow, dataTable, store);
			}
			dataRow["RawIdentity"] = ((ADObjectId)dataRow["Identity"]).ToString();
			MailboxPropertiesHelper.GetMaxSendReceiveSize(inputRow, dataTable, store);
			MailboxPropertiesHelper.GetAcceptRejectSendersOrMembers(inputRow, dataTable, store);
			Unlimited<EnhancedTimeSpan> unlimited = (Unlimited<EnhancedTimeSpan>)dataRow["LitigationHoldDuration"];
			if (unlimited.IsUnlimited)
			{
				dataRow["LitigationHoldDuration"] = "unlimited";
			}
			else
			{
				dataRow["LitigationHoldDuration"] = unlimited.Value.Days.ToString();
			}
			Unlimited<int> unlimited2 = (Unlimited<int>)dataRow["RecipientLimits"];
			if (unlimited2.IsUnlimited)
			{
				dataRow["RecipientLimits"] = "unlimited";
				return;
			}
			dataRow["RecipientLimits"] = unlimited2.Value.ToString();
		}

		public static void SetObjectPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			List<string> list = new List<string>();
			Unlimited<EnhancedTimeSpan> unlimited = Unlimited<EnhancedTimeSpan>.UnlimitedValue;
			if (!DBNull.Value.Equals(dataRow["LitigationHoldDuration"]) && (string)dataRow["LitigationHoldDuration"] != "unlimited" && ((string)dataRow["LitigationHoldDuration"]).Trim() != string.Empty)
			{
				list.Add("LitigationHoldDuration");
				unlimited = Unlimited<EnhancedTimeSpan>.Parse((string)dataRow["LitigationHoldDuration"]);
				store.SetModifiedColumns(list);
			}
			inputRow["LitigationHoldDuration"] = unlimited;
			dataRow["LitigationHoldDuration"] = unlimited;
			Unlimited<int> unlimited2 = Unlimited<int>.UnlimitedValue;
			if (!DBNull.Value.Equals(dataRow["RecipientLimits"]) && (string)dataRow["RecipientLimits"] != "unlimited")
			{
				list.Add("RecipientLimits");
				unlimited2 = Unlimited<int>.Parse((string)dataRow["RecipientLimits"]);
				store.SetModifiedColumns(list);
			}
			inputRow["RecipientLimits"] = unlimited2;
			dataRow["RecipientLimits"] = unlimited2;
			MailboxPropertiesHelper.SetMaxSendReceiveSize(inputRow, dataTable, store);
			MailboxPropertiesHelper.SetRetentionPolicy(inputRow, dataTable, store);
			List<string> list2 = new List<string>();
			MailboxPropertiesHelper.SaveQuotaProperty(dataRow, "UseDatabaseQuotaDefaults", "IssueWarningQuota", list2);
			MailboxPropertiesHelper.SaveQuotaProperty(dataRow, "UseDatabaseQuotaDefaults", "ProhibitSendQuota", list2);
			MailboxPropertiesHelper.SaveQuotaProperty(dataRow, "UseDatabaseQuotaDefaults", "ProhibitSendReceiveQuota", list2);
			if (DBNull.Value != dataRow["RetainDeletedItemsFor"])
			{
				dataRow["RetainDeletedItemsFor"] = EnhancedTimeSpan.Parse((string)dataRow["RetainDeletedItemsFor"]);
				list2.Add("RetainDeletedItemsFor");
			}
			if (list2.Count != 0)
			{
				store.SetModifiedColumns(list2);
			}
		}

		public static void GetSuggestionPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			MailboxPropertiesHelper.FilterNoSmtpEmailAddresses(inputRow, dataTable, store);
		}

		public static void GetDisconnectedMailboxListPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			dataTable.BeginLoadData();
			List<DataRow> list = new List<DataRow>();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (!DBNull.Value.Equals(dataRow["DisconnectReason"]) && (dataRow["DisconnectReason"] == null || (MailboxState)dataRow["DisconnectReason"] != MailboxState.Disabled))
				{
					list.Add(dataRow);
				}
			}
			foreach (DataRow row in list)
			{
				dataTable.Rows.Remove(row);
			}
			dataTable.EndLoadData();
		}

		public static void GetMailboxPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow row = dataTable.Rows[0];
			Mailbox mailbox = store.GetDataObject("Mailbox") as Mailbox;
			if (mailbox != null)
			{
				MailboxPropertiesHelper.TrySetColumnValue(row, "MailboxCanHaveArchive", mailbox.ExchangeVersion.CompareTo(ExchangeObjectVersion.Exchange2010) >= 0 && ((mailbox.RecipientType == RecipientType.UserMailbox && mailbox.RecipientTypeDetails != RecipientTypeDetails.LegacyMailbox) || mailbox.RecipientTypeDetails == (RecipientTypeDetails)((ulong)int.MinValue) || mailbox.RecipientTypeDetails == RecipientTypeDetails.RemoteRoomMailbox || mailbox.RecipientTypeDetails == RecipientTypeDetails.RemoteEquipmentMailbox || mailbox.RecipientTypeDetails == RecipientTypeDetails.RemoteSharedMailbox));
				MailboxPropertiesHelper.TrySetColumnValue(row, "EnableArchive", mailbox.ArchiveState != ArchiveState.None);
				MailboxPropertiesHelper.TrySetColumnValue(row, "HasArchive", mailbox.ArchiveState != ArchiveState.None);
				MailboxPropertiesHelper.TrySetColumnValue(row, "RemoteArchive", mailbox.ArchiveState == ArchiveState.HostedProvisioned || mailbox.ArchiveState == ArchiveState.HostedPending);
			}
		}

		internal static void TrySetColumnValue(DataRow row, string column, object value)
		{
			if (row.Table.Columns.Contains(column))
			{
				row[column] = value;
			}
		}

		public static void GetArchiveStatisticsPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			Mailbox mailbox = store.GetDataObject("Mailbox") as Mailbox;
			MailboxDatabase mbxDatabase = store.GetDataObject("MailboxDatabase") as MailboxDatabase;
			MailboxStatistics archiveStatistics = store.GetDataObject("ArchiveStatistics") as MailboxStatistics;
			if (mailbox != null)
			{
				MailboxPropertiesHelper.MailboxUsage mailboxUsage = new MailboxPropertiesHelper.MailboxUsage(mailbox, mbxDatabase, null, archiveStatistics);
				dataRow["ArchiveUsage"] = new StatisticsBarData(mailboxUsage.ArchiveUsagePercentage, mailboxUsage.ArchiveUsageState, mailboxUsage.ArchiveUsageText);
				dataRow["MailboxQuota"] = MailboxPropertiesHelper.UnlimitedByteQuantifiedSizeToString(mailboxUsage.ProhibitSendQuota);
			}
		}

		public static void SaveQuotaProperty(DataRow row, string isDefaultColumnName, string quotaPropertyColumnName, List<string> modifiedQuotaColumns)
		{
			if (DBNull.Value != row[quotaPropertyColumnName])
			{
				string text = (string)row[quotaPropertyColumnName];
				if (string.Equals(text.Trim(), Unlimited<ByteQuantifiedSize>.UnlimitedString, StringComparison.OrdinalIgnoreCase))
				{
					row[quotaPropertyColumnName] = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
				}
				else
				{
					row[quotaPropertyColumnName] = Unlimited<ByteQuantifiedSize>.Parse(text);
				}
				modifiedQuotaColumns.Add(quotaPropertyColumnName);
			}
		}

		public static void GetListPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			bool flag = RbacCheckerWrapper.RbacChecker.IsInRole("Remove-Mailbox?KeepWindowsLiveId@W:Organization");
			bool flag2 = !Util.IsDataCenter;
			dataTable.BeginLoadData();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)dataRow["RecipientTypeDetails"];
				bool flag3 = recipientTypeDetails == RecipientTypeDetails.UserMailbox;
				MailboxPropertiesHelper.FillTypeColumns(dataRow, recipientTypeDetails);
				if (flag2)
				{
					bool flag4 = false;
					bool flag5 = false;
					if (!DBNull.Value.Equals(dataRow["AuthenticationType"]))
					{
						flag4 = ((AuthenticationType)dataRow["AuthenticationType"] == AuthenticationType.Managed);
						flag5 = ((AuthenticationType)dataRow["AuthenticationType"] == AuthenticationType.Federated);
					}
					dataRow["IsUserManaged"] = (flag3 && flag4);
					dataRow["IsUserFederated"] = (flag3 && flag5);
					dataRow["IsKeepWindowsLiveIdAllowed"] = flag;
					dataRow["MailboxType"] = MailboxPropertiesHelper.TranslateMailboxTypeForListview(recipientTypeDetails, flag3 && flag5, (Guid)dataRow["ArchiveGuid"]);
				}
				else
				{
					dataRow["MailboxType"] = MailboxPropertiesHelper.TranslateMailboxTypeForListview(recipientTypeDetails, false, (Guid)dataRow["ArchiveGuid"]);
				}
			}
			dataTable.EndLoadData();
		}

		public static void GetRecipientMailboxType(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)dataRow["RecipientTypeDetails"];
			bool flag = recipientTypeDetails == RecipientTypeDetails.UserMailbox;
			bool flag2 = false;
			if (!DBNull.Value.Equals(dataRow["AuthenticationType"]))
			{
				flag2 = ((AuthenticationType)dataRow["AuthenticationType"] == AuthenticationType.Federated);
			}
			dataRow["MailboxType"] = MailboxPropertiesHelper.TranslateMailboxTypeForListview(recipientTypeDetails, flag && flag2, (Guid)dataRow["ArchiveGuid"]);
		}

		public static void GetRecipientPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)dataRow["RecipientTypeDetails"];
			MailboxPropertiesHelper.FillTypeColumns(dataRow, recipientTypeDetails);
			MailboxPropertiesHelper.GetRecipientMailboxType(inputRow, dataTable, store);
		}

		private static void FillTypeColumns(DataRow row, RecipientTypeDetails recipientTypeDetails)
		{
			row["IsRemoteMailbox"] = (recipientTypeDetails == RecipientTypeDetails.RemoteEquipmentMailbox || recipientTypeDetails == RecipientTypeDetails.RemoteRoomMailbox || recipientTypeDetails == (RecipientTypeDetails)((ulong)int.MinValue) || recipientTypeDetails == RecipientTypeDetails.RemoteSharedMailbox);
			row["IsSharedMailbox"] = (recipientTypeDetails == RecipientTypeDetails.SharedMailbox);
			row["IsLinkedMailbox"] = (recipientTypeDetails == RecipientTypeDetails.LinkedMailbox);
		}

		public static string TranslateMailboxTypeForListview(RecipientTypeDetails recipientTypeDetails, bool isFederatedUserMailbox, Guid archiveGuid)
		{
			string text = string.Empty;
			if (recipientTypeDetails <= RecipientTypeDetails.EquipmentMailbox)
			{
				if (recipientTypeDetails <= RecipientTypeDetails.LegacyMailbox)
				{
					if (recipientTypeDetails <= RecipientTypeDetails.SharedMailbox)
					{
						if (recipientTypeDetails < RecipientTypeDetails.UserMailbox)
						{
							goto IL_155;
						}
						switch ((int)(recipientTypeDetails - RecipientTypeDetails.UserMailbox))
						{
						case 0:
							text = (isFederatedUserMailbox ? Strings.FederatedUserMailboxText : Strings.UserMailboxText);
							goto IL_15B;
						case 1:
							text = Strings.LinkedMailboxText;
							goto IL_15B;
						case 2:
							goto IL_155;
						case 3:
							text = Strings.SharedMailboxText;
							goto IL_15B;
						}
					}
					if (recipientTypeDetails == RecipientTypeDetails.LegacyMailbox)
					{
						text = Strings.LegacyMailboxText;
						goto IL_15B;
					}
				}
				else
				{
					if (recipientTypeDetails == RecipientTypeDetails.RoomMailbox)
					{
						text = Strings.RoomMailboxText;
						goto IL_15B;
					}
					if (recipientTypeDetails == RecipientTypeDetails.EquipmentMailbox)
					{
						text = Strings.EquipmentMailboxText;
						goto IL_15B;
					}
				}
			}
			else if (recipientTypeDetails <= RecipientTypeDetails.RemoteRoomMailbox)
			{
				if (recipientTypeDetails == (RecipientTypeDetails)((ulong)-2147483648))
				{
					text = Strings.RemoteUserMailboxText;
					goto IL_15B;
				}
				if (recipientTypeDetails == RecipientTypeDetails.RemoteRoomMailbox)
				{
					text = Strings.RemoteRoomMailboxText;
					goto IL_15B;
				}
			}
			else
			{
				if (recipientTypeDetails == RecipientTypeDetails.RemoteEquipmentMailbox)
				{
					text = Strings.RemoteEquipmentMailboxText;
					goto IL_15B;
				}
				if (recipientTypeDetails == RecipientTypeDetails.RemoteSharedMailbox)
				{
					text = Strings.RemoteSharedMailboxText;
					goto IL_15B;
				}
				if (recipientTypeDetails == RecipientTypeDetails.TeamMailbox)
				{
					text = Strings.TeamMailboxText;
					goto IL_15B;
				}
			}
			IL_155:
			text = string.Empty;
			IL_15B:
			return archiveGuid.Equals(Guid.Empty) ? text : string.Format(Strings.ArchiveText, text);
		}

		public static void FilterEntSendAsPermission(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			dataTable.Rows[0]["SendAsPermissions"] = MailboxPropertiesHelper.FindRecipientsWithSendAsPermissionEnt(store.GetDataObject("ADPermissions") as IEnumerable<object>, store);
		}

		internal static IEnumerable<AcePermissionRecipientRow> FindRecipientsWithSendAsPermissionEnt(IEnumerable<object> permissions, DataObjectStore store)
		{
			List<SecurityPrincipalIdParameter> permissionsHelper = MailboxPropertiesHelper.GetPermissionsHelper(permissions, new IsExpectedPermission(MailboxPropertiesHelper.IsSendAsPermission), store);
			return RecipientObjectResolver.Instance.ResolveSecurityPrincipalId(permissionsHelper);
		}

		public static void FilterCloudSendAsPermission(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			dataTable.Rows[0]["SendAsPermissions"] = MailboxPropertiesHelper.FindRecipientsWithSendAsPermissionCloud(store.GetDataObject("RecipientPermission") as IEnumerable<object>);
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

		public static void FilterFullAccessPermission(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			IEnumerable<object> permissions = store.GetDataObject("MailboxPermissions") as IEnumerable<object>;
			dataTable.Rows[0]["FullAccessPermissions"] = MailboxPropertiesHelper.GetPermissionsHelper(permissions, new IsExpectedPermission(MailboxPropertiesHelper.IsFullAccessPermission), store);
		}

		public static void SetADPermissionPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			object[] array = inputRow["SendAsPermissionsAdded"] as object[];
			Identity[] array2;
			if (array == null)
			{
				array2 = new Identity[0];
			}
			else
			{
				array2 = Array.ConvertAll<object, Identity>(array, (object x) => x as Identity);
			}
			Identity[] array3 = array2;
			object[] array4 = inputRow["SendAsPermissionsRemoved"] as object[];
			Identity[] array5;
			if (array4 == null)
			{
				array5 = new Identity[0];
			}
			else
			{
				array5 = Array.ConvertAll<object, Identity>(array4, (object x) => x as Identity);
			}
			Identity[] array6 = array5;
			inputRow["SendAsPermissionsAdded"] = RecipientObjectResolver.Instance.ConvertGuidsToSid(Array.ConvertAll<Identity, string>(array3, (Identity x) => x.RawIdentity));
			inputRow["SendAsPermissionsRemoved"] = RecipientObjectResolver.Instance.ConvertGuidsToSid(Array.ConvertAll<Identity, string>(array6, (Identity x) => x.RawIdentity));
		}

		public static void SetMailboxPermissionPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			IEnumerable<object> allPermissions = store.GetDataObject("MailboxPermissions") as IEnumerable<object>;
			object[] array = inputRow["FullAccessPermissionsAdded"] as object[];
			Identity[] array2;
			if (array == null)
			{
				array2 = new Identity[0];
			}
			else
			{
				array2 = Array.ConvertAll<object, Identity>(array, (object x) => x as Identity);
			}
			Identity[] identitiesToAdd = array2;
			object[] array3 = inputRow["FullAccessPermissionsRemoved"] as object[];
			Identity[] array4;
			if (array3 == null)
			{
				array4 = new Identity[0];
			}
			else
			{
				array4 = Array.ConvertAll<object, Identity>(array3, (object x) => x as Identity);
			}
			Identity[] identitiesToRemove = array4;
			List<SecurityIdentifier> value;
			List<SecurityIdentifier> value2;
			Dictionary<SecurityIdentifier, bool> value3;
			Dictionary<SecurityIdentifier, bool> value4;
			Dictionary<SecurityIdentifier, bool> value5;
			MailboxPropertiesHelper.SetPermissionsHelper(allPermissions, identitiesToAdd, identitiesToRemove, out value, out value2, out value3, out value4, out value5, new IsExpectedPermission(MailboxPropertiesHelper.IsFullAccessPermission));
			inputRow["FullAccessPermissionsAdded"] = value;
			inputRow["FullAccessPermissionsRemoved"] = value2;
			inputRow["hasExplicitDenyForAdded"] = value3;
			inputRow["hasExplicitAllowForRemoved"] = value4;
			inputRow["hasInheritedAllowForRemoved"] = value5;
		}

		internal static List<SecurityPrincipalIdParameter> GetPermissionsHelper(IEnumerable<object> permissions, IsExpectedPermission isExpectedDelegate, DataObjectStore store)
		{
			List<SecurityIdentifier> list = new List<SecurityIdentifier>();
			List<SecurityIdentifier> list2 = new List<SecurityIdentifier>();
			List<SecurityPrincipalIdParameter> list3 = new List<SecurityPrincipalIdParameter>();
			string text = null;
			if (store != null)
			{
				Mailbox mailbox = store.GetDataObject("Mailbox") as Mailbox;
				if (mailbox.IsLinked)
				{
					text = mailbox.LinkedMasterAccount;
				}
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
								if (text == null || text != acePresentationObject.User.ToString())
								{
									list3.Add(acePresentationObject.User);
								}
							}
						}
					}
				}
			}
			return list3;
		}

		internal static bool IsFullAccessPermission(AcePresentationObject aceObject)
		{
			MailboxAcePresentationObject mailboxAcePresentationObject = aceObject as MailboxAcePresentationObject;
			foreach (MailboxRights mailboxRights in mailboxAcePresentationObject.AccessRights)
			{
				if ((mailboxRights & MailboxRights.FullAccess) == MailboxRights.FullAccess)
				{
					return true;
				}
			}
			return false;
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

		internal static void SetPermissionsHelper(IEnumerable<object> allPermissions, Identity[] identitiesToAdd, Identity[] identitiesToRemove, out List<SecurityIdentifier> sidsToAdd, out List<SecurityIdentifier> sidsToRemove, out Dictionary<SecurityIdentifier, bool> hasExplicitDenyForAdded, out Dictionary<SecurityIdentifier, bool> hasExplicitAllowForRemoved, out Dictionary<SecurityIdentifier, bool> hasInheritedAllowForRemoved, IsExpectedPermission isExpectedDelegate)
		{
			new List<string>();
			hasExplicitAllowForRemoved = new Dictionary<SecurityIdentifier, bool>();
			hasInheritedAllowForRemoved = new Dictionary<SecurityIdentifier, bool>();
			hasExplicitDenyForAdded = new Dictionary<SecurityIdentifier, bool>();
			sidsToAdd = RecipientObjectResolver.Instance.ConvertGuidsToSid(Array.ConvertAll<Identity, string>(identitiesToAdd, (Identity x) => x.RawIdentity));
			foreach (SecurityIdentifier key in sidsToAdd)
			{
				hasExplicitDenyForAdded[key] = false;
			}
			sidsToRemove = RecipientObjectResolver.Instance.ConvertGuidsToSid(Array.ConvertAll<Identity, string>(identitiesToRemove, (Identity x) => x.RawIdentity));
			foreach (SecurityIdentifier key2 in sidsToRemove)
			{
				hasExplicitAllowForRemoved[key2] = false;
				hasInheritedAllowForRemoved[key2] = false;
			}
			foreach (object obj in allPermissions)
			{
				AcePresentationObject acePresentationObject = (AcePresentationObject)obj;
				SecurityIdentifier securityIdentifier = acePresentationObject.User.SecurityIdentifier;
				bool flag = false;
				if (hasExplicitDenyForAdded.TryGetValue(securityIdentifier, out flag))
				{
					if (!flag && acePresentationObject.Deny.ToBool() && !acePresentationObject.IsInherited && isExpectedDelegate(acePresentationObject))
					{
						hasExplicitDenyForAdded[securityIdentifier] = true;
					}
				}
				else if (sidsToRemove.Contains(securityIdentifier) && isExpectedDelegate(acePresentationObject) && !acePresentationObject.Deny)
				{
					if (acePresentationObject.IsInherited)
					{
						hasInheritedAllowForRemoved[securityIdentifier] = true;
					}
					else
					{
						hasExplicitAllowForRemoved[securityIdentifier] = true;
					}
				}
			}
		}

		public static bool HasExplicitPermission(object sid, object dictionary)
		{
			SecurityIdentifier key = sid as SecurityIdentifier;
			Dictionary<SecurityIdentifier, bool> dictionary2 = dictionary as Dictionary<SecurityIdentifier, bool>;
			bool flag = false;
			return dictionary2.TryGetValue(key, out flag) && flag;
		}

		public static void AddToAllowList(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			MobileMailboxService.AddToAllowList(row, dataTable, store);
		}

		public static void AddToBlockList(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			MobileMailboxService.AddToBlockList(row, dataTable, store);
		}

		internal static void GetAcceptRejectSendersOrMembers(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			if (dataRow["AcceptMessagesOnlyFromSendersOrMembers"] != DBNull.Value)
			{
				ADMultiValuedProperty<ADObjectId> admultiValuedProperty = (ADMultiValuedProperty<ADObjectId>)dataRow["AcceptMessagesOnlyFromSendersOrMembers"];
				if (admultiValuedProperty != null && admultiValuedProperty.Count == 0)
				{
					dataRow["AcceptMessagesOnlyFromSendersOrMembers"] = null;
				}
			}
			if (dataRow["RejectMessagesFromSendersOrMembers"] != DBNull.Value)
			{
				ADMultiValuedProperty<ADObjectId> admultiValuedProperty2 = (ADMultiValuedProperty<ADObjectId>)dataRow["RejectMessagesFromSendersOrMembers"];
				if (admultiValuedProperty2 != null && admultiValuedProperty2.Count == 0)
				{
					dataRow["RejectMessagesFromSendersOrMembers"] = null;
				}
			}
		}

		internal static void GetMaxSendReceiveSize(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			if (dataRow["MaxSendSize"] != DBNull.Value)
			{
				Unlimited<ByteQuantifiedSize> unlimited = (Unlimited<ByteQuantifiedSize>)dataRow["MaxSendSize"];
				if (unlimited.IsUnlimited)
				{
					dataRow["MaxSendSize"] = null;
				}
				else
				{
					dataRow["MaxSendSize"] = unlimited.Value.ToKB().ToString();
				}
			}
			if (dataRow["MaxReceiveSize"] != DBNull.Value)
			{
				Unlimited<ByteQuantifiedSize> unlimited2 = (Unlimited<ByteQuantifiedSize>)dataRow["MaxReceiveSize"];
				if (unlimited2.IsUnlimited)
				{
					dataRow["MaxReceiveSize"] = null;
					return;
				}
				dataRow["MaxReceiveSize"] = unlimited2.Value.ToKB().ToString();
			}
		}

		internal static void SetMaxSendReceiveSize(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			List<string> list = new List<string>();
			Unlimited<ByteQuantifiedSize> unlimited = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
			Unlimited<ByteQuantifiedSize> unlimited2 = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
			if (!DBNull.Value.Equals(dataRow["MaxSendSize"]))
			{
				list.Add("MaxSendSize");
				unlimited = Unlimited<ByteQuantifiedSize>.Parse((string)dataRow["MaxSendSize"], ByteQuantifiedSize.Quantifier.KB);
				store.SetModifiedColumns(list);
			}
			if (!DBNull.Value.Equals(dataRow["MaxReceiveSize"]))
			{
				list.Add("MaxReceiveSize");
				unlimited2 = Unlimited<ByteQuantifiedSize>.Parse((string)dataRow["MaxReceiveSize"], ByteQuantifiedSize.Quantifier.KB);
				store.SetModifiedColumns(list);
			}
			inputRow["MaxSendSize"] = unlimited;
			dataRow["MaxSendSize"] = unlimited;
			inputRow["MaxReceiveSize"] = unlimited2;
			dataRow["MaxReceiveSize"] = unlimited2;
		}

		public static void GetCountryOrRegion(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			User user = store.GetDataObject("User") as User;
			if (user != null && null != user.CountryOrRegion)
			{
				dataRow["CountryOrRegion"] = user.CountryOrRegion.Name;
			}
		}

		public static void GetEmailAddresses(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			if (dataRow["SimpleEmailAddresses"] != DBNull.Value)
			{
				dataRow["EmailAddresses"] = EmailAddressList.FromProxyAddressCollection((ProxyAddressCollection)dataRow["SimpleEmailAddresses"]);
			}
			MailboxPropertiesHelper.UpdateCanSetABP(dataRow, store);
		}

		internal static void FilterNoSmtpEmailAddresses(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (dataRow["EmailAddresses"] != DBNull.Value)
				{
					ProxyAddressCollection proxyAddressCollection = new ProxyAddressCollection();
					foreach (ProxyAddress proxyAddress in ((ProxyAddressCollection)dataRow["EmailAddresses"]))
					{
						if (proxyAddress.PrefixString.ToLower() == "smtp")
						{
							try
							{
								proxyAddressCollection.Add(proxyAddress);
							}
							catch (DataValidationException)
							{
							}
						}
					}
					dataRow["EmailAddresses"] = proxyAddressCollection;
				}
			}
		}

		internal static void SetRetentionPolicy(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			if (!DBNull.Value.Equals(dataRow["RetentionPolicy"]) && string.IsNullOrEmpty((string)dataRow["RetentionPolicy"]))
			{
				inputRow["RetentionPolicy"] = null;
				dataRow["RetentionPolicy"] = null;
			}
		}

		internal static void UpdateCanSetABP(DataRow row, DataObjectStore store)
		{
			Mailbox mailbox = store.GetDataObject("Mailbox") as Mailbox;
			if (mailbox != null)
			{
				MailboxPropertiesHelper.TrySetColumnValue(row, "MailboxCanSetABP", mailbox.RecipientTypeDetails != RecipientTypeDetails.LegacyMailbox && mailbox.ExchangeVersion.CompareTo(ExchangeObjectVersion.Exchange2010) >= 0);
			}
		}

		public static void GetServersWithVersionPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DatabasePropertiesHelper.FilterRowsByAdminDisplayVersion(inputRow, dataTable, store, Server.CurrentExchangeMajorVersion, null);
		}

		public static void SetRecipientFilterPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (EacEnvironment.Instance.IsForefrontForOffice)
			{
				inputRow["RecipientTypeFilter"] = inputRow["FFORecipientTypeFilter"].ToString();
			}
		}

		public static void ViewFFOUserDetailsPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			ADObjectId adobjectId = (ADObjectId)dataRow["Identity"];
			List<Identity> list = new List<Identity>();
			if (!dataRow["Group"].IsNullValue())
			{
				List<ADObjectId> list2 = (List<ADObjectId>)dataRow["Group"];
				foreach (ADObjectId identity in list2)
				{
					list.Add(new Identity(identity));
				}
				dataRow["GroupNames"] = list.ToArray();
				return;
			}
			dataRow["GroupNames"] = string.Empty;
		}

		public static void FetchHoldNames(DataRow inputRow, DataTable dataTable, DataObjectStore store, PowerShellResults[] results)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)dataRow["InPlaceHolds"];
			MultiValuedProperty<string> multiValuedProperty2 = new MultiValuedProperty<string>();
			if (multiValuedProperty.Count > 0)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (store.GetDataObject("MailboxSearchWholeObject") != null)
				{
					IEnumerable<object> enumerable = store.GetDataObject("MailboxSearchWholeObject") as IEnumerable<object>;
					if (enumerable != null)
					{
						foreach (object obj in enumerable)
						{
							MailboxSearchObject mailboxSearchObject = (MailboxSearchObject)obj;
							if (!string.IsNullOrEmpty(mailboxSearchObject.InPlaceHoldIdentity) && !dictionary.ContainsKey(mailboxSearchObject.InPlaceHoldIdentity))
							{
								dictionary.Add(mailboxSearchObject.InPlaceHoldIdentity, mailboxSearchObject.Name);
							}
						}
					}
				}
				if (dictionary.Count > 0)
				{
					foreach (string key in multiValuedProperty)
					{
						if (dictionary.ContainsKey(key))
						{
							multiValuedProperty2.Add(dictionary[key]);
						}
					}
				}
			}
			dataRow["InPlaceHoldNames"] = multiValuedProperty2;
		}

		public static string GetHoldDescription(object holdIds)
		{
			string result = Strings.MailboxUnderNoHold;
			MultiValuedProperty<string> multiValuedProperty = holdIds as MultiValuedProperty<string>;
			if (multiValuedProperty.Count == 1)
			{
				result = Strings.MailboxUnderOneHold;
			}
			else if (multiValuedProperty.Count > 1)
			{
				result = string.Format(Strings.MailboxUnderMultipleHolds, multiValuedProperty.Count);
			}
			return result;
		}

		public static string ResolveExtendedOrgnaizationUnit(object ou)
		{
			Identity identity = ou as Identity;
			string result;
			if (null != identity)
			{
				result = identity.DisplayName;
			}
			else
			{
				result = (ou as string);
			}
			return result;
		}

		internal const string DatacenterPropertySet = "PrimarySmtpAddress,DisplayName,RecipientTypeDetails,ArchiveGuid,AuthenticationType,Identity";

		internal const string DatacenterGetListOutput = "DisplayName,PrimarySmtpAddress,Identity,MailboxType,RecipientTypeDetails,AuthenticationType,ArchiveGuid,IsUserManaged,IsKeepWindowsLiveIdAllowed,IsUserFederated,IsLinkedMailbox,IsRemoteMailbox,IsSharedMailbox";

		internal const string FFODatacenterGetListOutput = "DisplayName,PrimarySmtpAddress,Identity,MailboxType,RecipientTypeDetails,AuthenticationType,ArchiveGuid,IsUserManaged,IsKeepWindowsLiveIdAllowed,IsUserFederated,IsLinkedMailbox,IsRemoteMailbox,IsSharedMailbox,LocRecipientTypeDetails";

		internal const string EnterprisePropertySet = "PrimarySmtpAddress,DisplayName,RecipientTypeDetails,ArchiveGuid,Identity";

		internal const string EnterpriseGetListOutput = "DisplayName,PrimarySmtpAddress,Identity,MailboxType,RecipientTypeDetails,IsLinkedMailbox,IsRemoteMailbox,IsSharedMailbox";

		private const string MailboxObjectName = "Mailbox";

		private const string RecipientObjectName = "Recipient";

		private const string UserObjectName = "User";

		private const string MailboxDatabaseObjectName = "MailboxDatabase";

		private const string MailboxStatisticsObjectName = "MailboxStatistics";

		private const string MailboxUsageColumnName = "MailboxUsage";

		private const string CountryOrRegionColumnName = "CountryOrRegion";

		private const string WarningQuotaColumnName = "IssueWarningQuota";

		private const string ProhibitSendQuotaColumnName = "ProhibitSendQuota";

		private const string ProhibitSendReceiveQuotaColumnName = "ProhibitSendReceiveQuota";

		private const string RetentionDaysColumnName = "RetainDeletedItemsFor";

		private const string RetainUntilBackUpColumnName = "RetainDeletedItemsUntilBackup";

		private const string UseDatabaseQuotaDefaultsColumnName = "UseDatabaseQuotaDefaults";

		private const string UseDatabaseRetentionDefaultsColumnName = "UseDatabaseRetentionDefaults";

		private const string MailboxUsageUnlimitedColumnName = "IsMailboxUsageUnlimited";

		private const string MailboxSearchWholeObject = "MailboxSearchWholeObject";

		private const string MailboxQuotaColumnName = "MailboxQuota";

		private const string MailboxCanHaveArchiveColumnName = "MailboxCanHaveArchive";

		private const string MailboxCanSetABPColumnName = "MailboxCanSetABP";

		private const string EnableArchiveColumnName = "EnableArchive";

		private const string HasArchiveColumnName = "HasArchive";

		private const string RemoteArchiveColumnName = "RemoteArchive";

		private const string ArchiveStatisticsObjectName = "ArchiveStatistics";

		private const string ArchiveUsageColumnName = "ArchiveUsage";

		private const string ArchiveNameColumnName = "ArchiveName";

		private const string LitigationHoldDurationColumnName = "LitigationHoldDuration";

		private static readonly string[] PasswordParameter = new string[]
		{
			"Password"
		};

		public static readonly string GetListPropertySet = null;

		public static readonly string GetListWorkflowOutput = null;

		public static readonly string NewSetObjectWorkflowOutput = "IsLinkedMailbox,IsRemoteMailbox,IsSharedMailbox,DisplayName,MailboxType,PrimarySmtpAddress,Identity,RecipientTypeDetails,Name,FirstName,LastName,EmailAddressesTxt,ExchangeVersion,RecipientType,Alias,OrganizationalUnit,DatabaseTxt,ArchiveStateTxt,SharingPolicy,ActiveSyncMailboxPolicyTxt,WhenChanged,EmailAddressPolicyEnabled,ArchiveDatabaseTxt,City,Company,CountryOrRegion,Department,HiddenFromAddressListsEnabled,LitigationHoldEnabled,Office,OwaMailboxPolicyTxt,Phone,PostalCode,RetentionPolicy,StateOrProvince,Title,UMEnabled,UMMailboxPolicy,CustomAttribute1,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15";

		public static readonly string NewObjectWorkflowOutputForLinkedMailbox = "DisplayName,MailboxType,PrimarySmtpAddress,Identity,RecipientTypeDetails,Name,FirstName,LastName,EmailAddressesTxt,ExchangeVersion,RecipientType,Alias,OrganizationalUnit,DatabaseTxt,ArchiveStateTxt,SharingPolicy,ActiveSyncMailboxPolicyTxt,WhenChanged,EmailAddressPolicyEnabled,IsLinkedMailbox";

		public static readonly string ConsoleSmallSet = "DisplayName,Alias,OrganizationalUnit,PrimarySmtpAddress,EmailAddresses,HiddenFromAddressListsEnabled,Name,WhenChanged,City,Company,CountryOrRegion,DatabaseName,Department,ExpansionServer,ExternalEmailAddress,FirstName,LastName,Office,StateOrProvince,Title,UMEnabled,HasActiveSyncDevicePartnership,Manager,SharingPolicy,ArchiveGuid,IsValidSecurityPrincipal,ArchiveState,MailboxMoveTargetMDB,MailboxMoveSourceMDB,MailboxMoveFlags,MailboxMoveRemoteHostName,MailboxMoveBatchName,MailboxMoveStatus,RecipientType,RecipientTypeDetails,Identity,ExchangeVersion,OrganizationId";

		public class MailboxUsage
		{
			public MailboxUsage(Mailbox mbx, MailboxDatabase mbxDatabase, MailboxStatistics mailboxStatistics, MailboxStatistics archiveStatistics)
			{
				this.mailbox = mbx;
				this.mailboxDatabase = mbxDatabase;
				this.mailboxStatistics = mailboxStatistics;
				this.archiveStatistics = archiveStatistics;
			}

			private bool UseDatabaseQuotaDefaults
			{
				get
				{
					return this.mailbox.UseDatabaseQuotaDefaults != null && this.mailbox.UseDatabaseQuotaDefaults.Value && this.mailboxDatabase != null && !Util.IsDataCenter;
				}
			}

			private bool UseDatabaseRetentionDefaults
			{
				get
				{
					return this.mailbox.UseDatabaseRetentionDefaults && this.mailboxDatabase != null && !Util.IsDataCenter;
				}
			}

			public EnhancedTimeSpan RetainDeletedItemsFor
			{
				get
				{
					if (!this.UseDatabaseRetentionDefaults)
					{
						return this.mailbox.RetainDeletedItemsFor;
					}
					return this.mailboxDatabase.DeletedItemRetention;
				}
			}

			public bool RetainDeletedItemsUntilBackup
			{
				get
				{
					if (!this.UseDatabaseRetentionDefaults)
					{
						return this.mailbox.RetainDeletedItemsUntilBackup;
					}
					return this.mailboxDatabase.RetainDeletedItemsUntilBackup;
				}
			}

			public Unlimited<ByteQuantifiedSize> ProhibitSendQuota
			{
				get
				{
					if (!this.UseDatabaseQuotaDefaults)
					{
						return this.mailbox.ProhibitSendQuota;
					}
					return this.mailboxDatabase.ProhibitSendQuota;
				}
			}

			public Unlimited<ByteQuantifiedSize> ProhibitSendReceiveQuota
			{
				get
				{
					if (!this.UseDatabaseQuotaDefaults)
					{
						return this.mailbox.ProhibitSendReceiveQuota;
					}
					return this.mailboxDatabase.ProhibitSendReceiveQuota;
				}
			}

			public Unlimited<ByteQuantifiedSize> IssueWarningQuota
			{
				get
				{
					if (!this.UseDatabaseQuotaDefaults)
					{
						return this.mailbox.IssueWarningQuota;
					}
					return this.mailboxDatabase.IssueWarningQuota;
				}
			}

			public uint MailboxUsagePercentage
			{
				get
				{
					return MailboxPropertiesHelper.MailboxUsage.CalculateUsagePercentage(this.mailboxStatistics, this.ProhibitSendQuota);
				}
			}

			public uint ArchiveUsagePercentage
			{
				get
				{
					return MailboxPropertiesHelper.MailboxUsage.CalculateUsagePercentage(this.archiveStatistics, this.mailbox.ArchiveQuota);
				}
			}

			public StatisticsBarState MailboxUsageState
			{
				get
				{
					return MailboxPropertiesHelper.MailboxUsage.CalculateUsageState(this.mailboxStatistics, this.ProhibitSendQuota, this.IssueWarningQuota);
				}
			}

			public StatisticsBarState ArchiveUsageState
			{
				get
				{
					return MailboxPropertiesHelper.MailboxUsage.CalculateUsageState(this.archiveStatistics, this.mailbox.ArchiveQuota, this.mailbox.ArchiveWarningQuota);
				}
			}

			public string MailboxUsageText
			{
				get
				{
					return MailboxPropertiesHelper.MailboxUsage.BuildUsageText(this.mailboxStatistics, this.ProhibitSendQuota);
				}
			}

			public string ArchiveUsageText
			{
				get
				{
					return MailboxPropertiesHelper.MailboxUsage.BuildUsageText(this.archiveStatistics, this.mailbox.ArchiveQuota);
				}
			}

			private static uint CalculateUsagePercentage(MailboxStatistics statistics, Unlimited<ByteQuantifiedSize> quota)
			{
				if (statistics == null)
				{
					return 0U;
				}
				if (!quota.IsUnlimited)
				{
					return (uint)(statistics.TotalItemSize.Value.ToBytes() / quota.Value.ToBytes() * 100.0);
				}
				if (statistics.TotalItemSize.IsUnlimited)
				{
					return 100U;
				}
				if (statistics.TotalItemSize.Value.ToBytes() > 0UL)
				{
					return 3U;
				}
				return 0U;
			}

			private static StatisticsBarState CalculateUsageState(MailboxStatistics statistics, Unlimited<ByteQuantifiedSize> quota, Unlimited<ByteQuantifiedSize> warningQuota)
			{
				if (statistics != null)
				{
					if (statistics.StorageLimitStatus != null)
					{
						if (statistics.StorageLimitStatus == StorageLimitStatus.ProhibitSend || statistics.StorageLimitStatus == StorageLimitStatus.MailboxDisabled)
						{
							return StatisticsBarState.Exceeded;
						}
						if (statistics.StorageLimitStatus == StorageLimitStatus.IssueWarning)
						{
							return StatisticsBarState.Warning;
						}
					}
					else
					{
						if (!quota.IsUnlimited && statistics.TotalItemSize.Value.ToBytes() > quota.Value.ToBytes())
						{
							return StatisticsBarState.Exceeded;
						}
						if (!warningQuota.IsUnlimited && statistics.TotalItemSize.Value.ToBytes() > warningQuota.Value.ToBytes())
						{
							return StatisticsBarState.Warning;
						}
					}
				}
				return StatisticsBarState.Normal;
			}

			private static string BuildUsageText(MailboxStatistics statistics, Unlimited<ByteQuantifiedSize> quota)
			{
				string format = string.Empty;
				if (quota.IsUnlimited)
				{
					format = Strings.MailboxUsageWithUnlimitedText;
				}
				else
				{
					format = Strings.MailboxUsageText;
				}
				if (statistics == null)
				{
					return string.Format(format, new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(0UL)).ToAppropriateUnitFormatString(), MailboxPropertiesHelper.MailboxUsage.CalculateUsagePercentage(statistics, quota), quota.ToAppropriateUnitFormatString());
				}
				return string.Format(format, statistics.TotalItemSize.ToAppropriateUnitFormatString(), MailboxPropertiesHelper.MailboxUsage.CalculateUsagePercentage(statistics, quota), quota.ToAppropriateUnitFormatString());
			}

			private Mailbox mailbox;

			private MailboxDatabase mailboxDatabase;

			private MailboxStatistics archiveStatistics;

			private MailboxStatistics mailboxStatistics;
		}
	}
}
