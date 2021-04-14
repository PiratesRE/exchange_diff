using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public sealed class ConditionalIndexMappingBlobTableFunction
	{
		internal ConditionalIndexMappingBlobTableFunction()
		{
			this.columnName = Factory.CreatePhysicalColumn("columnName", "columnName", typeof(string), false, false, false, false, false, Visibility.Public, 0, 512, 512);
			this.columnValue = Factory.CreatePhysicalColumn("columnValue", "columnValue", typeof(bool), false, false, false, false, false, Visibility.Public, 0, 1, 1);
			string name = "PrimaryKey";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[2];
			Index index = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.columnName,
				this.columnValue
			});
			Index[] indexes = new Index[]
			{
				index
			};
			this.tableFunction = Factory.CreateTableFunction("ConditionalIndexMappingBlob", new TableFunction.GetTableContentsDelegate(this.GetTableContents), new TableFunction.GetColumnFromRowDelegate(this.GetColumnFromRow), Visibility.Public, new Type[]
			{
				typeof(byte[])
			}, indexes, new PhysicalColumn[]
			{
				this.columnName,
				this.columnValue
			});
		}

		public TableFunction TableFunction
		{
			get
			{
				return this.tableFunction;
			}
		}

		public PhysicalColumn ColumnName
		{
			get
			{
				return this.columnName;
			}
		}

		public PhysicalColumn ColumnValue
		{
			get
			{
				return this.columnValue;
			}
		}

		public object GetTableContents(IConnectionProvider connectionProvider, object[] parameters)
		{
			if (parameters[0] == null)
			{
				throw new InvalidSerializedFormatException("Blob must not be null");
			}
			return ConditionalIndexMappingBlob.Deserialize((byte[])parameters[0]);
		}

		public object GetColumnFromRow(IConnectionProvider connectionProvider, object row, PhysicalColumn columnToFetch)
		{
			ConditionalIndexMappingBlob conditionalIndexMappingBlob = (ConditionalIndexMappingBlob)row;
			if (columnToFetch == this.ColumnName)
			{
				return conditionalIndexMappingBlob.ColumnName;
			}
			if (columnToFetch == this.ColumnValue)
			{
				return conditionalIndexMappingBlob.ColumnValue;
			}
			return null;
		}

		public const string columnNameName = "columnName";

		public const string columnValueName = "columnValue";

		public const string TableFunctionName = "ConditionalIndexMappingBlob";

		private PhysicalColumn columnName;

		private PhysicalColumn columnValue;

		private TableFunction tableFunction;
	}
}
