using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public sealed class IndexDefinitionBlobHeaderTableFunction
	{
		internal IndexDefinitionBlobHeaderTableFunction()
		{
			this.keyColumnCount = Factory.CreatePhysicalColumn("keyColumnCount", "keyColumnCount", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.lcid = Factory.CreatePhysicalColumn("lcid", "lcid", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.identityColumnIndex = Factory.CreatePhysicalColumn("identityColumnIndex", "identityColumnIndex", typeof(short), false, false, false, false, false, Visibility.Public, 0, 2, 2);
			string name = "PrimaryKey";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[3];
			Index index = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true,
				true
			}, new PhysicalColumn[]
			{
				this.keyColumnCount,
				this.lcid,
				this.identityColumnIndex
			});
			Index[] indexes = new Index[]
			{
				index
			};
			this.tableFunction = Factory.CreateTableFunction("IndexDefinitionBlobHeader", new TableFunction.GetTableContentsDelegate(this.GetTableContents), new TableFunction.GetColumnFromRowDelegate(this.GetColumnFromRow), Visibility.Public, new Type[]
			{
				typeof(byte[])
			}, indexes, new PhysicalColumn[]
			{
				this.keyColumnCount,
				this.lcid,
				this.identityColumnIndex
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

		public PhysicalColumn Lcid
		{
			get
			{
				return this.lcid;
			}
		}

		public PhysicalColumn IdentityColumnIndex
		{
			get
			{
				return this.identityColumnIndex;
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
			IndexDefinitionBlob.Deserialize(out num, out num2, out num3, (byte[])parameters[0]);
			return new IndexDefinitionBlobHeaderTableFunction.IndexDefinitionBlobHeader[]
			{
				new IndexDefinitionBlobHeaderTableFunction.IndexDefinitionBlobHeader(num, num2, num3)
			};
		}

		public object GetColumnFromRow(IConnectionProvider connectionProvider, object row, PhysicalColumn columnToFetch)
		{
			IndexDefinitionBlobHeaderTableFunction.IndexDefinitionBlobHeader indexDefinitionBlobHeader = (IndexDefinitionBlobHeaderTableFunction.IndexDefinitionBlobHeader)row;
			if (columnToFetch == this.KeyColumnCount)
			{
				return indexDefinitionBlobHeader.KeyColumnCount;
			}
			if (columnToFetch == this.Lcid)
			{
				return indexDefinitionBlobHeader.Lcid;
			}
			if (columnToFetch == this.IdentityColumnIndex)
			{
				return indexDefinitionBlobHeader.IdentityColumnIndex;
			}
			return null;
		}

		public const string keyColumnCountName = "keyColumnCount";

		public const string lcidName = "lcid";

		public const string identityColumnIndexName = "identityColumnIndex";

		public const string TableFunctionName = "IndexDefinitionBlobHeader";

		private PhysicalColumn keyColumnCount;

		private PhysicalColumn lcid;

		private PhysicalColumn identityColumnIndex;

		private TableFunction tableFunction;

		private class IndexDefinitionBlobHeader
		{
			public IndexDefinitionBlobHeader(int keyColumnCount, int lcid, short identityColumnIndex)
			{
				this.keyColumnCount = keyColumnCount;
				this.lcid = lcid;
				this.identityColumnIndex = identityColumnIndex;
			}

			public int KeyColumnCount
			{
				get
				{
					return this.keyColumnCount;
				}
			}

			public int Lcid
			{
				get
				{
					return this.lcid;
				}
			}

			public short IdentityColumnIndex
			{
				get
				{
					return this.identityColumnIndex;
				}
			}

			private readonly int keyColumnCount;

			private readonly int lcid;

			private readonly short identityColumnIndex;
		}
	}
}
