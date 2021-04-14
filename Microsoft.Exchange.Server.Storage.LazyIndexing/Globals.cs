using System;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public static class Globals
	{
		public static void Initialize()
		{
			DatabaseSchema.Initialize();
			PhysicalIndexCache.Initialize();
			LogicalIndexVersionHistory.Initialize();
			LogicalIndexCache.Initialize();
			LogicalIndex.Initialize();
			Mailbox.TableSizeStatistics[] array = new Mailbox.TableSizeStatistics[1];
			Mailbox.TableSizeStatistics[] array2 = array;
			int num = 0;
			Mailbox.TableSizeStatistics tableSizeStatistics = default(Mailbox.TableSizeStatistics);
			tableSizeStatistics.TableAccessor = ((Context context) => DatabaseSchema.PseudoIndexMaintenanceTable(context.Database).Table);
			tableSizeStatistics.TotalPagesProperty = PropTag.Mailbox.OtherTablesTotalPages;
			tableSizeStatistics.AvailablePagesProperty = PropTag.Mailbox.OtherTablesAvailablePages;
			array2[num] = tableSizeStatistics;
			Mailbox.RegisterTableSizeStatistics(array);
		}

		public static void Terminate()
		{
		}

		public static void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
			DatabaseSchema.Initialize(database);
		}

		public static void DatabaseMounting(Context context, StoreDatabase database)
		{
			DatabaseSchema.PostMountInitialize(context, database);
			LogicalIndexVersionHistory.MountEventHandler(database);
			PhysicalIndexCache.MountEventHandler(context);
		}

		public static void DatabaseMounted(Context context, StoreDatabase database)
		{
			if (!database.IsReadOnly)
			{
				LogicalIndexCache.MountedEventHandler(context);
			}
		}

		public static void DatabaseDismounting(Context context, StoreDatabase database)
		{
			PhysicalIndexCache.DismountEventHandler(database);
			LogicalIndexVersionHistory.DismountEventHandler(database);
		}
	}
}
