using System;
using System.Security.Principal;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.FullTextIndex;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class Globals
	{
		public static uint MaxRPCThreadCount
		{
			get
			{
				return Microsoft.Exchange.Server.Storage.StoreCommonServices.Globals.maxRPCThreadCount;
			}
			set
			{
				Microsoft.Exchange.Server.Storage.StoreCommonServices.Globals.maxRPCThreadCount = value;
			}
		}

		internal static ClientSecurityContext ProcessSecurityContext
		{
			get
			{
				return Microsoft.Exchange.Server.Storage.StoreCommonServices.Globals.systemSecurityContext;
			}
		}

		public static void Initialize(StoreDatabase.InitInMemoryDatabaseSchemaHandlerDelegate initInMemoryDatabaseSchemaHandler, StoreDatabase.MountingHandlerDelegate mountingHandler, StoreDatabase.MountedHandlerDelegate mountedHandler, StoreDatabase.DismountingHandlerDelegate dismountingHandler)
		{
			WindowsIdentity current = WindowsIdentity.GetCurrent();
			Microsoft.Exchange.Server.Storage.StoreCommonServices.Globals.systemSecurityContext = new ClientSecurityContext(current);
			StoreDatabase.InitInMemoryDatabaseSchemaHandler = initInMemoryDatabaseSchemaHandler;
			StoreDatabase.MountingHandler = mountingHandler;
			StoreDatabase.MountedHandler = mountedHandler;
			StoreDatabase.DismountingHandler = dismountingHandler;
			Context.Initialize();
			Storage.Initialize();
			DatabaseSchema.Initialize();
			StoreDatabase.Initialize();
			Mailbox.Initialize();
			AssistantActivityMonitor.Initialize();
			MaintenanceHandler.Initialize();
			MailboxTaskQueue.Initialize();
			MailboxStateCache.Initialize();
			PropertySchema.Initialize();
			SharedObjectPropertyBagDataCache.Initialize();
			MailboxState.Initialize();
			ReplidGuidMap.Initialize();
			GlobalNamedPropertyMap.Initialize();
			NamedPropertyMap.Initialize();
			ChangeNumberAndIdCounters.Initialize();
			TimedEventsQueue.Initialize();
			PerformanceCounterFactory.CreateClientTypeInstances(false);
			FullTextIndexLogger.Initialize();
			BufferedPerformanceCounter.Initialize();
			LazyMailboxActionList.Initialize();
			ClientActivityStrings.Initialize();
			RopSummaryCollector.Initialize();
			QueryPlanner.Initialize();
			Mailbox.TableSizeStatistics[] array = new Mailbox.TableSizeStatistics[2];
			Mailbox.TableSizeStatistics[] array2 = array;
			int num = 0;
			Mailbox.TableSizeStatistics tableSizeStatistics = default(Mailbox.TableSizeStatistics);
			tableSizeStatistics.TableAccessor = ((Context context) => DatabaseSchema.ExtendedPropertyNameMappingTable(context.Database).Table);
			tableSizeStatistics.TotalPagesProperty = PropTag.Mailbox.OtherTablesTotalPages;
			tableSizeStatistics.AvailablePagesProperty = PropTag.Mailbox.OtherTablesAvailablePages;
			array2[num] = tableSizeStatistics;
			Mailbox.TableSizeStatistics[] array3 = array;
			int num2 = 1;
			Mailbox.TableSizeStatistics tableSizeStatistics2 = default(Mailbox.TableSizeStatistics);
			tableSizeStatistics2.TableAccessor = ((Context context) => DatabaseSchema.ReplidGuidMapTable(context.Database).Table);
			tableSizeStatistics2.TotalPagesProperty = PropTag.Mailbox.OtherTablesTotalPages;
			tableSizeStatistics2.AvailablePagesProperty = PropTag.Mailbox.OtherTablesAvailablePages;
			array3[num2] = tableSizeStatistics2;
			Mailbox.RegisterTableSizeStatistics(array);
			RopSummaryResolver.Add(OperationType.Rop, (byte operationId) => ((RopId)operationId).ToString());
			RopSummaryResolver.Add(OperationType.Task, (byte operationId) => ((TaskTypeId)operationId).ToString());
		}

		public static void Terminate()
		{
			BufferedPerformanceCounter.Terminate();
			StoreDatabase.Terminate();
			Storage.Terminate();
			if (Microsoft.Exchange.Server.Storage.StoreCommonServices.Globals.systemSecurityContext != null)
			{
				Microsoft.Exchange.Server.Storage.StoreCommonServices.Globals.systemSecurityContext.Dispose();
				Microsoft.Exchange.Server.Storage.StoreCommonServices.Globals.systemSecurityContext = null;
			}
		}

		public static void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
			DatabaseSchema.Initialize(database);
		}

		public static void DatabaseMounting(Context context, StoreDatabase database, bool readOnly)
		{
			DatabaseSchema.PostMountInitialize(context, database);
			PerformanceCounterFactory.CreateDatabaseInstance(database);
			if (Microsoft.Exchange.Server.Storage.Common.Globals.IsMultiProcess)
			{
				PerformanceCounterFactory.DefaultDatabaseInstanceName = database.MdbName;
				TempStream.Configure(database.PhysicalDatabase.FilePath);
			}
			MailboxStateCache.MountHandler(database, context);
			AssistantActivityMonitor.MountHandler(context, database);
			MaintenanceHandler.MountHandler(context);
			PropertySchema.MountEventHandler(database);
			PropertySchemaPopulation.MountEventHandler(database);
			MailboxQuarantiner.MountEventHandler(database);
			TimedEventsQueue.MountEventHandler(context, database, readOnly);
			ResourceMonitorDigest.MountEventHandler(database);
			RopSummaryCollector.MountHandler(context);
			if (database.PhysicalDatabase.DatabaseType == DatabaseType.Sql)
			{
				BadPlanDetector.MountEventHandler(database);
			}
		}

		public static void DatabaseMounted(Context context, StoreDatabase database)
		{
			if (!database.IsReadOnly)
			{
				AssistantActivityMonitor.MountedHandler(context, database);
			}
		}

		public static void DatabaseDismounting(Context context, StoreDatabase database)
		{
			PropertySchema.DismountEventHandler(database);
			MaintenanceHandler.DismountHandler(database);
			AssistantActivityMonitor.DismountHandler(database);
			MailboxStateCache.DismountHandler(context, database);
			PerformanceCounterFactory.CloseDatabaseInstance(database);
		}

		private static ClientSecurityContext systemSecurityContext;

		private static uint maxRPCThreadCount;
	}
}
