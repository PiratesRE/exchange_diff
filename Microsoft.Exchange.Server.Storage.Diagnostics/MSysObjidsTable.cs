using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public sealed class MSysObjidsTable
	{
		internal MSysObjidsTable()
		{
			this.objid = Factory.CreatePhysicalColumn("objid", "objid", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.objidTable = Factory.CreatePhysicalColumn("objidTable", "objidTable", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.type = Factory.CreatePhysicalColumn("type", "type", typeof(short), false, false, false, false, false, Visibility.Public, 0, 2, 2);
			string name = "primary";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[1];
			this.primaryIndex = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true
			}, new PhysicalColumn[]
			{
				this.objid
			});
			Index[] indexes = new Index[]
			{
				this.primaryIndex
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 0);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.objid,
				this.objidTable,
				this.type
			};
			this.table = Factory.CreateTable("MSysObjids", TableClass.Unknown, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, true, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public PhysicalColumn Objid
		{
			get
			{
				return this.objid;
			}
		}

		public PhysicalColumn ObjidTable
		{
			get
			{
				return this.objidTable;
			}
		}

		public PhysicalColumn Type
		{
			get
			{
				return this.type;
			}
		}

		public Index PrimaryIndex
		{
			get
			{
				return this.primaryIndex;
			}
		}

		internal void PostMountInitialize(ComponentVersion databaseVersion)
		{
			PhysicalColumn physicalColumn = this.objid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.objid = null;
			}
			physicalColumn = this.objidTable;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.objidTable = null;
			}
			physicalColumn = this.type;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.type = null;
			}
			for (int i = this.Table.Columns.Count - 1; i >= 0; i--)
			{
				this.Table.Columns[i].Index = i;
			}
			Index index = this.primaryIndex;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.primaryIndex = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string objidName = "objid";

		public const string objidTableName = "objidTable";

		public const string typeName = "type";

		public const string PhysicalTableName = "MSysObjids";

		private PhysicalColumn objid;

		private PhysicalColumn objidTable;

		private PhysicalColumn type;

		private Index primaryIndex;

		private Table table;
	}
}
