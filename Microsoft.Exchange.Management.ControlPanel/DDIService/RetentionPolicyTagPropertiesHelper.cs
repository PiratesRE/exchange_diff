using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class RetentionPolicyTagPropertiesHelper
	{
		public static void GetListPostAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			List<DataRow> list = new List<DataRow>();
			dataTable.BeginLoadData();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				ElcFolderType elcFolderType = (ElcFolderType)dataRow["Type"];
				if (elcFolderType == ElcFolderType.ManagedCustomFolder || elcFolderType == ElcFolderType.RecoverableItems)
				{
					list.Add(dataRow);
				}
				else
				{
					EnhancedTimeSpan? ageLimitForRetention = dataRow["AgeLimitForRetention"].IsNullValue() ? null : ((EnhancedTimeSpan?)dataRow["AgeLimitForRetention"]);
					dataRow["FolderType"] = RetentionUtils.GetLocalizedType((ElcFolderType)dataRow["Type"]);
					dataRow["RetentionDays"] = RetentionPolicyTagPropertiesHelper.GetRetentionDays(ageLimitForRetention, (bool)dataRow["RetentionEnabled"]);
					dataRow["RetentionPeriodDays"] = RetentionPolicyTagPropertiesHelper.GetRetentionPeriodDays(ageLimitForRetention, (bool)dataRow["RetentionEnabled"]);
					dataRow["RetentionPolicyActionType"] = RetentionUtils.GetLocalizedRetentionActionType((RetentionActionType)dataRow["RetentionAction"]);
				}
			}
			foreach (DataRow row in list)
			{
				dataTable.Rows.Remove(row);
			}
			dataTable.EndLoadData();
		}

		public static void GetForSDOPostAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			EnhancedTimeSpan? ageLimitForRetention = dataRow["AgeLimitForRetention"].IsNullValue() ? null : ((EnhancedTimeSpan?)dataRow["AgeLimitForRetention"]);
			dataRow["FolderType"] = RetentionUtils.GetLocalizedType((ElcFolderType)dataRow["Type"]);
			dataRow["RetentionPeriodDetail"] = RetentionPolicyTagPropertiesHelper.GetRetentionPeriodDetail(ageLimitForRetention, (bool)dataRow["RetentionEnabled"]);
			dataRow["RetentionActionDescription"] = RetentionPolicyTagPropertiesHelper.GetLocalizedRetentionAction((bool)dataRow["RetentionEnabled"], (RetentionActionType)dataRow["RetentionAction"]);
		}

		public static void GetObjectPostAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			EnhancedTimeSpan? ageLimitForRetention = dataRow["AgeLimitForRetention"].IsNullValue() ? null : ((EnhancedTimeSpan?)dataRow["AgeLimitForRetention"]);
			dataRow["TypeGroup"] = RetentionPolicyTagPropertiesHelper.GetLocalizedType((ElcFolderType)dataRow["Type"]);
			dataRow["AgeLimitForRetention"] = ((ageLimitForRetention != null) ? ageLimitForRetention.Value.Days.ToString() : null);
			dataRow["FolderType"] = RetentionUtils.GetLocalizedType((ElcFolderType)dataRow["Type"]);
			dataRow["RetentionDays"] = RetentionPolicyTagPropertiesHelper.GetRetentionDays(ageLimitForRetention, (bool)dataRow["RetentionEnabled"]);
			dataRow["RetentionPeriodDays"] = RetentionPolicyTagPropertiesHelper.GetRetentionPeriodDays(ageLimitForRetention, (bool)dataRow["RetentionEnabled"]);
			dataRow["RetentionPolicyActionType"] = RetentionUtils.GetLocalizedRetentionActionType((RetentionActionType)dataRow["RetentionAction"]);
		}

		public static void SetObjectPreAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			if (!dataRow["RetentionEnabled"].IsNullValue() && dataRow["RetentionEnabled"].IsFalse())
			{
				dataRow["AgeLimitForRetention"] = null;
			}
		}

		public static void GetListForPickerPostAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			List<DataRow> list = new List<DataRow>();
			dataTable.BeginLoadData();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				ElcFolderType elcFolderType = (ElcFolderType)dataRow["Type"];
				if (elcFolderType == ElcFolderType.ManagedCustomFolder)
				{
					list.Add(dataRow);
				}
				else
				{
					EnhancedTimeSpan? ageLimitForRetention = dataRow["AgeLimitForRetention"].IsNullValue() ? null : ((EnhancedTimeSpan?)dataRow["AgeLimitForRetention"]);
					dataRow["FolderType"] = RetentionUtils.GetLocalizedType((ElcFolderType)dataRow["Type"]);
					dataRow["RetentionDays"] = RetentionPolicyTagPropertiesHelper.GetRetentionDays(ageLimitForRetention, (bool)dataRow["RetentionEnabled"]);
					dataRow["RetentionPeriodDays"] = RetentionPolicyTagPropertiesHelper.GetRetentionPeriodDays(ageLimitForRetention, (bool)dataRow["RetentionEnabled"]);
					dataRow["RetentionPolicyActionType"] = RetentionUtils.GetLocalizedRetentionActionType((RetentionActionType)dataRow["RetentionAction"]);
				}
			}
			foreach (DataRow row in list)
			{
				dataTable.Rows.Remove(row);
			}
			dataTable.EndLoadData();
		}

		private static string GetRetentionPeriodDays(EnhancedTimeSpan? ageLimitForRetention, bool retentionEnabled)
		{
			if (ageLimitForRetention != null && retentionEnabled)
			{
				int days = ageLimitForRetention.Value.Days;
				return string.Format((days > 1) ? Strings.RPTDays : Strings.RPTDay, days);
			}
			return Strings.Unlimited;
		}

		private static int GetRetentionDays(EnhancedTimeSpan? ageLimitForRetention, bool retentionEnabled)
		{
			if (ageLimitForRetention == null || !retentionEnabled)
			{
				return int.MaxValue;
			}
			return ageLimitForRetention.Value.Days;
		}

		private static string GetRetentionPeriodDetail(EnhancedTimeSpan? ageLimitForRetention, bool retentionEnabled)
		{
			if (ageLimitForRetention != null && retentionEnabled)
			{
				return RetentionPolicyTagPropertiesHelper.GetRetentionPeriodDays(ageLimitForRetention, retentionEnabled);
			}
			return Strings.RPTNone;
		}

		private static string GetLocalizedRetentionAction(bool retentionEnabled, RetentionActionType retentionActionType)
		{
			string result = LocalizedDescriptionAttribute.FromEnum(typeof(RetentionActionType), retentionActionType);
			bool flag = retentionActionType == RetentionActionType.MoveToArchive;
			if (retentionActionType == RetentionActionType.DeleteAndAllowRecovery)
			{
				result = Strings.RetentionActionDeleteAndAllowRecovery;
			}
			if (!retentionEnabled)
			{
				if (flag)
				{
					result = Strings.RetentionActionNeverMove;
				}
				else
				{
					result = Strings.RetentionActionNeverDelete;
				}
			}
			return result;
		}

		private static string GetLocalizedType(ElcFolderType folderType)
		{
			string result = Strings.RetentionTagNewSystemFolder;
			if (folderType != ElcFolderType.All)
			{
				if (folderType == ElcFolderType.Personal)
				{
					result = Strings.RetentionTagNewPersonal;
				}
			}
			else
			{
				result = Strings.RetentionTagNewAll;
			}
			return result;
		}
	}
}
