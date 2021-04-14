using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class ConversationMembersBlobTableFunction
	{
		internal ConversationMembersBlobTableFunction()
		{
			this.folderId = Factory.CreatePhysicalColumn("FolderId", "FolderId", typeof(byte[]), false, false, false, false, false, Visibility.Public, 0, 26, 26);
			this.messageId = Factory.CreatePhysicalColumn("MessageId", "MessageId", typeof(byte[]), false, false, false, false, false, Visibility.Public, 0, 26, 26);
			this.sortPosition = Factory.CreatePhysicalColumn("SortPosition", "SortPosition", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
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
				this.SortPosition
			});
			Index[] indexes = new Index[]
			{
				index
			};
			this.tableFunction = Factory.CreateTableFunction("ConversationMembersBlob", new TableFunction.GetTableContentsDelegate(this.GetTableContents), new TableFunction.GetColumnFromRowDelegate(this.GetColumnFromRow), Visibility.Public, new Type[]
			{
				typeof(byte[])
			}, indexes, new PhysicalColumn[]
			{
				this.FolderId,
				this.MessageId,
				this.SortPosition
			});
		}

		public TableFunction TableFunction
		{
			get
			{
				return this.tableFunction;
			}
		}

		public PhysicalColumn FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public PhysicalColumn MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		public PhysicalColumn SortPosition
		{
			get
			{
				return this.sortPosition;
			}
		}

		public object GetTableContents(IConnectionProvider connectionProvider, object[] parameters)
		{
			if (parameters[0] == null)
			{
				throw new InvalidSerializedFormatException("Blob must not be null");
			}
			return ConversationMembersBlob.Deserialize((byte[])parameters[0]);
		}

		public object GetColumnFromRow(IConnectionProvider connectionProvider, object row, PhysicalColumn columnToFetch)
		{
			ConversationMembersBlob conversationMembersBlob = (ConversationMembersBlob)row;
			if (columnToFetch == this.FolderId)
			{
				return conversationMembersBlob.FolderId;
			}
			if (columnToFetch == this.MessageId)
			{
				return conversationMembersBlob.MessageId;
			}
			if (columnToFetch == this.SortPosition)
			{
				return conversationMembersBlob.SortPosition;
			}
			return null;
		}

		public const string FolderIdName = "FolderId";

		public const string MessageIdName = "MessageId";

		public const string SortPositionName = "SortPosition";

		public const string TableFunctionName = "ConversationMembersBlob";

		private PhysicalColumn folderId;

		private PhysicalColumn messageId;

		private PhysicalColumn sortPosition;

		private TableFunction tableFunction;
	}
}
