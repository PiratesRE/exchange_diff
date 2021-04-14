using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public sealed class ColumnMappingBlobHeaderTableFunction
	{
		internal ColumnMappingBlobHeaderTableFunction()
		{
			this.keyColumnCount = Factory.CreatePhysicalColumn("keyColumnCount", "keyColumnCount", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			string name = "PrimaryKey";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[1];
			Index index = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true
			}, new PhysicalColumn[]
			{
				this.keyColumnCount
			});
			Index[] indexes = new Index[]
			{
				index
			};
			this.tableFunction = Factory.CreateTableFunction("ColumnMappingBlobHeader", new TableFunction.GetTableContentsDelegate(this.GetTableContents), new TableFunction.GetColumnFromRowDelegate(this.GetColumnFromRow), Visibility.Public, new Type[]
			{
				typeof(byte[])
			}, indexes, new PhysicalColumn[]
			{
				this.keyColumnCount
			});
		}

		public TableFunction TableFunction
		{
			get
			{
				return this.tableFunction;
			}
		}

		public PhysicalColumn KeyColumnCount
		{
			get
			{
				return this.keyColumnCount;
			}
		}

		public object GetTableContents(IConnectionProvider connectionProvider, object[] parameters)
		{
			if (parameters[0] == null)
			{
				throw new InvalidSerializedFormatException("Blob must not be null");
			}
			int num;
			ColumnMappingBlob.Deserialize(out num, (byte[])parameters[0]);
			return new ColumnMappingBlobHeaderTableFunction.ColumnMappingBlobHeader[]
			{
				new ColumnMappingBlobHeaderTableFunction.ColumnMappingBlobHeader(num)
			};
		}

		public object GetColumnFromRow(IConnectionProvider connectionProvider, object row, PhysicalColumn columnToFetch)
		{
			ColumnMappingBlobHeaderTableFunction.ColumnMappingBlobHeader columnMappingBlobHeader = (ColumnMappingBlobHeaderTableFunction.ColumnMappingBlobHeader)row;
			if (columnToFetch == this.KeyColumnCount)
			{
				return columnMappingBlobHeader.KeyColumnCount;
			}
			return null;
		}

		public const string keyColumnCountName = "keyColumnCount";

		public const string TableFunctionName = "ColumnMappingBlobHeader";

		private PhysicalColumn keyColumnCount;

		private TableFunction tableFunction;

		private class ColumnMappingBlobHeader
		{
			public ColumnMappingBlobHeader(int keyColumnCount)
			{
				this.keyColumnCount = keyColumnCount;
			}

			public int KeyColumnCount
			{
				get
				{
					return this.keyColumnCount;
				}
			}

			private readonly int keyColumnCount;
		}
	}
}
