using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class DatabasePropertiesHelper
	{
		public static void GetObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			if (!DBNull.Value.Equals(dataRow["IssueWarningQuota"]))
			{
				Unlimited<ByteQuantifiedSize> unlimited = (Unlimited<ByteQuantifiedSize>)dataRow["IssueWarningQuota"];
				dataRow["IssueWarningQuota"] = MailboxPropertiesHelper.UnlimitedByteQuantifiedSizeToString(unlimited);
			}
			if (!DBNull.Value.Equals(dataRow["ProhibitSendQuota"]))
			{
				Unlimited<ByteQuantifiedSize> unlimited2 = (Unlimited<ByteQuantifiedSize>)dataRow["ProhibitSendQuota"];
				dataRow["ProhibitSendQuota"] = MailboxPropertiesHelper.UnlimitedByteQuantifiedSizeToString(unlimited2);
			}
			if (!DBNull.Value.Equals(dataRow["ProhibitSendReceiveQuota"]))
			{
				Unlimited<ByteQuantifiedSize> unlimited3 = (Unlimited<ByteQuantifiedSize>)dataRow["ProhibitSendReceiveQuota"];
				dataRow["ProhibitSendReceiveQuota"] = MailboxPropertiesHelper.UnlimitedByteQuantifiedSizeToString(unlimited3);
			}
			dataRow["MountStatus"] = ((DDIHelper.IsEmptyValue(dataRow["Mounted"]) || !(bool)dataRow["Mounted"]) ? Strings.StatusDismounted : Strings.StatusMounted);
			if (!DBNull.Value.Equals(dataRow["DeletedItemRetention"]))
			{
				dataRow["DeletedItemRetention"] = ((EnhancedTimeSpan)dataRow["DeletedItemRetention"]).Days.ToString();
			}
			if (!DBNull.Value.Equals(dataRow["MailboxRetention"]))
			{
				dataRow["MailboxRetention"] = ((EnhancedTimeSpan)dataRow["MailboxRetention"]).Days.ToString();
			}
			MailboxDatabase mailboxDatabase = store.GetDataObject("MailboxDatabase") as MailboxDatabase;
			if (mailboxDatabase != null)
			{
				dataRow["Servers"] = mailboxDatabase.Servers;
			}
			if (!DBNull.Value.Equals(dataRow["MaintenanceSchedule"]))
			{
				Schedule schedule = (Schedule)dataRow["MaintenanceSchedule"];
				dataRow["MaintenanceSchedule"] = new ScheduleBuilder(schedule).GetEntireState();
			}
			if (!DBNull.Value.Equals(dataRow["QuotaNotificationSchedule"]))
			{
				Schedule schedule2 = (Schedule)dataRow["QuotaNotificationSchedule"];
				dataRow["QuotaNotificationSchedule"] = new ScheduleBuilder(schedule2).GetEntireState();
			}
		}

		public static void OnPreAddDatabaseCopy(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			if (!DBNull.Value.Equals(dataRow["ReplayLagTime"]) && !DDIHelper.IsEmptyValue(dataRow["ReplayLagTime"]))
			{
				inputRow["ReplayLagTime"] = ((string)dataRow["ReplayLagTime"]).FromTimeSpan(TimeUnit.Day);
			}
			else
			{
				inputRow["ReplayLagTime"] = "0".FromTimeSpan(TimeUnit.Day);
			}
			store.SetModifiedColumns(new List<string>
			{
				"ReplayLagTime"
			});
		}

		public static void SetObjectPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			List<string> list = new List<string>();
			MailboxPropertiesHelper.SaveQuotaProperty(dataRow, null, "IssueWarningQuota", list);
			MailboxPropertiesHelper.SaveQuotaProperty(dataRow, null, "ProhibitSendQuota", list);
			MailboxPropertiesHelper.SaveQuotaProperty(dataRow, null, "ProhibitSendReceiveQuota", list);
			if (DBNull.Value != dataRow["DeletedItemRetention"])
			{
				dataRow["DeletedItemRetention"] = EnhancedTimeSpan.Parse((string)dataRow["DeletedItemRetention"]);
				list.Add("DeletedItemRetention");
			}
			if (DBNull.Value != dataRow["MailboxRetention"])
			{
				dataRow["MailboxRetention"] = EnhancedTimeSpan.Parse((string)dataRow["MailboxRetention"]);
				list.Add("MailboxRetention");
			}
			DatabasePropertiesHelper.SetScheduleProperty(dataRow, "MaintenanceSchedule", list);
			DatabasePropertiesHelper.SetScheduleProperty(dataRow, "QuotaNotificationSchedule", list);
			if (list.Count != 0)
			{
				store.SetModifiedColumns(list);
			}
		}

		public static void GetMailboxServerPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			int majorVersion = 0;
			string text = inputRow["Version"] as string;
			if (text == "current")
			{
				majorVersion = Server.CurrentExchangeMajorVersion;
			}
			else
			{
				int.TryParse(text, out majorVersion);
			}
			DatabasePropertiesHelper.FilterRowsByAdminDisplayVersion(inputRow, dataTable, store, majorVersion, null);
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (DBNull.Value != dataRow["DataPath"])
				{
					LocalLongFullPath localLongFullPath = (LocalLongFullPath)dataRow["DataPath"];
					dataRow["DataPath"] = localLongFullPath.PathName;
				}
			}
		}

		public static void GetServerToAddCopyPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DatabasePropertiesHelper.FilterRowsByAdminDisplayVersion(inputRow, dataTable, store, Server.CurrentExchangeMajorVersion, new Func<DataRow, DataRow, DataObjectStore, bool>(DatabasePropertiesHelper.FindServerInSameDagButDontHaveTheDatabase));
		}

		private static bool FindServerInSameDagButDontHaveTheDatabase(DataRow inputRow, DataRow row, DataObjectStore store)
		{
			if (DBNull.Value.Equals(row["DatabaseAvailabilityGroup"]))
			{
				return false;
			}
			MailboxDatabase mailboxDatabase = (MailboxDatabase)store.GetDataObject("MailboxDatabase");
			string b = (string)row["DatabaseAvailabilityGroup"];
			string serverName = (string)row["Name"];
			return mailboxDatabase.MasterServerOrAvailabilityGroup.Name == b && !mailboxDatabase.Servers.Any((ADObjectId x) => serverName.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
		}

		public static void GetServerForSeedingPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DatabasePropertiesHelper.FilterRowsByAdminDisplayVersion(inputRow, dataTable, store, Server.CurrentExchangeMajorVersion, new Func<DataRow, DataRow, DataObjectStore, bool>(DatabasePropertiesHelper.FindServerInSameDatabaseButNotCurrentServer));
		}

		public static void GetServerToManageDAGMember(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DatabasePropertiesHelper.FilterRowsByAdminDisplayVersion(inputRow, dataTable, store, Server.CurrentExchangeMajorVersion, new Func<DataRow, DataRow, DataObjectStore, bool>(DatabasePropertiesHelper.FindServerInSameDagOrNotAnyDag));
		}

		public static void GetServersForSwitchOver(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DatabasePropertiesHelper.FilterRowsByAdminDisplayVersion(inputRow, dataTable, store, Server.CurrentExchangeMajorVersion, new Func<DataRow, DataRow, DataObjectStore, bool>(DatabasePropertiesHelper.FindServerToSwichover));
		}

		public static void FilterRecoveryDatabase(DataTable dataTable)
		{
			List<DataRow> list = new List<DataRow>();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (!DBNull.Value.Equals(dataRow["Recovery"]) && (bool)dataRow["Recovery"])
				{
					list.Add(dataRow);
				}
			}
			foreach (DataRow row in list)
			{
				dataTable.Rows.Remove(row);
			}
		}

		public static void GetMailboxDatabasePostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DatabasePropertiesHelper.FilterRecoveryDatabase(dataTable);
			DatabasePropertiesHelper.FilterRowsByAdminDisplayVersion(inputRow, dataTable, store, 0, new Func<DataRow, DataRow, DataObjectStore, bool>(DatabasePropertiesHelper.IsAdminDisplayVersionE14OrAfter));
		}

		private static bool FindServerInSameDagOrNotAnyDag(DataRow inputRow, DataRow row, DataObjectStore store)
		{
			Guid empty = Guid.Empty;
			if (!Guid.TryParse(inputRow["DAGId"].ToString(), out empty))
			{
				return false;
			}
			object obj = row["DatabaseAvailabilityGroupRawValue"];
			if (DBNull.Value.Equals(obj))
			{
				return true;
			}
			if (((ADObjectId)obj).ObjectGuid.Equals(empty))
			{
				row["DatabaseAvailabilityGroup"] = obj.ToString();
				return true;
			}
			return false;
		}

		private static bool FindServerInSameDatabaseButNotCurrentServer(DataRow inputRow, DataRow row, DataObjectStore store)
		{
			if (DBNull.Value.Equals(row["DatabaseAvailabilityGroup"]))
			{
				return false;
			}
			MailboxDatabase mailboxDatabase = (MailboxDatabase)store.GetDataObject("MailboxDatabase");
			string b = (string)row["DatabaseAvailabilityGroup"];
			string text = (string)row["Name"];
			ADObjectId currentServerId = (ADObjectId)row["Identity"];
			string value = (string)inputRow["CurrentServer"];
			return mailboxDatabase.MasterServerOrAvailabilityGroup.Name == b && mailboxDatabase.Servers.Any((ADObjectId serverId) => serverId.Equals(currentServerId)) && !text.Equals(value, StringComparison.OrdinalIgnoreCase);
		}

		private static bool FindServerToSwichover(DataRow inputRow, DataRow row, DataObjectStore store)
		{
			if (store.GetDataObject("MailboxDatabaseWholeObject") == null)
			{
				return false;
			}
			HashSet<Guid> hashSet = null;
			if (DBNull.Value.Equals(inputRow["ServerSetForSwitchoverPicker"]))
			{
				hashSet = new HashSet<Guid>();
				new Dictionary<Guid, int>();
				IEnumerable<object> enumerable = store.GetDataObject("MailboxDatabaseWholeObject") as IEnumerable<object>;
				if (enumerable != null)
				{
					foreach (object obj in enumerable)
					{
						MailboxDatabase mailboxDatabase = obj as MailboxDatabase;
						foreach (ADObjectId adobjectId in mailboxDatabase.Servers)
						{
							if (!hashSet.Contains(adobjectId.ObjectGuid))
							{
								hashSet.Add(adobjectId.ObjectGuid);
							}
						}
					}
				}
				inputRow["ServerSetForSwitchoverPicker"] = hashSet;
			}
			else
			{
				hashSet = (inputRow["ServerSetForSwitchoverPicker"] as HashSet<Guid>);
			}
			if (hashSet != null && hashSet.Count > 0)
			{
				ADObjectId adobjectId2 = row["Identity"] as ADObjectId;
				string value = (string)inputRow["CurrentServer"];
				return adobjectId2 != null && hashSet.Contains(adobjectId2.ObjectGuid) && !adobjectId2.ObjectGuid.ToString().Equals(value, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		private static bool IsAdminDisplayVersionE14OrAfter(DataRow inputRow, DataRow row, DataObjectStore store)
		{
			ServerVersion serverVersion = (ServerVersion)row["AdminDisplayVersion"];
			return serverVersion.Major >= Server.Exchange2009MajorVersion;
		}

		private static void FilterMailboxServerRows(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DatabasePropertiesHelper.FilterRowsByAdminDisplayVersion(inputRow, dataTable, store, Server.CurrentExchangeMajorVersion, null);
		}

		internal static void FilterRowsByAdminDisplayVersion(DataRow inputRow, DataTable dataTable, DataObjectStore store, int majorVersion, Func<DataRow, DataRow, DataObjectStore, bool> predicate)
		{
			List<DataRow> list = new List<DataRow>();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				ServerVersion serverVersion = (ServerVersion)dataRow["AdminDisplayVersion"];
				if ((majorVersion != 0 && serverVersion.Major != majorVersion) || (predicate != null && !predicate(inputRow, dataRow, store)))
				{
					list.Add(dataRow);
				}
			}
			foreach (DataRow row in list)
			{
				dataTable.Rows.Remove(row);
			}
		}

		private static void SetScheduleProperty(DataRow row, string scheduleColumnName, List<string> modifiedColumns)
		{
			if (DBNull.Value != row[scheduleColumnName])
			{
				object[] array = (object[])row[scheduleColumnName];
				bool[] array2 = new bool[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = (bool)array[i];
				}
				ScheduleBuilder scheduleBuilder = new ScheduleBuilder();
				scheduleBuilder.SetEntireState(array2);
				row[scheduleColumnName] = scheduleBuilder.Schedule;
				modifiedColumns.Add(scheduleColumnName);
			}
		}

		public static void GetMailboxCopyPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			MailboxDatabase mailboxDatabase = (MailboxDatabase)store.GetDataObject("MailboxDatabaseFoGet");
			DatabaseCopyStatusEntry databaseCopyStatusEntry = (DatabaseCopyStatusEntry)store.GetDataObject("DatabaseCopyStatusEntry");
			DatabaseCopy databaseCopy = null;
			foreach (DatabaseCopy databaseCopy2 in mailboxDatabase.DatabaseCopies)
			{
				if (string.Equals(databaseCopyStatusEntry.MailboxServer, databaseCopy2.HostServerName, StringComparison.InvariantCultureIgnoreCase))
				{
					databaseCopy = databaseCopy2;
					break;
				}
			}
			if (databaseCopy != null)
			{
				dataRow["ActivationPreference"] = databaseCopy.ActivationPreference;
				dataRow["DatabaseCopiesLength"] = mailboxDatabase.DatabaseCopies.Length;
				dataRow["ReplayLagTime"] = databaseCopy.ReplayLagTime.ToString(TimeUnit.Day, 9);
			}
		}

		public static void GetForSDODBStatusPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			List<object> source = (List<object>)dataRow["DatabaseCopies"];
			MailboxDatabase mailboxDatabase = (MailboxDatabase)store.GetDataObject("MailboxDatabase");
			List<object> list = new List<object>();
			KeyValuePair<ADObjectId, int>[] activationPreference = mailboxDatabase.ActivationPreference;
			for (int i = 0; i < activationPreference.Length; i++)
			{
				KeyValuePair<ADObjectId, int> pref = activationPreference[i];
				list.Add(source.First(delegate(object x)
				{
					string mailboxServer = ((DatabaseCopyStatusEntry)x).MailboxServer;
					KeyValuePair<ADObjectId, int> pref = pref;
					return mailboxServer.Equals(pref.Key.Name, StringComparison.OrdinalIgnoreCase);
				}));
			}
			dataRow["DatabaseCopies"] = list;
		}

		public static void PrepareDBCopyForSDO(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DatabaseCopyStatusEntry statusEntry = (DatabaseCopyStatusEntry)store.GetDataObject("DatabaseCopyStatusEntry");
			DatabaseCopyStatus databaseCopyStatus = new DatabaseCopyStatus(statusEntry);
			DataRow dataRow = dataTable.Rows[0];
			dataRow["CanActivate"] = databaseCopyStatus.CanActivate;
			dataRow["CanRemove"] = databaseCopyStatus.CanRemove;
			dataRow["CanResume"] = databaseCopyStatus.CanResume;
			dataRow["CanSuspend"] = databaseCopyStatus.CanSuspend;
			dataRow["CanUpdate"] = databaseCopyStatus.CanUpdate;
			dataRow["ContentIndexStateString"] = databaseCopyStatus.ContentIndexStateString;
			dataRow["IsActive"] = databaseCopyStatus.IsActive;
			dataRow["StatusString"] = databaseCopyStatus.StatusString;
		}

		public static void GetFirstObjectPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			if (dataRow["Identity"] is IEnumerable<Identity>)
			{
				inputRow["FirstDatabase"] = ((Identity[])dataRow["Identity"])[0];
			}
			else
			{
				inputRow["FirstDatabase"] = dataRow["Identity"];
			}
			store.SetModifiedColumns(new List<string>
			{
				"FirstDatabase"
			});
		}

		private const string IssueWarningQuotaColumnName = "IssueWarningQuota";

		private const string ProhibitSendQuotaColumnName = "ProhibitSendQuota";

		private const string ProhibitSendReceiveQuotaColumnName = "ProhibitSendReceiveQuota";

		private const string DeletedItemRetentionColumnName = "DeletedItemRetention";

		private const string MailboxRetentionColumnName = "MailboxRetention";

		private const string ServersColumnName = "Servers";

		private const string MountedColumnName = "Mounted";

		private const string MountStatusColumnName = "MountStatus";

		private const string MaintenanceScheduleColumnName = "MaintenanceSchedule";

		private const string QuotaNotificationScheduleColumnName = "QuotaNotificationSchedule";

		private const string DatabaseCopiesColumnName = "DatabaseCopies";

		private const string AdminDisplayVersionColumnName = "AdminDisplayVersion";

		private const string MailboxDatabaseObjectName = "MailboxDatabase";

		private const string MailboxDatabaseWholeObject = "MailboxDatabaseWholeObject";

		private const string DatabaseAvailabilityGroupColumnName = "DatabaseAvailabilityGroup";

		private const string DatabaseAvailabilityGroupRawValueColumnName = "DatabaseAvailabilityGroupRawValue";

		private const string ServerSetColumnName = "ServerSetForSwitchoverPicker";

		private const string ReplayLagTime = "ReplayLagTime";
	}
}
