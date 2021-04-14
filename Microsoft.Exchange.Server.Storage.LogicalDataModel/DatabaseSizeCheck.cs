using System;
using System.Text;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public static class DatabaseSizeCheck
	{
		internal static void LaunchDatabaseSizeCheckTask(StoreDatabase database)
		{
			Task<StoreDatabase>.TaskCallback callback = TaskExecutionWrapper<StoreDatabase>.WrapExecute(new TaskDiagnosticInformation(TaskTypeId.DatabaseSizeCheck, ClientType.System, database.MdbGuid), new TaskExecutionWrapper<StoreDatabase>.TaskCallback<Context>(DatabaseSizeCheck.DatabaseSizePeriodicCheck));
			RecurringTask<StoreDatabase> task = new RecurringTask<StoreDatabase>(callback, database, TimeSpan.FromDays(1.0), false);
			database.TaskList.Add(task, true);
		}

		private static void DatabaseSizePeriodicCheck(Context context, StoreDatabase database, Func<bool> shouldCallbackContinue)
		{
			DatabaseSizeCheck.CheckDatabaseSize(context, database);
		}

		internal static IDisposable SetGetDatabaseSizeTestHook(DatabaseSizeCheck.GetDatabaseSizeDelegate testDelegate)
		{
			return DatabaseSizeCheck.getDatabaseSizeTestHook.SetTestHook(testDelegate);
		}

		internal static void CheckDatabaseSize(Context context, StoreDatabase database)
		{
			uint warningThresholdPct = 0U;
			uint num = 0U;
			uint num2 = (uint)(database.PhysicalDatabase.PageSize / 1024);
			DatabaseSizeCheck.numberOfPagesInOneGb = 1048576U / num2;
			DatabaseSizeCheck.GetDbSizeLimitParamsFromRegistry(database, out num, out warningThresholdPct);
			uint pageSize = 0U;
			uint num3 = 0U;
			uint num4 = 0U;
			DatabaseSizeCheck.getDatabaseSizeTestHook.Value(context, database, out num3, out num4, out pageSize);
			DatabaseSizeCheck.TracePhysicalSize(pageSize, num3);
			uint num5 = DatabaseSizeCheck.ComputeMaximumSizeInPages(num, pageSize);
			uint num6 = DatabaseSizeCheck.ComputeWarningThreshold(num5, warningThresholdPct);
			if (num3 > num6)
			{
				DatabaseSizeCheck.TraceAvailablePages(pageSize, num4);
				num3 -= Math.Min(num4, num3);
			}
			if (num3 > num6)
			{
				if (num3 > num5)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_MaxDbSizeExceededDismountForced, new object[]
					{
						database.MdbName,
						num5,
						num,
						ConfigurationSchema.DatabaseSizeLimitGB.RegistryValueName,
						ConfigurationSchema.LocalDatabaseRegistryKey
					});
					DatabaseSizeCheck.TraceExceedDatabaseLimit(num, pageSize, num3, num5);
					database.PublishHaFailure(FailureTag.ExceededDatabaseMaxSize);
					return;
				}
				DatabaseSizeCheck.TraceExceedWarningThreshold(num, pageSize, num3, num5, num6);
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_ApproachingMaxDbSize, new object[]
				{
					database.MdbName,
					num5,
					num
				});
			}
		}

		private static void TracePhysicalSize(uint pageSize, uint totalPages)
		{
			if (ExTraceGlobals.DatabaseSizeCheckTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(1000);
				stringBuilder.Append("Physical size:");
				stringBuilder.Append(DatabaseSizeCheck.GbFromPages(totalPages, pageSize));
				stringBuilder.Append("Gb (");
				stringBuilder.Append(totalPages);
				stringBuilder.Append(" pages)");
				ExTraceGlobals.DatabaseSizeCheckTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		private static void TraceAvailablePages(uint pageSize, uint availablePages)
		{
			if (ExTraceGlobals.DatabaseSizeCheckTracer.IsTraceEnabled(TraceType.DebugTrace) && ExTraceGlobals.DatabaseSizeCheckTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(1000);
				stringBuilder.Append("Db free space:");
				stringBuilder.Append(DatabaseSizeCheck.GbFromPages(availablePages, pageSize));
				stringBuilder.Append("Gb (");
				stringBuilder.Append(availablePages);
				stringBuilder.Append(" pages)");
				ExTraceGlobals.DatabaseSizeCheckTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		private static void TraceExceedDatabaseLimit(uint maxSizeGB, uint pageSize, uint totalPages, uint maxSizeInPages)
		{
			if (ExTraceGlobals.DatabaseSizeCheckTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(1000);
				stringBuilder.Append("Db size of");
				stringBuilder.Append(DatabaseSizeCheck.GbFromPages(totalPages, pageSize));
				stringBuilder.Append("Gb (");
				stringBuilder.Append(totalPages);
				stringBuilder.Append(" pages) exceeds limit of");
				stringBuilder.Append(maxSizeGB);
				stringBuilder.Append("Gb (");
				stringBuilder.Append(maxSizeInPages);
				stringBuilder.Append("pages).");
				ExTraceGlobals.DatabaseSizeCheckTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		private static void TraceExceedWarningThreshold(uint maxSizeGB, uint pageSize, uint totalPages, uint maxSizeInPages, uint warningThreshold)
		{
			if (ExTraceGlobals.DatabaseSizeCheckTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(1000);
				stringBuilder.Append("Db size of");
				stringBuilder.Append(DatabaseSizeCheck.GbFromPages(totalPages, pageSize));
				stringBuilder.Append("Gb (");
				stringBuilder.Append(totalPages);
				stringBuilder.Append(" pages) exceeds warning threshold of");
				stringBuilder.Append(DatabaseSizeCheck.GbFromPages(warningThreshold, pageSize));
				stringBuilder.Append("Gb (");
				stringBuilder.Append(warningThreshold);
				stringBuilder.Append("pages). [MaxSize=");
				stringBuilder.Append(maxSizeGB);
				stringBuilder.Append("Gb (");
				stringBuilder.Append(maxSizeInPages);
				stringBuilder.Append("pages)]");
				ExTraceGlobals.DatabaseSizeCheckTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		internal static uint ComputeMaximumSizeInPages(uint maxSizeGB, uint pageSize)
		{
			return DatabaseSizeCheck.PagesFromGb(maxSizeGB, pageSize);
		}

		internal static uint ComputeWarningThreshold(uint maxSizePage, uint warningThresholdPct)
		{
			return (uint)(maxSizePage * ((100U - warningThresholdPct) / 100.0));
		}

		private static uint GbFromPages(uint sizeInPages, uint pageSize)
		{
			return sizeInPages / DatabaseSizeCheck.numberOfPagesInOneGb;
		}

		private static uint PagesFromGb(uint sizeInGb, uint pageSize)
		{
			return sizeInGb * DatabaseSizeCheck.numberOfPagesInOneGb;
		}

		private static void GetDatabaseSize(Context context, StoreDatabase database, out uint totalPages, out uint availablePages, out uint pageSize)
		{
			using (context.AssociateWithDatabase(database))
			{
				database.PhysicalDatabase.GetDatabaseSize(context, out totalPages, out availablePages, out pageSize);
			}
		}

		private static void GetDbSizeLimitParamsFromRegistry(StoreDatabase database, out uint maxSizeGB, out uint warningThresholdPct)
		{
			maxSizeGB = ConfigurationSchema.DatabaseSizeLimitGB.Value;
			if (maxSizeGB != 0U)
			{
				uint val = (database.ServerEdition == ServerEditionType.Enterprise) ? DatabaseSizeCheck.databaseSizeMaxEnterpriseSKU : DatabaseSizeCheck.databaseSizeMaxStandardSKU;
				maxSizeGB = Math.Min(val, maxSizeGB);
			}
			else
			{
				maxSizeGB = ((database.ServerEdition == ServerEditionType.Enterprise) ? DatabaseSizeCheck.DatabaseSizeMaxEnterpriseSKUDefault : 1024U);
			}
			warningThresholdPct = Math.Min(ConfigurationSchema.DatabaseWarningThresholdPercent.Value, 100U);
		}

		private const uint DatabaseSizeMaxStandardSKUDefault = 1024U;

		internal const uint DbSizeWarningThresholdMax = 100U;

		private static uint numberOfPagesInOneGb;

		private static UnlimitedBytes databaseSizeMax = UnlimitedBytes.FromGB(16000L);

		private static uint databaseSizeMaxEnterpriseSKU = (uint)DatabaseSizeCheck.databaseSizeMax.GB;

		private static uint databaseSizeMaxStandardSKU = (uint)DatabaseSizeCheck.databaseSizeMax.GB;

		internal static uint DatabaseSizeMaxEnterpriseSKUDefault = DatabaseSizeCheck.databaseSizeMaxEnterpriseSKU;

		private static Hookable<DatabaseSizeCheck.GetDatabaseSizeDelegate> getDatabaseSizeTestHook = Hookable<DatabaseSizeCheck.GetDatabaseSizeDelegate>.Create(true, new DatabaseSizeCheck.GetDatabaseSizeDelegate(DatabaseSizeCheck.GetDatabaseSize));

		public delegate void GetDatabaseSizeDelegate(Context context, StoreDatabase database, out uint totalPages, out uint availablePages, out uint pageSize);
	}
}
