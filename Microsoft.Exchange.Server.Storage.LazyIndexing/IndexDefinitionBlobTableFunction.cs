using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public sealed class IndexDefinitionBlobTableFunction
	{
		internal IndexDefinitionBlobTableFunction()
		{
			this.columnType = Factory.CreatePhysicalColumn("columnType", "columnType", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.maxLength = Factory.CreatePhysicalColumn("maxLength", "maxLength", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.fixedLength = Factory.CreatePhysicalColumn("fixedLength", "fixedLength", typeof(bool), false, false, false, false, false, Visibility.Public, 0, 1, 1);
			this.ascending = Factory.CreatePhysicalColumn("ascending", "ascending", typeof(bool), false, false, false, false, false, Visibility.Public, 0, 1, 1);
			string name = "PrimaryKey";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[4];
			Index index = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true,
				true,
				true
			}, new PhysicalColumn[]
			{
				this.columnType,
				this.maxLength,
				this.fixedLength,
				this.ascending
			});
			Index[] indexes = new Index[]
			{
				index
			};
			this.tableFunction = Factory.CreateTableFunction("IndexDefinitionBlob", new TableFunction.GetTableContentsDelegate(this.GetTableContents), new TableFunction.GetColumnFromRowDelegate(this.GetColumnFromRow), Visibility.Public, new Type[]
			{
				typeof(byte[])
			}, indexes, new PhysicalColumn[]
			{
				this.columnType,
				this.maxLength,
				this.fixedLength,
				this.ascending
			});
		}

		public TableFunction TableFunction
		{
			get
			{
				return this.tableFunction;
			}
		}

		public PhysicalColumn ColumnType
		{
			get
			{
				return this.columnType;
			}
		}

		public PhysicalColumn MaxLength
		{
			get
			{
				return this.maxLength;
			}
		}

		public PhysicalColumn FixedLength
		{
			get
			{
				return this.fixedLength;
			}
		}

		public PhysicalColumn Ascending
		{
			get
			{
				return this.ascending;
			}
		}

		public object GetTableContents(IConnectionProvider connectionProvider, object[] parameters)
		{
			if (parameters[0] == null)
			{
				throw new InvalidSerializedFormatException("Blob must not be null");
			}
			int num;
			int num2;
			short num3;
			return IndexDefinitionBlob.Deserialize(out num, out num2, out num3, (byte[])parameters[0]);
		}

		public object GetColumnFromRow(IConnectionProvider connectionProvider, object row, PhysicalColumn columnToFetch)
		{
			IndexDefinitionBlob indexDefinitionBlob = (IndexDefinitionBlob)row;
			if (columnToFetch == this.ColumnType)
			{
				return indexDefinitionBlob.ColumnType;
			}
			if (columnToFetch == this.MaxLength)
			{
				return indexDefinitionBlob.MaxLength;
			}
			if (columnToFetch == this.FixedLength)
			{
				return indexDefinitionBlob.FixedLength;
			}
			if (columnToFetch == this.Ascending)
			{
				return indexDefinitionBlob.Ascending;
			}
			return null;
		}

		public const string columnTypeName = "columnType";

		public const string maxLengthName = "maxLength";

		public const string fixedLengthName = "fixedLength";

		public const string ascendingName = "ascending";

		public const string TableFunctionName = "IndexDefinitionBlob";

		private PhysicalColumn columnType;

		private PhysicalColumn maxLength;

		private PhysicalColumn fixedLength;

		private PhysicalColumn ascending;

		private TableFunction tableFunction;
	}
}
