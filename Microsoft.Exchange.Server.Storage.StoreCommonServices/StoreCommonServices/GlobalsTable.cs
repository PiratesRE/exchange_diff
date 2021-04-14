using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public sealed class GlobalsTable
	{
		internal GlobalsTable()
		{
			this.versionName = Factory.CreatePhysicalColumn("VersionName", "VersionName", typeof(string), false, false, false, false, false, Visibility.Public, 64, 0, 64);
			this.databaseVersion = Factory.CreatePhysicalColumn("DatabaseVersion", "DatabaseVersion", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.inid = Factory.CreatePhysicalColumn("Inid", "Inid", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.lastMaintenanceTask = Factory.CreatePhysicalColumn("LastMaintenanceTask", "LastMaintenanceTask", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.eventCounterLowerBound = Factory.CreatePhysicalColumn("EventCounterLowerBound", "EventCounterLowerBound", typeof(long), true, false, false, false, true, Visibility.Public, 0, 8, 8);
			this.eventCounterUpperBound = Factory.CreatePhysicalColumn("EventCounterUpperBound", "EventCounterUpperBound", typeof(long), true, false, false, false, true, Visibility.Public, 0, 8, 8);
			this.databaseDsGuid = Factory.CreatePhysicalColumn("DatabaseDsGuid", "DatabaseDsGuid", typeof(Guid), true, false, false, false, true, Visibility.Public, 0, 16, 16);
			string name = "GlobalsPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[1];
			this.globalsPK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true
			}, new PhysicalColumn[]
			{
				this.VersionName
			});
			string name2 = "NewGlobalsPK";
			bool primaryKey2 = false;
			bool unique2 = true;
			bool schemaExtension2 = true;
			bool[] conditional2 = new bool[1];
			this.newGlobalsPK = new Index(name2, primaryKey2, unique2, schemaExtension2, conditional2, new bool[]
			{
				true
			}, new PhysicalColumn[]
			{
				this.DatabaseVersion
			});
			Index[] indexes = new Index[]
			{
				this.GlobalsPK,
				this.NewGlobalsPK
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 0);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.VersionName,
				this.DatabaseVersion,
				this.Inid,
				this.LastMaintenanceTask,
				this.ExtensionBlob,
				this.EventCounterLowerBound,
				this.EventCounterUpperBound,
				this.DatabaseDsGuid
			};
			this.table = Factory.CreateTable("Globals", TableClass.Globals, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public PhysicalColumn VersionName
		{
			get
			{
				return this.versionName;
			}
		}

		public PhysicalColumn DatabaseVersion
		{
			get
			{
				return this.databaseVersion;
			}
		}

		public PhysicalColumn Inid
		{
			get
			{
				return this.inid;
			}
		}

		public PhysicalColumn LastMaintenanceTask
		{
			get
			{
				return this.lastMaintenanceTask;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public PhysicalColumn EventCounterLowerBound
		{
			get
			{
				return this.eventCounterLowerBound;
			}
		}

		public PhysicalColumn EventCounterUpperBound
		{
			get
			{
				return this.eventCounterUpperBound;
			}
		}

		public PhysicalColumn DatabaseDsGuid
		{
			get
			{
				return this.databaseDsGuid;
			}
		}

		public Index GlobalsPK
		{
			get
			{
				return this.globalsPK;
			}
		}

		public Index NewGlobalsPK
		{
			get
			{
				return this.newGlobalsPK;
			}
		}

		internal void PostMountInitialize(ComponentVersion databaseVersion)
		{
			PhysicalColumn physicalColumn = this.versionName;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.versionName = null;
			}
			physicalColumn = this.databaseVersion;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.databaseVersion = null;
			}
			physicalColumn = this.inid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.inid = null;
			}
			physicalColumn = this.lastMaintenanceTask;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lastMaintenanceTask = null;
			}
			physicalColumn = this.extensionBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.extensionBlob = null;
			}
			physicalColumn = this.eventCounterLowerBound;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.eventCounterLowerBound = null;
			}
			physicalColumn = this.eventCounterUpperBound;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.eventCounterUpperBound = null;
			}
			physicalColumn = this.databaseDsGuid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.databaseDsGuid = null;
			}
			for (int i = this.Table.Columns.Count - 1; i >= 0; i--)
			{
				this.Table.Columns[i].Index = i;
			}
			Index index = this.globalsPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.globalsPK = null;
				this.Table.Indexes.Remove(index);
			}
			index = this.newGlobalsPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.newGlobalsPK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string VersionNameName = "VersionName";

		public const string DatabaseVersionName = "DatabaseVersion";

		public const string InidName = "Inid";

		public const string LastMaintenanceTaskName = "LastMaintenanceTask";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string EventCounterLowerBoundName = "EventCounterLowerBound";

		public const string EventCounterUpperBoundName = "EventCounterUpperBound";

		public const string DatabaseDsGuidName = "DatabaseDsGuid";

		public const string PhysicalTableName = "Globals";

		private PhysicalColumn versionName;

		private PhysicalColumn databaseVersion;

		private PhysicalColumn inid;

		private PhysicalColumn lastMaintenanceTask;

		private PhysicalColumn extensionBlob;

		private PhysicalColumn eventCounterLowerBound;

		private PhysicalColumn eventCounterUpperBound;

		private PhysicalColumn databaseDsGuid;

		private Index globalsPK;

		private Index newGlobalsPK;

		private Table table;
	}
}
