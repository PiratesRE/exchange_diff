using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class QuerySegmentEnumerator : SegmentEnumerator
	{
		public QuerySegmentEnumerator(CoreFolder coreFolder, ItemQueryType itemQueryType, int segmentSize) : base(segmentSize)
		{
			this.queryResult = coreFolder.QueryExecutor.ItemQuery(itemQueryType, null, null, new PropertyDefinition[]
			{
				ItemSchema.Id
			});
		}

		public QuerySegmentEnumerator(CoreFolder coreFolder, FolderQueryFlags folderQueryFlags, int segmentSize) : base(segmentSize)
		{
			this.queryResult = coreFolder.QueryExecutor.FolderQuery(folderQueryFlags, null, null, new PropertyDefinition[]
			{
				FolderSchema.Id
			});
		}

		public override StoreObjectId[] GetNextBatchIds()
		{
			object[][] rows = this.queryResult.GetRows(base.SegmentSize);
			StoreObjectId[] array = new StoreObjectId[rows.Length];
			int num = 0;
			foreach (object[] array3 in rows)
			{
				array[num++] = StoreId.GetStoreObjectId((StoreId)array3[0]);
			}
			return array;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<QuerySegmentEnumerator>(this);
		}

		protected override void InternalDispose()
		{
			this.queryResult.Dispose();
			base.InternalDispose();
		}

		private readonly QueryResult queryResult;
	}
}
