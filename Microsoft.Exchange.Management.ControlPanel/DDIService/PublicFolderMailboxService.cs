using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class PublicFolderMailboxService
	{
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
				Unlimited<ByteQuantifiedSize> maxReceiveSize = mailbox.MaxReceiveSize;
				if (maxReceiveSize.IsUnlimited)
				{
					dataRow["MaxReceiveSize"] = "unlimited";
				}
				else
				{
					dataRow["MaxReceiveSize"] = PublicFolderMailboxService.UnlimitedByteQuantifiedSizeToString(maxReceiveSize);
				}
				dataRow["IssueWarningQuota"] = PublicFolderMailboxService.UnlimitedByteQuantifiedSizeToString(mailboxUsage.IssueWarningQuota);
				dataRow["ProhibitSendQuota"] = PublicFolderMailboxService.UnlimitedByteQuantifiedSizeToString(mailboxUsage.ProhibitSendQuota);
				dataRow["ProhibitSendReceiveQuota"] = PublicFolderMailboxService.UnlimitedByteQuantifiedSizeToString(mailboxUsage.ProhibitSendReceiveQuota);
				dataRow["RetainDeletedItemsFor"] = mailboxUsage.RetainDeletedItemsFor.Days.ToString();
				dataRow["RetainDeletedItemsUntilBackup"] = mailboxUsage.RetainDeletedItemsUntilBackup;
			}
		}

		public static void SetObjectPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			List<string> list = new List<string>();
			PublicFolderMailboxService.SaveQuotaProperty(dataRow, "UseDatabaseQuotaDefaults", "IssueWarningQuota", list);
			PublicFolderMailboxService.SaveQuotaProperty(dataRow, "UseDatabaseQuotaDefaults", "ProhibitSendReceiveQuota", list);
			Unlimited<ByteQuantifiedSize> unlimited = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
			if (!DBNull.Value.Equals(dataRow["MaxReceiveSize"]))
			{
				list.Add("MaxReceiveSize");
				unlimited = Unlimited<ByteQuantifiedSize>.Parse((string)dataRow["MaxReceiveSize"]);
				store.SetModifiedColumns(list);
			}
			inputRow["MaxReceiveSize"] = unlimited;
			dataRow["MaxReceiveSize"] = unlimited;
			if (list.Count != 0)
			{
				store.SetModifiedColumns(list);
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

		private const string MailboxObjectName = "Mailbox";

		private const string MailboxStatisticsObjectName = "MailboxStatistics";

		private const string MailboxDatabaseObjectName = "MailboxDatabase";

		private const string ArchiveStatisticsObjectName = "ArchiveStatistics";

		private const string MailboxUsageColumnName = "MailboxUsage";

		private const string MailboxUsageUnlimitedColumnName = "IsMailboxUsageUnlimited";

		private const string WarningQuotaColumnName = "IssueWarningQuota";

		private const string ProhibitSendQuotaColumnName = "ProhibitSendQuota";

		private const string ProhibitSendReceiveQuotaColumnName = "ProhibitSendReceiveQuota";

		private const string RetentionDaysColumnName = "RetainDeletedItemsFor";

		private const string RetainUntilBackUpColumnName = "RetainDeletedItemsUntilBackup";

		private const string MaxReceiveSizeMailbox = "MaxReceiveSize";

		private const string UseDatabaseQuotaDefaultsColumnName = "UseDatabaseQuotaDefaults";
	}
}
