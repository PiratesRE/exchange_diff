using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public sealed class Index : IIndex
	{
		public Index(string name, bool primaryKey, bool unique, bool schemaExtension, bool[] conditional, bool[] ascending, params PhysicalColumn[] columns)
		{
			this.name = name;
			this.primaryKey = primaryKey;
			this.unique = unique;
			this.columns = new List<Column>(columns);
			this.conditional = new List<bool>(conditional);
			this.ascending = new List<bool>(ascending);
			if (schemaExtension)
			{
				this.minVersion = int.MaxValue;
			}
			else
			{
				this.minVersion = 0;
			}
			this.maxVersion = int.MaxValue;
			this.renameDictionary = new Dictionary<Column, Column>(columns.Length);
			foreach (PhysicalColumn column in columns)
			{
				this.renameDictionary.Add(column, column);
			}
		}

		public IReadOnlyDictionary<Column, Column> RenameDictionary
		{
			get
			{
				return this.renameDictionary;
			}
		}

		public Table Table
		{
			get
			{
				return this.columns[0].Table;
			}
		}

		public Table IndexTable
		{
			get
			{
				return this.Table;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public IList<Column> Columns
		{
			get
			{
				return this.columns;
			}
		}

		public ISet<Column> ConstantColumns
		{
			get
			{
				return null;
			}
		}

		public int ColumnCount
		{
			get
			{
				return this.columns.Count;
			}
		}

		public IList<object> IndexKeyPrefix
		{
			get
			{
				return null;
			}
		}

		public bool PrimaryKey
		{
			get
			{
				return this.primaryKey;
			}
		}

		public bool Unique
		{
			get
			{
				return this.unique;
			}
		}

		public IList<bool> Conditional
		{
			get
			{
				return this.conditional;
			}
		}

		public IList<bool> Ascending
		{
			get
			{
				return this.ascending;
			}
		}

		public SortOrder SortOrder
		{
			get
			{
				return new SortOrder(this.columns, this.ascending);
			}
		}

		public SortOrder LogicalSortOrder
		{
			get
			{
				return this.SortOrder;
			}
		}

		public bool SchemaExtension
		{
			get
			{
				return this.minVersion != 0;
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

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(20);
			if (this.PrimaryKey)
			{
				stringBuilder.Append("PrimaryKey ");
			}
			if (this.Unique)
			{
				stringBuilder.Append("Unique ");
			}
			stringBuilder.Append("Name:[");
			stringBuilder.Append(this.Name);
			stringBuilder.Append("] Columns:[");
			for (int i = 0; i < this.ColumnCount; i++)
			{
				stringBuilder.Append("[");
				stringBuilder.Append(this.Columns[i].Name);
				if (this.Conditional[i])
				{
					stringBuilder.Append(", Conditional");
				}
				if (!this.Ascending[i])
				{
					stringBuilder.Append(", Descending");
				}
				stringBuilder.Append("]");
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public int PositionInIndex(Column col)
		{
			for (int i = 0; i < this.columns.Count; i++)
			{
				if (this.columns[i] == col)
				{
					return i;
				}
			}
			return -1;
		}

		public bool GetIndexColumn(Column column, bool acceptTruncated, out Column indexColumn)
		{
			if (this.PositionInIndex(column) != -1 && (acceptTruncated || column.Size > 0 || column.MaxLength <= 255))
			{
				indexColumn = column;
				return true;
			}
			indexColumn = null;
			return false;
		}

		internal void SetIsPrimaryKeyForUpgraders(bool primaryKey)
		{
			this.primaryKey = primaryKey;
		}

		public const CompareOptions IndexCompareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;

		private readonly string name;

		private readonly List<Column> columns;

		private readonly List<bool> conditional;

		private readonly List<bool> ascending;

		private readonly bool unique;

		private bool primaryKey;

		private int minVersion;

		private int maxVersion;

		private Dictionary<Column, Column> renameDictionary;
	}
}
