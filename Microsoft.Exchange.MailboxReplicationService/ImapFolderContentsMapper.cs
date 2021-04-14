using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ImapFolderContentsMapper : FolderContentsMapper
	{
		public ImapFolderContentsMapper(FolderMapping folderMapping, ISourceFolder srcFolder, FolderHierarchy sourceHierarchy, IDestinationFolder destFolder, FolderHierarchy destHierarchy, ConflictResolutionOption conflictResolutionOption, FAICopyOption faiCopyOption, FolderContentsMapperFlags mapperFlags) : base(folderMapping, srcFolder, sourceHierarchy, destFolder, destHierarchy, conflictResolutionOption, faiCopyOption, mapperFlags)
		{
		}

		protected override List<byte[]> GetSecondaryKeys(MessageRec message, MessageRecType messageRecType)
		{
			if (!messageRecType.Equals(MessageRecType.Source))
			{
				return new List<byte[]>(2)
				{
					ImapFolderContentsMapper.GetImapSyncPropertiesHash(message, this.destHierarchy),
					ImapFolderContentsMapper.GetCloudIdHash(message, messageRecType, this.destHierarchy)
				};
			}
			return new List<byte[]>(2)
			{
				ImapFolderContentsMapper.GetImapSyncPropertiesHash(message, this.sourceHierarchy),
				ImapFolderContentsMapper.GetSyncMessageIdHash(message, messageRecType, this.sourceHierarchy)
			};
		}

		protected override PropTag[] GetSourcePTagsInitialSync()
		{
			return new PropTag[]
			{
				this.sourceHierarchy.SourceSyncAccountNamePtag,
				this.sourceHierarchy.SourceSyncMessageIdPtag,
				this.sourceHierarchy.SourceSyncFolderIdPtag
			};
		}

		protected override PropTag[] GetTargetPTagsInitialSync()
		{
			return new PropTag[]
			{
				this.destHierarchy.SourceEntryIDPtag,
				this.destHierarchy.SourceLastModifiedTimestampPtag,
				this.destHierarchy.SourceSyncAccountNamePtag,
				this.destHierarchy.SourceSyncFolderIdPtag,
				this.destHierarchy.SourceSyncMessageIdPtag,
				this.destHierarchy.SharingInstanceGuidPtag,
				this.destHierarchy.CloudIdPtag
			};
		}

		protected override void MapSourceToTargetMessage(MessageRec sourceMR, EntryIdMap<MessageRec> targetBySourceEntryId, EntryIdMap<MessageRec> targetBySecondaryKey, HashSet<byte[]> duplicateTargetSecondaryKeys, HashSet<byte[]> duplicateSourceSecondaryKeys, out MessageRec targetMR)
		{
			targetMR = null;
			byte[] array = ImapFolderContentsMapper.GetImapSyncPropertiesHash(sourceMR, this.sourceHierarchy);
			if (array != null)
			{
				targetBySecondaryKey.TryGetValue(array, out targetMR);
			}
			if (targetMR == null)
			{
				array = ImapFolderContentsMapper.GetSyncMessageIdHash(sourceMR, MessageRecType.Source, this.sourceHierarchy);
				targetBySecondaryKey.TryGetValue(array, out targetMR);
			}
		}

		protected override bool ShouldTargetMessagePropsBeUpdated(MessageRec sourceMR, MessageRec targetMR, out MessageRec updatedMessageRec)
		{
			updatedMessageRec = null;
			if (targetMR == null)
			{
				return false;
			}
			byte[] array = targetMR[this.destHierarchy.SourceEntryIDPtag] as byte[];
			if (array != null)
			{
				return this.ShouldStampNewSourceEntryId(sourceMR, targetMR, out updatedMessageRec);
			}
			return this.ShouldStampSourceSyncProperties(sourceMR, targetMR, out updatedMessageRec);
		}

		protected override bool ShouldItemBeCopied(MessageRec sourceMR, MessageRec targetMR, HashSet<byte[]> duplicateTargetSecondaryKeys)
		{
			byte[] imapSyncPropertiesHash = ImapFolderContentsMapper.GetImapSyncPropertiesHash(sourceMR, this.sourceHierarchy);
			return (duplicateTargetSecondaryKeys == null || !duplicateTargetSecondaryKeys.Contains(imapSyncPropertiesHash)) && targetMR == null;
		}

		private static byte[] GetImapSyncPropertiesHash(MessageRec message, FolderHierarchy hierarchy)
		{
			string text = message[hierarchy.SourceSyncMessageIdPtag] as string;
			string text2 = message[hierarchy.SourceSyncAccountNamePtag] as string;
			byte[] array = message[hierarchy.SourceSyncFolderIdPtag] as byte[];
			if (text != null && text2 != null && array != null)
			{
				byte[] sha1Hash = CommonUtils.GetSHA1Hash(text2.ToLowerInvariant() + text.ToLowerInvariant());
				byte[] array2 = new byte[array.Length + sha1Hash.Length];
				Array.Copy(array, 0, array2, 0, array.Length);
				Array.Copy(sha1Hash, 0, array2, array.Length, sha1Hash.Length);
				return array2;
			}
			return null;
		}

		private static byte[] GetSyncMessageIdHash(MessageRec message, MessageRecType messageRecType, FolderHierarchy hierarchy)
		{
			string text = message[hierarchy.SourceSyncMessageIdPtag] as string;
			if (!string.IsNullOrEmpty(text))
			{
				return CommonUtils.GetSHA1Hash(text.ToLowerInvariant());
			}
			return null;
		}

		private static byte[] GetCloudIdHash(MessageRec message, MessageRecType messageRecType, FolderHierarchy hierarchy)
		{
			if (message[hierarchy.SharingInstanceGuidPtag] == null || Guid.Empty.Equals((Guid)message[hierarchy.SharingInstanceGuidPtag]))
			{
				return null;
			}
			string text = message[hierarchy.CloudIdPtag] as string;
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(new char[]
				{
					' '
				});
				if (array.Length == 2)
				{
					return CommonUtils.GetSHA1Hash(array[1].ToLowerInvariant());
				}
			}
			return null;
		}

		private bool ShouldStampSourceSyncProperties(MessageRec sourceMR, MessageRec targetMR, out MessageRec updatedMessageRec)
		{
			updatedMessageRec = null;
			byte[] array = targetMR[this.destHierarchy.SourceEntryIDPtag] as byte[];
			if (array != null)
			{
				return false;
			}
			List<PropValueData> list = new List<PropValueData>(4)
			{
				new PropValueData(this.destHierarchy.SourceEntryIDPtag, sourceMR.EntryId),
				new PropValueData(this.destHierarchy.SourceSyncFolderIdPtag, sourceMR.FolderId),
				new PropValueData(this.destHierarchy.SourceSyncAccountNamePtag, sourceMR[this.sourceHierarchy.SourceSyncAccountNamePtag]),
				new PropValueData(this.destHierarchy.SourceSyncMessageIdPtag, sourceMR[this.sourceHierarchy.SourceSyncMessageIdPtag])
			};
			updatedMessageRec = new MessageRec(targetMR.EntryId, targetMR.FolderId, CommonUtils.DefaultLastModificationTime, 0, MsgRecFlags.None, list.ToArray());
			return true;
		}

		private bool ShouldStampNewSourceEntryId(MessageRec sourceMR, MessageRec targetMR, out MessageRec updatedMessageRec)
		{
			updatedMessageRec = null;
			byte[] array = targetMR[this.destHierarchy.SourceEntryIDPtag] as byte[];
			byte[] imapSyncPropertiesHash = ImapFolderContentsMapper.GetImapSyncPropertiesHash(sourceMR, this.sourceHierarchy);
			byte[] imapSyncPropertiesHash2 = ImapFolderContentsMapper.GetImapSyncPropertiesHash(targetMR, this.destHierarchy);
			if (imapSyncPropertiesHash == null || imapSyncPropertiesHash2 == null || array == null)
			{
				return false;
			}
			if (imapSyncPropertiesHash.SequenceEqual(imapSyncPropertiesHash2) && !array.SequenceEqual(sourceMR.EntryId))
			{
				List<PropValueData> list = new List<PropValueData>(1)
				{
					new PropValueData(this.destHierarchy.SourceEntryIDPtag, sourceMR.EntryId)
				};
				updatedMessageRec = new MessageRec(targetMR.EntryId, targetMR.FolderId, CommonUtils.DefaultLastModificationTime, 0, MsgRecFlags.None, list.ToArray());
				return true;
			}
			return false;
		}

		private const char TxSyncCloudIdDelimiter = ' ';

		private const int TxSyncCloudIdTokenCount = 2;
	}
}
