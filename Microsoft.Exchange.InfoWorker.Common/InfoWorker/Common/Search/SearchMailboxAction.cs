using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal abstract class SearchMailboxAction : ICloneable
	{
		protected static bool PropertyExists(object property)
		{
			return property != null && !(property is PropertyError);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public abstract SearchMailboxAction Clone();

		public abstract void PerformBatchOperation(object[][] batchedItemBuffer, int fetchedItemCount, StoreId currentFolderId, MailboxSession sourceMailbox, MailboxSession targetMailbox, Dictionary<StoreId, FolderNode> folderNodeMap, SearchResultProcessor processor);

		protected static readonly Trace Tracer = ExTraceGlobals.SearchTracer;

		protected enum ItemPropertyIndex
		{
			Id,
			Size,
			ParentItemId,
			Subject,
			IsRead,
			SentTime,
			ReceivedTime,
			Sender,
			SenderSmtpAddress
		}
	}
}
