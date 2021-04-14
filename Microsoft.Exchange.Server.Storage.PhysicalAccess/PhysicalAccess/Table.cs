using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class Table
	{
		public static PhysicalColumn[] NoColumns
		{
			get
			{
				return Array<PhysicalColumn>.Empty;
			}
		}

		protected Table(string name, TableClass tableClass, CultureInfo culture, bool trackDirtyObjects, TableAccessHints tableAccessHints, bool readOnly, Visibility visibility, bool schemaExtension, SpecialColumns specialCols, Index[] indexes, PhysicalColumn[] computedColumns, PhysicalColumn[] columns)
		{
			if (schemaExtension)
			{
				this.minVersion = int.MaxValue;
			}
			else
			{
				this.minVersion = 0;
			}
			this.maxVersion = int.MaxValue;
			this.specialCols = specialCols;
			this.indexes = new List<Index>(indexes);
			this.name = name;
			this.tableClass = tableClass;
			this.culture = culture;
			this.columns = new List<PhysicalColumn>(columns);
			for (int i = 0; i < this.Columns.Count; i++)
			{
				this.Columns[i].Index = i;
				this.Columns[i].Table = this;
			}
			for (int j = 0; j < computedColumns.Length; j++)
			{
				computedColumns[j].Index = -1;
				computedColumns[j].Table = this;
			}
			for (int k = 0; k < this.Indexes.Count; k++)
			{
				if (this.Indexes[k].PrimaryKey)
				{
					this.primaryKeyIndex = this.Indexes[k];
					break;
				}
			}
			this.trackDirtyObjects = trackDirtyObjects;
			this.tableAccessHints = tableAccessHints;
			this.readOnly = readOnly;
			this.visibility = visibility;
			this.commonColumns = new List<PhysicalColumn>(this.Columns.Count);
			for (int l = 0; l < this.Columns.Count; l++)
			{
				if (!this.Columns[l].NotFetchedByDefault && this.PrimaryKeyIndex.PositionInIndex(this.Columns[l]) < 0)
				{
					this.commonColumns.Add(this.Columns[l]);
				}
			}
		}

		public TableClass TableClass
		{
			[DebuggerStepThrough]
			get
			{
				return this.tableClass;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
		}

		public SpecialColumns SpecialCols
		{
			get
			{
				return this.specialCols;
			}
		}

		public IList<PhysicalColumn> Columns
		{
			get
			{
				return this.columns;
			}
		}

		public IList<Column> ColumnsForTestOnly
		{
			get
			{
				return new List<Column>(this.columns);
			}
		}

		public IList<Index> Indexes
		{
			get
			{
				return this.indexes;
			}
		}

		public IIndex FullTextIndex
		{
			get
			{
				return this.fullTextIndex;
			}
			set
			{
				this.fullTextIndex = value;
			}
		}

		public int MinVersion
		{
			get
			{
				return this.minVersion;
			}
			set
			{
				this.minVersion = value;
			}
		}

		public int MaxVersion
		{
			get
			{
				return this.maxVersion;
			}
			set
			{
				this.maxVersion = value;
			}
		}

		public Index PrimaryKeyIndex
		{
			get
			{
				return this.primaryKeyIndex;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public IList<PhysicalColumn> CommonColumns
		{
			get
			{
				return this.commonColumns;
			}
		}

		public bool TrackDirtyObjects
		{
			get
			{
				return this.trackDirtyObjects;
			}
		}

		public TableAccessHints TableAccessHints
		{
			get
			{
				return this.tableAccessHints;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		public int NumberOfPartitioningColumns
		{
			get
			{
				return this.specialCols.NumberOfPartioningColumns;
			}
		}

		public bool IsPartitioned
		{
			get
			{
				return this.NumberOfPartitioningColumns > 0;
			}
		}

		public Visibility Visibility
		{
			get
			{
				return this.visibility;
			}
		}

		public static bool operator ==(Table table1, Table table2)
		{
			return object.ReferenceEquals(table1, table2) || (table1 != null && table2 != null && table1.Equals(table2));
		}

		public static bool operator !=(Table table1, Table table2)
		{
			return !(table1 == table2);
		}

		public abstract void CreateTable(IConnectionProvider connectionProvider, int version);

		public abstract void AddColumn(IConnectionProvider connectionProvider, PhysicalColumn column);

		public abstract void RemoveColumn(IConnectionProvider connectionProvider, PhysicalColumn column);

		public abstract void CreateIndex(IConnectionProvider connectionProvider, Index index, IList<object> partitionValues);

		public abstract void DeleteIndex(IConnectionProvider connectionProvider, Index index, IList<object> partitionValues);

		public abstract bool IsIndexCreated(IConnectionProvider connectionProvider, Index index, IList<object> partitionValues);

		public abstract bool ValidateLocaleVersion(IConnectionProvider connectionProvider, IList<object> partitionValues);

		public abstract void GetTableSize(IConnectionProvider connectionProvider, IList<object> partitionValues, out int totalPages, out int availablePages);

		public virtual VirtualColumn VirtualColumn(string column)
		{
			return null;
		}

		public override bool Equals(object other)
		{
			Table table = other as Table;
			return table != null && this.name == table.name;
		}

		public override int GetHashCode()
		{
			return this.name.GetHashCode();
		}

		public override string ToString()
		{
			return this.Name;
		}

		public PhysicalColumn Column(string name)
		{
			PhysicalColumn result = null;
			for (int i = 0; i < this.Columns.Count; i++)
			{
				if (this.Columns[i].Name == name)
				{
					result = this.Columns[i];
					break;
				}
			}
			return result;
		}

		internal void SetPrimaryKeyIndexForUpgraders(Index primaryKeyIndex)
		{
			this.primaryKeyIndex.SetIsPrimaryKeyForUpgraders(false);
			this.primaryKeyIndex = primaryKeyIndex;
			this.primaryKeyIndex.SetIsPrimaryKeyForUpgraders(true);
		}

		private readonly string name;

		private readonly TableClass tableClass;

		private readonly CultureInfo culture;

		private readonly bool readOnly;

		private Index primaryKeyIndex;

		private IIndex fullTextIndex;

		private IList<PhysicalColumn> columns;

		private IList<Index> indexes;

		private SpecialColumns specialCols;

		private IList<PhysicalColumn> commonColumns;

		private bool trackDirtyObjects;

		private TableAccessHints tableAccessHints;

		private int minVersion;

		private int maxVersion;

		private Visibility visibility;
	}
}
