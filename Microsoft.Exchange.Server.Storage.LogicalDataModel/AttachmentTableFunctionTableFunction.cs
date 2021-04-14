using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.AttachmentBlob;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class AttachmentTableFunctionTableFunction
	{
		internal AttachmentTableFunctionTableFunction()
		{
			this.attachmentNumber = Factory.CreatePhysicalColumn("AttachmentNumber", "AttachmentNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.inid = Factory.CreatePhysicalColumn("Inid", "Inid", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
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
				this.AttachmentNumber
			});
			Index[] indexes = new Index[]
			{
				index
			};
			this.tableFunction = Factory.CreateTableFunction("AttachmentTableFunction", new TableFunction.GetTableContentsDelegate(this.GetTableContents), new TableFunction.GetColumnFromRowDelegate(this.GetColumnFromRow), Visibility.Redacted, new Type[]
			{
				typeof(byte[])
			}, indexes, new PhysicalColumn[]
			{
				this.AttachmentNumber,
				this.Inid
			});
		}

		public TableFunction TableFunction
		{
			get
			{
				return this.tableFunction;
			}
		}

		public PhysicalColumn AttachmentNumber
		{
			get
			{
				return this.attachmentNumber;
			}
		}

		public PhysicalColumn Inid
		{
			get
			{
				return this.inid;
			}
		}

		public object GetTableContents(IConnectionProvider connectionProvider, object[] parameters)
		{
			if (parameters != null)
			{
				return AttachmentBlob.Deserialize((byte[])parameters[0], true);
			}
			return Enumerable.Empty<KeyValuePair<int, long>>();
		}

		public object GetColumnFromRow(IConnectionProvider connectionProvider, object row, PhysicalColumn columnToFetch)
		{
			KeyValuePair<int, long> keyValuePair = (KeyValuePair<int, long>)row;
			if (columnToFetch == this.AttachmentNumber)
			{
				return keyValuePair.Key;
			}
			if (columnToFetch == this.Inid)
			{
				return keyValuePair.Value;
			}
			return null;
		}

		public const string AttachmentNumberName = "AttachmentNumber";

		public const string InidName = "Inid";

		public const string TableFunctionName = "AttachmentTableFunction";

		private PhysicalColumn attachmentNumber;

		private PhysicalColumn inid;

		private TableFunction tableFunction;
	}
}
