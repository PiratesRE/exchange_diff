using System;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class RemoteMailboxPropertiesHelper
	{
		public static void GetObjectPostAction(DataRow inputRow, DataTable table, DataObjectStore store)
		{
			DataRow dataRow = table.Rows[0];
			if (dataRow["SimpleEmailAddresses"] != DBNull.Value)
			{
				dataRow["EmailAddresses"] = EmailAddressList.FromProxyAddressCollection((ProxyAddressCollection)dataRow["SimpleEmailAddresses"]);
			}
			if (dataRow["ObjectCountryOrRegion"] != DBNull.Value)
			{
				dataRow["CountryOrRegion"] = ((CountryInfo)dataRow["ObjectCountryOrRegion"]).Name;
			}
			MailboxPropertiesHelper.GetMaxSendReceiveSize(inputRow, table, store);
			MailboxPropertiesHelper.GetAcceptRejectSendersOrMembers(inputRow, table, store);
			if (dataRow["EmailAddresses"] != DBNull.Value && dataRow["RemoteRoutingAddress"] != DBNull.Value)
			{
				EmailAddressList emailAddressList = (EmailAddressList)dataRow["EmailAddresses"];
				string strA = (string)dataRow["RemoteRoutingAddress"];
				foreach (EmailAddressItem emailAddressItem in emailAddressList)
				{
					string identity = emailAddressItem.Identity;
					if (string.Compare(strA, identity, true) == 0)
					{
						dataRow["RemoteRoutingAddress"] = identity;
						break;
					}
				}
			}
			dataRow["IsRemoteUserMailbox"] = ((RecipientTypeDetails)((ulong)int.MinValue)).Equals(dataRow["RecipientTypeDetails"]);
		}

		public static void GetRecipientPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			RemoteMailboxPropertiesHelper.GetObjectPostAction(inputRow, dataTable, store);
			MailboxPropertiesHelper.GetRecipientPostAction(inputRow, dataTable, store);
		}

		public static void SetObjectPreAction(DataRow inputRow, DataTable table, DataObjectStore store)
		{
			MailboxPropertiesHelper.SetMaxSendReceiveSize(inputRow, table, store);
		}

		public static void GetRemoteMailboxPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow row = dataTable.Rows[0];
			RemoteMailbox remoteMailbox = store.GetDataObject("RemoteMailbox") as RemoteMailbox;
			if (remoteMailbox != null)
			{
				MailboxPropertiesHelper.TrySetColumnValue(row, "MailboxCanHaveArchive", remoteMailbox.ExchangeVersion.CompareTo(ExchangeObjectVersion.Exchange2010) >= 0 && (remoteMailbox.RecipientTypeDetails == (RecipientTypeDetails)((ulong)int.MinValue) || remoteMailbox.RecipientTypeDetails == RecipientTypeDetails.RemoteRoomMailbox || remoteMailbox.RecipientTypeDetails == RecipientTypeDetails.RemoteEquipmentMailbox || remoteMailbox.RecipientTypeDetails == RecipientTypeDetails.RemoteSharedMailbox));
				MailboxPropertiesHelper.TrySetColumnValue(row, "EnableArchive", remoteMailbox.ArchiveState != ArchiveState.None);
				MailboxPropertiesHelper.TrySetColumnValue(row, "HasArchive", remoteMailbox.ArchiveState != ArchiveState.None);
				MailboxPropertiesHelper.TrySetColumnValue(row, "RemoteArchive", remoteMailbox.ArchiveState == ArchiveState.HostedProvisioned || remoteMailbox.ArchiveState == ArchiveState.HostedPending);
			}
		}

		private const string RemoteMailboxObjectName = "RemoteMailbox";

		private const string MailboxCanHaveArchiveColumnName = "MailboxCanHaveArchive";

		private const string EnableArchiveColumnName = "EnableArchive";

		private const string HasArchiveColumnName = "HasArchive";

		private const string RemoteArchiveColumnName = "RemoteArchive";
	}
}
