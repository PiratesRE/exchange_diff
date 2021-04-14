using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class SyncEmailUtils
	{
		public static PropValueData[] GetMessageProps(SyncEmailContext context, string accountName, byte[] folderId, params PropValueData[] extraProps)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("accountName", accountName);
			ArgumentValidator.ThrowIfNull("folderId", folderId);
			List<PropValueData> list = new List<PropValueData>(SyncEmailUtils.SyncEmailPropTags.Length + 3);
			list.Add(new PropValueData(SyncEmailUtils.SyncMessageIdPTag, context.SyncMessageId));
			list.Add(new PropValueData(SyncEmailUtils.SyncAccountNamePTag, accountName));
			list.Add(new PropValueData(SyncEmailUtils.SyncFolderIdPTag, folderId));
			foreach (PropTag propTag in SyncEmailUtils.SyncEmailPropTags)
			{
				PropTag propTag2 = propTag;
				if (propTag2 != PropTag.MessageFlags)
				{
					if (propTag2 != (PropTag)276824067U)
					{
						if (propTag2 == PropTag.SpamConfidenceLevel)
						{
							list.Add(new PropValueData(propTag, -1));
						}
					}
					else if (context.ResponseType != null)
					{
						list.Add(new PropValueData(propTag, (int)SyncEmailUtils.GetIconIndex(context.ResponseType)));
					}
				}
				else
				{
					list.Add(new PropValueData(propTag, (int)SyncEmailUtils.GetMessageFlags(context.IsRead, context.IsDraft)));
				}
			}
			if (extraProps != null)
			{
				list.AddRange(extraProps);
			}
			return list.ToArray();
		}

		public static PropTag[] GetIDsFromNames(NamedPropData[] npda, Predicate<NamedPropData> skip = null)
		{
			ArgumentValidator.ThrowIfNull("npda", npda);
			List<PropTag> list = new List<PropTag>();
			foreach (NamedPropData namedPropData in npda)
			{
				if (skip == null || !skip(namedPropData))
				{
					string name;
					if (namedPropData.Guid.Equals(FolderHierarchyUtils.MRSPropsGuid) && (name = namedPropData.Name) != null)
					{
						if (name == "SourceSyncMessageId")
						{
							list.Add(SyncEmailUtils.SyncMessageIdPTag);
							goto IL_A6;
						}
						if (name == "SourceSyncAccountName")
						{
							list.Add(SyncEmailUtils.SyncAccountNamePTag);
							goto IL_A6;
						}
						if (name == "SourceSyncFolderId")
						{
							list.Add(SyncEmailUtils.SyncFolderIdPTag);
							goto IL_A6;
						}
					}
					list.Add(PropTag.Unresolved);
				}
				IL_A6:;
			}
			return list.ToArray();
		}

		public static void CopyMimeStream(ISupportMime mimeSource, MessageRec message, IFolderProxy folderProxy)
		{
			ArgumentValidator.ThrowIfNull("mimeSource", mimeSource);
			ArgumentValidator.ThrowIfNull("message", message);
			ArgumentValidator.ThrowIfNull("folderProxy", folderProxy);
			using (mimeSource.RHTracker.Start())
			{
				PropValueData[] array;
				using (Stream mimeStream = mimeSource.GetMimeStream(message, out array))
				{
					using (IMessageProxy messageProxy = folderProxy.OpenMessage(message.EntryId))
					{
						BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(BufferPoolCollection.BufferSize.Size128K);
						byte[] array2 = bufferPool.Acquire();
						try
						{
							for (;;)
							{
								int num = mimeStream.Read(array2, 0, array2.Length);
								if (num == 0)
								{
									break;
								}
								if (num == array2.Length)
								{
									messageProxy.WriteToMime(array2);
								}
								else
								{
									byte[] array3 = new byte[num];
									Array.Copy(array2, 0, array3, 0, num);
									messageProxy.WriteToMime(array3);
								}
							}
						}
						finally
						{
							bufferPool.Release(array2);
						}
						List<PropValueData> list = new List<PropValueData>(SyncEmailUtils.SyncEmailPropTags.Length);
						list.AddRange(Array.FindAll<PropValueData>(message.AdditionalProps, (PropValueData propValue) => Array.IndexOf<PropTag>(SyncEmailUtils.SyncEmailPropTags, (PropTag)propValue.PropTag) >= 0));
						if (array != null)
						{
							list.AddRange(array);
						}
						using (mimeSource.RHTracker.StartExclusive())
						{
							messageProxy.SetProps(list.ToArray());
							messageProxy.SaveChanges();
						}
					}
				}
			}
		}

		private static IconIndex GetIconIndex(SyncMessageResponseType? responseType)
		{
			if (responseType == null)
			{
				throw new ArgumentNullException("responseType", "ResponseType passed in to GetIconIndex should not be null");
			}
			switch (responseType.Value)
			{
			case SyncMessageResponseType.None:
				return IconIndex.BaseMail;
			case SyncMessageResponseType.Replied:
				return IconIndex.MailReplied;
			case SyncMessageResponseType.Forwarded:
				return IconIndex.MailForwarded;
			default:
				throw new ArgumentException("responseType", string.Format("Unknown message response type: {0}", responseType));
			}
		}

		private static MessageFlags GetMessageFlags(bool? isRead, bool? isDraft)
		{
			MessageFlags messageFlags = MessageFlags.None;
			if (isRead != null && isRead.Value)
			{
				messageFlags |= MessageFlags.Read;
			}
			if (isDraft != null && isDraft.Value)
			{
				messageFlags |= MessageFlags.Unsent;
			}
			return messageFlags;
		}

		private const int SpamConfidenceLevel = -1;

		private const BufferPoolCollection.BufferSize BufferSize128K = BufferPoolCollection.BufferSize.Size128K;

		private static readonly PropTag SyncMessageIdPTag = PropTagHelper.PropTagFromIdAndType(32768, PropType.String);

		private static readonly PropTag SyncAccountNamePTag = PropTagHelper.PropTagFromIdAndType(32769, PropType.String);

		private static readonly PropTag SyncFolderIdPTag = PropTagHelper.PropTagFromIdAndType(32770, PropType.Binary);

		private static readonly PropTag[] SyncEmailPropTags = new PropTag[]
		{
			PropTag.MessageFlags,
			PropTag.SpamConfidenceLevel,
			(PropTag)276824067U
		};
	}
}
