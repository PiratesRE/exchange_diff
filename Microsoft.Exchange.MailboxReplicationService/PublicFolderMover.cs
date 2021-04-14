using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PublicFolderMover : MailboxCopierBase
	{
		internal PublicFolderMover(TransactionalRequestJob moveRequestJob, BaseJob publicFolderMoveJob, List<byte[]> hierarchyFolderEntryIds, MailboxCopierFlags copierFlags, LocalizedString sourceTracingID, LocalizedString targetTracingID) : base(moveRequestJob.SourceExchangeGuid, moveRequestJob.TargetExchangeGuid, moveRequestJob, publicFolderMoveJob, copierFlags, sourceTracingID, targetTracingID)
		{
			MrsTracer.Service.Function("PublicFolderMover.Constructor", new object[0]);
			this.hierarchyFolderEntryIds = hierarchyFolderEntryIds;
			this.sessionSpecificEntryIds = new Dictionary<byte[], byte[]>(this.hierarchyFolderEntryIds.Count, ArrayComparer<byte>.Comparer);
		}

		public override void ConfigureProviders()
		{
			base.ConfigDestinationMailbox(this.destinationMailbox);
		}

		public override void UnconfigureProviders()
		{
			base.SourceMailboxWrapper = null;
			base.UnconfigureProviders();
		}

		public override FolderMap GetSourceFolderMap(GetFolderMapFlags flags)
		{
			base.SourceMailboxWrapper.LoadFolderMap(flags, delegate
			{
				List<FolderRecWrapper> list = new List<FolderRecWrapper>(this.hierarchyFolderEntryIds.Count);
				foreach (byte[] entryId in this.hierarchyFolderEntryIds)
				{
					byte[] sessionSpecificEntryId = base.SourceMailbox.GetSessionSpecificEntryId(entryId);
					using (ISourceFolder folder = base.SourceMailbox.GetFolder(sessionSpecificEntryId))
					{
						list.Add(new FolderRecWrapper(folder.GetFolderRec(null, GetFolderRecFlags.None)));
					}
				}
				return new PublicFolderMap(list);
			});
			return base.SourceMailboxWrapper.FolderMap;
		}

		public override FolderMap GetDestinationFolderMap(GetFolderMapFlags flags)
		{
			base.DestMailboxWrapper.LoadFolderMap(flags, delegate
			{
				List<FolderRecWrapper> list = new List<FolderRecWrapper>(this.hierarchyFolderEntryIds.Count);
				foreach (byte[] entryId in this.hierarchyFolderEntryIds)
				{
					byte[] sessionSpecificEntryId = base.DestMailbox.GetSessionSpecificEntryId(entryId);
					using (IDestinationFolder folder = base.DestMailbox.GetFolder(sessionSpecificEntryId))
					{
						list.Add(new FolderRecWrapper(folder.GetFolderRec(null, GetFolderRecFlags.None)));
					}
				}
				return new PublicFolderMap(list);
			});
			return base.DestMailboxWrapper.FolderMap;
		}

		public override IEnumerator<FolderRecWrapper> GetSourceHierarchyEnumeratorForChangedFolders()
		{
			return this.GetSourceFolderMap(GetFolderMapFlags.None).GetFolderHierarchyEnumerator(EnumHierarchyFlags.AllFolders);
		}

		public override byte[] GetSourceFolderEntryId(FolderRecWrapper destinationFolderRec)
		{
			return base.SourceMailbox.GetSessionSpecificEntryId(destinationFolderRec.EntryId);
		}

		public override byte[] GetDestinationFolderEntryId(byte[] srcFolderEntryId)
		{
			return base.DestMailbox.GetSessionSpecificEntryId(srcFolderEntryId);
		}

		public override IFxProxyPool GetDestinationFxProxyPool(ICollection<byte[]> folderIds)
		{
			IFxProxyPool fxProxyPool = base.DestMailbox.GetFxProxyPool(folderIds);
			return new TranslatorPFProxy(base.SourceMailbox, base.DestMailbox, fxProxyPool);
		}

		public override bool IsContentAvailableInTargetMailbox(FolderRecWrapper destinationFolderRec)
		{
			return this.sessionSpecificEntryIds.ContainsKey(destinationFolderRec.EntryId);
		}

		protected override bool ShouldCompareParentIDs()
		{
			return false;
		}

		protected override EnumerateMessagesFlags GetAdditionalEnumerateMessagesFlagsForContentVerification()
		{
			return EnumerateMessagesFlags.ReturnLongTermIDs;
		}

		public override void CopyFolderProperties(FolderRecWrapper sourceFolderRecWrapper, ISourceFolder sourceFolder, IDestinationFolder destFolder, FolderRecDataFlags dataToCopy, out bool wasPropertyCopyingSkipped)
		{
			wasPropertyCopyingSkipped = false;
			if (this.sessionSpecificEntryIds.ContainsKey(sourceFolderRecWrapper.EntryId) && destFolder != null)
			{
				if (base.SupportsPerUserReadUnreadDataTransfer)
				{
					using (IFxProxy fxProxy = destFolder.GetFxProxy(FastTransferFlags.PassThrough))
					{
						using (IFxProxy fxProxy2 = base.CreateFxProxyTransmissionPipeline(fxProxy))
						{
							sourceFolder.CopyTo(fxProxy2, CopyPropertiesFlags.CopyFolderPerUserData, Array<PropTag>.Empty);
						}
					}
				}
				base.CopyFolderProperties(sourceFolderRecWrapper, sourceFolder, destFolder, FolderRecDataFlags.Rules, out wasPropertyCopyingSkipped);
			}
		}

		public override SyncContext CreateSyncContext()
		{
			return new PublicFolderMoveSyncContext(base.SourceMailbox, this.GetSourceFolderMap(GetFolderMapFlags.ForceRefresh), base.DestMailbox, this.GetDestinationFolderMap(GetFolderMapFlags.ForceRefresh));
		}

		protected override byte[] GetMessageKey(MessageRec messageRec, MailboxWrapperFlags flags)
		{
			return (byte[])messageRec[PropTag.LTID];
		}

		internal void SetMailboxWrappers(SourceMailboxWrapper sourceMailboxWrapper, IDestinationMailbox destinationMailbox)
		{
			base.SourceMailboxWrapper = sourceMailboxWrapper;
			this.destinationMailbox = destinationMailbox;
		}

		internal void UpdateSourceDestinationFolderIds()
		{
			foreach (byte[] entryId in this.hierarchyFolderEntryIds)
			{
				byte[] sessionSpecificEntryId = base.SourceMailbox.GetSessionSpecificEntryId(entryId);
				byte[] sessionSpecificEntryId2 = base.DestMailbox.GetSessionSpecificEntryId(entryId);
				this.sessionSpecificEntryIds[sessionSpecificEntryId] = sessionSpecificEntryId2;
				this.sessionSpecificEntryIds[sessionSpecificEntryId2] = sessionSpecificEntryId;
			}
		}

		private IDestinationMailbox destinationMailbox;

		private List<byte[]> hierarchyFolderEntryIds;

		private Dictionary<byte[], byte[]> sessionSpecificEntryIds;
	}
}
