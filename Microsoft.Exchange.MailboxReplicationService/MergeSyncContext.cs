using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MergeSyncContext : SyncContext, IEntryIdTranslator
	{
		public MergeSyncContext(MailboxMerger merger) : base(merger.SourceHierarchy, merger.DestHierarchy)
		{
			this.Merger = merger;
		}

		public MailboxMerger Merger { get; private set; }

		public int NumberOfActionsReplayed { get; set; }

		public int NumberOfActionsIgnored { get; set; }

		public ReplayAction LastActionProcessed { get; set; }

		byte[] IEntryIdTranslator.GetSourceFolderIdFromTargetFolderId(byte[] targetFolderId)
		{
			ArgumentValidator.ThrowIfNull("targetFolderId", targetFolderId);
			byte[] array = this.Merger.FolderIdTranslator.TranslateTargetFolderId(targetFolderId);
			if (array == null)
			{
				MrsTracer.Service.Warning("Destination folder {0} doesn't have mapped source folder", new object[]
				{
					TraceUtils.DumpEntryId(targetFolderId)
				});
			}
			return array;
		}

		byte[] IEntryIdTranslator.GetSourceMessageIdFromTargetMessageId(byte[] targetMessageId)
		{
			ArgumentValidator.ThrowIfNull("targetMessageId", targetMessageId);
			byte[] result;
			this.prefetchedSourceEntryIdMap.TryGetValue(targetMessageId, out result);
			return result;
		}

		public override byte[] GetSourceEntryIdFromTargetFolder(FolderRecWrapper targetFolder)
		{
			return this.Merger.GetSourceFolderEntryId(targetFolder);
		}

		public override FolderRecWrapper GetTargetFolderBySourceId(byte[] sourceId)
		{
			byte[] destinationFolderEntryId = this.Merger.GetDestinationFolderEntryId(sourceId);
			if (destinationFolderEntryId == null)
			{
				return null;
			}
			return this.Merger.DestHierarchy[destinationFolderEntryId];
		}

		public override FolderRecWrapper CreateSourceFolderRec(FolderRec fRec)
		{
			return new FolderMapping(fRec);
		}

		public override FolderRecWrapper CreateTargetFolderRec(FolderRecWrapper sourceFolderRec)
		{
			FolderMapping folderMapping = (FolderMapping)sourceFolderRec;
			FolderMapping folderMapping2 = new FolderMapping(sourceFolderRec.FolderRec);
			folderMapping2.SourceFolder = folderMapping;
			folderMapping.TargetFolder = folderMapping2;
			return folderMapping2;
		}

		public void PrefetchSourceMessageIdsFromTargetMessageIds(EntryIdMap<List<byte[]>> destMessagesToTranslate)
		{
			this.prefetchedSourceEntryIdMap.Clear();
			PropTag sourceEntryIDPtag = this.Merger.DestHierarchy.SourceEntryIDPtag;
			PropTag[] additionalPtagsToLoad = new PropTag[]
			{
				sourceEntryIDPtag
			};
			foreach (KeyValuePair<byte[], List<byte[]>> keyValuePair in destMessagesToTranslate)
			{
				byte[] key = keyValuePair.Key;
				List<byte[]> value = keyValuePair.Value;
				foreach (byte[] key2 in value)
				{
					this.prefetchedSourceEntryIdMap[key2] = null;
				}
				using (IDestinationFolder folder = this.Merger.DestMailbox.GetFolder(key))
				{
					if (folder == null)
					{
						MrsTracer.Service.Warning("Destination folder {0} disappeared", new object[]
						{
							TraceUtils.DumpEntryId(key)
						});
					}
					else
					{
						List<MessageRec> list = folder.LookupMessages(PropTag.EntryId, value, additionalPtagsToLoad);
						foreach (MessageRec messageRec in list)
						{
							byte[] array = messageRec[sourceEntryIDPtag] as byte[];
							if (array == null)
							{
								MrsTracer.Service.Warning("Destination message {0} doesn't have mapped source message", new object[]
								{
									TraceUtils.DumpEntryId(messageRec.EntryId)
								});
							}
							else
							{
								this.prefetchedSourceEntryIdMap[messageRec.EntryId] = array;
							}
						}
					}
				}
			}
		}

		private EntryIdMap<byte[]> prefetchedSourceEntryIdMap = new EntryIdMap<byte[]>();
	}
}
