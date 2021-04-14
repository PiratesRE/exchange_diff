using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal class MoveCopyMessagesOperation : MessageListBulkOperation
	{
		public MoveCopyMessagesOperation(bool copy, MapiFolder sourceFolder, MapiFolder destinationFolder, IList<ExchangeId> messageIds, Properties propsToSet, IList<ExchangeId> outputMids, IList<ExchangeId> outputCns, int chunkSize) : base(sourceFolder, messageIds, chunkSize)
		{
			this.copy = copy;
			this.destinationFolder = destinationFolder;
			this.propsToSet = propsToSet;
			this.outputMids = outputMids;
			this.outputCns = outputCns;
		}

		public MoveCopyMessagesOperation(bool copy, MapiFolder sourceFolder, MapiFolder destinationFolder, IList<ExchangeId> messageIds, Properties propsToSet, IList<ExchangeId> outputMids, IList<ExchangeId> outputCns) : this(copy, sourceFolder, destinationFolder, messageIds, propsToSet, outputMids, outputCns, 100)
		{
		}

		internal static IDisposable SetIdleIndexTimeForEmptyFolderOperation(TimeSpan idleTime)
		{
			return MoveCopyMessagesOperation.idleIndexTimeForEmptyFolderOperation.SetTestHook(idleTime);
		}

		protected bool CheckDestinationFolder(MapiContext context)
		{
			return this.destinationFolder.CheckAlive(context);
		}

		protected override bool ProcessStart(MapiContext context, out int progressCount, ref ErrorCode error)
		{
			progressCount = 0;
			if (!this.CheckDestinationFolder(context))
			{
				error = ErrorCode.CreateObjectDeleted((LID)53784U);
				return false;
			}
			if (this.destinationFolder.IsSearchFolder())
			{
				progressCount = 0;
				error = ErrorCode.CreateSearchFolder((LID)41496U);
				return false;
			}
			if (!this.copy)
			{
				if (base.MessageIds == null || base.MessageIds.Count == 0)
				{
					base.Folder.StoreFolder.InvalidateIndexes(context, true, false);
				}
				else
				{
					bool flag = context.Diagnostics.ClientActionString != null && context.Diagnostics.ClientActionString.Contains("EmptyFolder");
					if (flag)
					{
						DateTime lastReferenceDateThreshold = base.Folder.Logon.StoreMailbox.UtcNow - MoveCopyMessagesOperation.idleIndexTimeForEmptyFolderOperation.Value;
						base.Folder.StoreFolder.InvalidateIndexes(context, true, false, lastReferenceDateThreshold);
					}
				}
			}
			return true;
		}

		protected override void ProcessEnd(MapiContext context, bool incomplete, ErrorCode error)
		{
			if (!this.copy && this.CheckSourceFolder(context))
			{
				BulkOperation.InvalidateFolderIndicesIfNeeded(context, base.Folder.StoreFolder);
			}
		}

		protected override bool ProcessMessages(MapiContext context, MapiFolder folder, IList<ExchangeId> midsToProcess, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			if (!this.CheckDestinationFolder(context))
			{
				progressCount = 0;
				error = ErrorCode.CreateObjectDeleted((LID)57880U);
				return false;
			}
			BulkErrorAction softErrorAction = (midsToProcess.Count == 1) ? BulkErrorAction.Error : BulkErrorAction.Incomplete;
			if (this.copy)
			{
				return BulkOperation.CopyMessages(context, folder, this.destinationFolder, midsToProcess, this.propsToSet, BulkErrorAction.Incomplete, softErrorAction, this.outputMids, this.outputCns, out progressCount, ref incomplete, ref error);
			}
			return BulkOperation.MoveMessages(context, folder, this.destinationFolder, midsToProcess, this.propsToSet, BulkErrorAction.Incomplete, softErrorAction, this.outputMids, this.outputCns, out progressCount, ref incomplete, ref error);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MoveCopyMessagesOperation>(this);
		}

		private static Hookable<TimeSpan> idleIndexTimeForEmptyFolderOperation = Hookable<TimeSpan>.Create(false, DefaultSettings.Get.IdleIndexTimeForEmptyFolderOperation);

		private bool copy;

		private MapiFolder destinationFolder;

		private Properties propsToSet;

		private IList<ExchangeId> outputMids;

		private IList<ExchangeId> outputCns;
	}
}
