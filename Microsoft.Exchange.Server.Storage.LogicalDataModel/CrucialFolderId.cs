using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class CrucialFolderId
	{
		public void AdjustPrivateMailboxReplids(Context context, IReplidGuidMap replidGuidMap)
		{
			this.FidRoot = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidRoot);
			this.FidIPMsubtree = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidIPMsubtree);
			this.FidDAF = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidDAF);
			this.FidSpoolerQ = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidSpoolerQ);
			this.FidInbox = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidInbox);
			this.FidOutbox = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidOutbox);
			this.FidSentmail = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidSentmail);
			this.FidWastebasket = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidWastebasket);
			this.FidFinder = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidFinder);
			this.FidViews = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidViews);
			this.FidCommonViews = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidCommonViews);
			this.FidSchedule = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidSchedule);
			this.FidShortcuts = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidShortcuts);
			this.FidConversations = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidConversations);
		}

		public void AdjustPublicFoldersMailboxReplids(Context context, IReplidGuidMap replidGuidMap)
		{
			this.FidRoot = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidRoot);
			this.FidPublicFolderIpmSubTree = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidPublicFolderIpmSubTree);
			this.FidPublicFolderNonIpmSubTree = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidPublicFolderNonIpmSubTree);
			this.FidPublicFolderEFormsRegistry = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidPublicFolderEFormsRegistry);
			this.FidPublicFolderDumpsterRoot = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidPublicFolderDumpsterRoot);
			this.FidPublicFolderTombstonesRoot = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidPublicFolderTombstonesRoot);
			this.FidPublicFolderAsyncDeleteState = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidPublicFolderAsyncDeleteState);
			this.FidPublicFolderInternalSubmission = CrucialFolderId.AdjustReplid(context, replidGuidMap, this.FidPublicFolderInternalSubmission);
		}

		private static ExchangeId AdjustReplid(Context context, IReplidGuidMap replidGuidMap, ExchangeId id)
		{
			ushort replidFromGuid = replidGuidMap.GetReplidFromGuid(context, id.Guid);
			if (!id.IsReplidKnown || id.Replid != replidFromGuid)
			{
				if (!ExchangeId.IsGlobCntValid(id.Counter))
				{
					throw new CorruptDataException((LID)62032U, string.Format("Corrupt Global Counter {0:X}.", id.Counter));
				}
				if (!ReplidGuidMap.IsReplidValid(replidFromGuid))
				{
					throw new CorruptDataException((LID)51008U, string.Format("Corrupt Replid {0:X}.", replidFromGuid));
				}
				if (Guid.Empty == id.Guid)
				{
					throw new CorruptDataException((LID)34624U, "Corrupt Guid (zeros).");
				}
				id = ExchangeId.Create(id.Guid, id.Counter, replidFromGuid);
			}
			return id;
		}

		public ExchangeId FidRoot = ExchangeId.Zero;

		public ExchangeId FidIPMsubtree = ExchangeId.Zero;

		public ExchangeId FidDAF = ExchangeId.Zero;

		public ExchangeId FidSpoolerQ = ExchangeId.Zero;

		public ExchangeId FidInbox = ExchangeId.Zero;

		public ExchangeId FidOutbox = ExchangeId.Zero;

		public ExchangeId FidSentmail = ExchangeId.Zero;

		public ExchangeId FidWastebasket = ExchangeId.Zero;

		public ExchangeId FidFinder = ExchangeId.Zero;

		public ExchangeId FidViews = ExchangeId.Zero;

		public ExchangeId FidCommonViews = ExchangeId.Zero;

		public ExchangeId FidSchedule = ExchangeId.Zero;

		public ExchangeId FidShortcuts = ExchangeId.Zero;

		public ExchangeId FidConversations = ExchangeId.Zero;

		public ExchangeId FidPublicFolderIpmSubTree = ExchangeId.Zero;

		public ExchangeId FidPublicFolderNonIpmSubTree = ExchangeId.Zero;

		public ExchangeId FidPublicFolderEFormsRegistry = ExchangeId.Zero;

		public ExchangeId FidPublicFolderDumpsterRoot = ExchangeId.Zero;

		public ExchangeId FidPublicFolderTombstonesRoot = ExchangeId.Zero;

		public ExchangeId FidPublicFolderAsyncDeleteState = ExchangeId.Zero;

		public ExchangeId FidPublicFolderInternalSubmission = ExchangeId.Zero;
	}
}
