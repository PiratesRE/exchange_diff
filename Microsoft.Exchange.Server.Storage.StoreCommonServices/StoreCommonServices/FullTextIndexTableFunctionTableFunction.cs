using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.FullTextIndex;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public sealed class FullTextIndexTableFunctionTableFunction
	{
		internal FullTextIndexTableFunctionTableFunction()
		{
			this.mailboxPartitionNumber = Factory.CreatePhysicalColumn("MailboxPartitionNumber", "MailboxPartitionNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.messageDocumentId = Factory.CreatePhysicalColumn("MessageDocumentId", "MessageDocumentId", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
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
				this.MailboxPartitionNumber,
				this.MessageDocumentId
			});
			Index[] indexes = new Index[]
			{
				index
			};
			this.tableFunction = Factory.CreateTableFunction("FullTextIndexTableFunction", new TableFunction.GetTableContentsDelegate(this.GetTableContents), new TableFunction.GetColumnFromRowDelegate(this.GetColumnFromRow), Visibility.Redacted, new Type[]
			{
				typeof(StoreFullTextIndexQuery)
			}, indexes, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.MessageDocumentId
			});
		}

		public TableFunction TableFunction
		{
			get
			{
				return this.tableFunction;
			}
		}

		public PhysicalColumn MailboxPartitionNumber
		{
			get
			{
				return this.mailboxPartitionNumber;
			}
		}

		public PhysicalColumn MessageDocumentId
		{
			get
			{
				return this.messageDocumentId;
			}
		}

		public object GetTableContents(IConnectionProvider connectionProvider, object[] parameters)
		{
			return (StoreFullTextIndexQuery)parameters[0];
		}

		public object GetColumnFromRow(IConnectionProvider connectionProvider, object row, PhysicalColumn columnToFetch)
		{
			FullTextIndexRow fullTextIndexRow = (FullTextIndexRow)row;
			if (columnToFetch == this.MailboxPartitionNumber)
			{
				return fullTextIndexRow.MailboxNumber;
			}
			if (columnToFetch == this.MessageDocumentId)
			{
				return fullTextIndexRow.DocumentId;
			}
			return null;
		}

		public const string MailboxPartitionNumberName = "MailboxPartitionNumber";

		public const string MessageDocumentIdName = "MessageDocumentId";

		public const string TableFunctionName = "FullTextIndexTableFunction";

		private PhysicalColumn mailboxPartitionNumber;

		private PhysicalColumn messageDocumentId;

		private TableFunction tableFunction;
	}
}
