using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal class DeleteMessagesOperation : MessageListBulkOperation
	{
		public DeleteMessagesOperation(MapiFolder folder, IList<ExchangeId> messageIds, bool sendNRN, int chunkSize) : base(folder, messageIds, chunkSize)
		{
			this.sendNRN = sendNRN;
		}

		public DeleteMessagesOperation(MapiFolder folder, IList<ExchangeId> messageIds, bool sendNRN) : this(folder, messageIds, sendNRN, 100)
		{
		}

		protected override bool ProcessStart(MapiContext context, out int progressCount, ref ErrorCode error)
		{
			progressCount = 0;
			if (base.MessageIds == null || base.MessageIds.Count == 0)
			{
				base.Folder.StoreFolder.InvalidateIndexes(context, true, false);
			}
			return true;
		}

		protected override void ProcessEnd(MapiContext context, bool incomplete, ErrorCode error)
		{
			if (this.CheckSourceFolder(context))
			{
				BulkOperation.InvalidateFolderIndicesIfNeeded(context, base.Folder.StoreFolder);
			}
		}

		protected override bool ProcessMessages(MapiContext context, MapiFolder folder, IList<ExchangeId> midsToProcess, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			return BulkOperation.DeleteMessages(context, folder, this.sendNRN, false, midsToProcess, BulkErrorAction.Incomplete, BulkErrorAction.Error, out progressCount, ref incomplete, ref error);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DeleteMessagesOperation>(this);
		}

		private bool sendNRN;
	}
}
