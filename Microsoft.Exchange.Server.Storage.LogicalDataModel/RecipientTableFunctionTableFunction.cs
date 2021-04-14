using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class RecipientTableFunctionTableFunction
	{
		internal RecipientTableFunctionTableFunction()
		{
			this.recipientBlob = Factory.CreatePhysicalColumn("RecipientBlob", "RecipientBlob", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
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
				this.RecipientBlob
			});
			Index[] indexes = new Index[]
			{
				index
			};
			this.tableFunction = Factory.CreateTableFunction("RecipientTableFunction", new TableFunction.GetTableContentsDelegate(this.GetTableContents), new TableFunction.GetColumnFromRowDelegate(this.GetColumnFromRow), Visibility.Public, new Type[0], indexes, new PhysicalColumn[]
			{
				this.RecipientBlob
			});
		}

		public TableFunction TableFunction
		{
			get
			{
				return this.tableFunction;
			}
		}

		public PhysicalColumn RecipientBlob
		{
			get
			{
				return this.recipientBlob;
			}
		}

		public object GetTableContents(IConnectionProvider connectionProvider, object[] parameters)
		{
			return new object[0];
		}

		public object GetColumnFromRow(IConnectionProvider connectionProvider, object row, PhysicalColumn columnToFetch)
		{
			if (columnToFetch == this.RecipientBlob)
			{
				return null;
			}
			return null;
		}

		public const string RecipientBlobName = "RecipientBlob";

		public const string TableFunctionName = "RecipientTableFunction";

		private PhysicalColumn recipientBlob;

		private TableFunction tableFunction;
	}
}
