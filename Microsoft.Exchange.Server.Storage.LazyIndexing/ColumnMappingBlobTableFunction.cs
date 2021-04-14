using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public sealed class ColumnMappingBlobTableFunction
	{
		internal ColumnMappingBlobTableFunction()
		{
			this.columnType = Factory.CreatePhysicalColumn("columnType", "columnType", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.fixedLength = Factory.CreatePhysicalColumn("fixedLength", "fixedLength", typeof(bool), false, false, false, false, false, Visibility.Public, 0, 1, 1);
			this.columnLength = Factory.CreatePhysicalColumn("columnLength", "columnLength", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.propName = Factory.CreatePhysicalColumn("propName", "propName", typeof(string), false, false, false, false, false, Visibility.Public, 0, 512, 512);
			this.propId = Factory.CreatePhysicalColumn("propId", "propId", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			string name = "PrimaryKey";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[5];
			Index index = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true,
				true,
				true,
				true
			}, new PhysicalColumn[]
			{
				this.columnType,
				this.fixedLength,
				this.columnLength,
				this.propName,
				this.propId
			});
			Index[] indexes = new Index[]
			{
				index
			};
			this.tableFunction = Factory.CreateTableFunction("ColumnMappingBlob", new TableFunction.GetTableContentsDelegate(this.GetTableContents), new TableFunction.GetColumnFromRowDelegate(this.GetColumnFromRow), Visibility.Public, new Type[]
			{
				typeof(byte[])
			}, indexes, new PhysicalColumn[]
			{
				this.columnType,
				this.fixedLength,
				this.columnLength,
				this.propName,
				this.propId
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

		public PhysicalColumn FixedLength
		{
			get
			{
				return this.fixedLength;
			}
		}

		public PhysicalColumn ColumnLength
		{
			get
			{
				return this.columnLength;
			}
		}

		public PhysicalColumn PropName
		{
			get
			{
				return this.propName;
			}
		}

		public PhysicalColumn PropId
		{
			get
			{
				return this.propId;
			}
		}

		public object GetTableContents(IConnectionProvider connectionProvider, object[] parameters)
		{
			if (parameters[0] == null)
			{
				throw new InvalidSerializedFormatException("Blob must not be null");
			}
			int num;
			return ColumnMappingBlob.Deserialize(out num, (byte[])parameters[0]);
		}

		public object GetColumnFromRow(IConnectionProvider connectionProvider, object row, PhysicalColumn columnToFetch)
		{
			ColumnMappingBlob columnMappingBlob = (ColumnMappingBlob)row;
			if (columnToFetch == this.ColumnType)
			{
				return columnMappingBlob.ColumnType;
			}
			if (columnToFetch == this.FixedLength)
			{
				return columnMappingBlob.FixedLength;
			}
			if (columnToFetch == this.ColumnLength)
			{
				return columnMappingBlob.ColumnLength;
			}
			if (columnToFetch == this.PropName)
			{
				return columnMappingBlob.PropName;
			}
			if (columnToFetch == this.PropId)
			{
				return columnMappingBlob.PropId;
			}
			return null;
		}

		public const string columnTypeName = "columnType";

		public const string fixedLengthName = "fixedLength";

		public const string columnLengthName = "columnLength";

		public const string propNameName = "propName";

		public const string propIdName = "propId";

		public const string TableFunctionName = "ColumnMappingBlob";

		private PhysicalColumn columnType;

		private PhysicalColumn fixedLength;

		private PhysicalColumn columnLength;

		private PhysicalColumn propName;

		private PhysicalColumn propId;

		private TableFunction tableFunction;
	}
}
