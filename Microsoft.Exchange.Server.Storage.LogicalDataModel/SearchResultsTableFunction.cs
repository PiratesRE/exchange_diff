using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class SearchResultsTableFunction
	{
		internal SearchResultsTableFunction()
		{
			this.sortPosition = Factory.CreatePhysicalColumn("SortPosition", "SortPosition", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.messageDocumentId = Factory.CreatePhysicalColumn("MessageDocumentId", "MessageDocumentId", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.conversationDocumentId = Factory.CreatePhysicalColumn("ConversationDocumentId", "ConversationDocumentId", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.count = Factory.CreatePhysicalColumn("Count", "Count", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
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
				this.SortPosition,
				this.Count
			});
			Index[] indexes = new Index[]
			{
				index
			};
			this.tableFunction = Factory.CreateTableFunction("SearchResults", new TableFunction.GetTableContentsDelegate(this.GetTableContents), new TableFunction.GetColumnFromRowDelegate(this.GetColumnFromRow), Visibility.Redacted, new Type[]
			{
				typeof(IList<SearchFolder.InstantSearchResultsEntry>)
			}, indexes, new PhysicalColumn[]
			{
				this.SortPosition,
				this.MessageDocumentId,
				this.ConversationDocumentId,
				this.Count
			});
		}

		public TableFunction TableFunction
		{
			get
			{
				return this.tableFunction;
			}
		}

		public PhysicalColumn SortPosition
		{
			get
			{
				return this.sortPosition;
			}
		}

		public PhysicalColumn MessageDocumentId
		{
			get
			{
				return this.messageDocumentId;
			}
		}

		public PhysicalColumn ConversationDocumentId
		{
			get
			{
				return this.conversationDocumentId;
			}
		}

		public PhysicalColumn Count
		{
			get
			{
				return this.count;
			}
		}

		public object GetTableContents(IConnectionProvider connectionProvider, object[] parameters)
		{
			return (IList<SearchFolder.InstantSearchResultsEntry>)parameters[0];
		}

		public object GetColumnFromRow(IConnectionProvider connectionProvider, object row, PhysicalColumn columnToFetch)
		{
			SearchFolder.InstantSearchResultsEntry instantSearchResultsEntry = (SearchFolder.InstantSearchResultsEntry)row;
			if (columnToFetch == this.SortPosition)
			{
				return instantSearchResultsEntry.SortPosition;
			}
			if (columnToFetch == this.MessageDocumentId)
			{
				return instantSearchResultsEntry.MessageDocumentId;
			}
			if (columnToFetch == this.ConversationDocumentId)
			{
				return instantSearchResultsEntry.ConversationDocumentId;
			}
			if (columnToFetch == this.Count)
			{
				return instantSearchResultsEntry.Count;
			}
			return null;
		}

		public const string SortPositionName = "SortPosition";

		public const string MessageDocumentIdName = "MessageDocumentId";

		public const string ConversationDocumentIdName = "ConversationDocumentId";

		public const string CountName = "Count";

		public const string TableFunctionName = "SearchResults";

		private PhysicalColumn sortPosition;

		private PhysicalColumn messageDocumentId;

		private PhysicalColumn conversationDocumentId;

		private PhysicalColumn count;

		private TableFunction tableFunction;
	}
}
