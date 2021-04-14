using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PST;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PstSourceMailbox : PstMailbox, ISourceMailbox, IMailbox, IDisposable
	{
		byte[] ISourceMailbox.GetMailboxBasicInfo(MailboxSignatureFlags flags)
		{
			throw new NotImplementedException();
		}

		ISourceFolder ISourceMailbox.GetFolder(byte[] entryId)
		{
			MrsTracer.Provider.Function("PstSourceMailbox.GetFolder({0})", new object[]
			{
				TraceUtils.DumpEntryId(entryId)
			});
			return base.GetFolder<PstSourceFolder>(entryId);
		}

		void ISourceMailbox.CopyTo(IFxProxy destMailboxProxy, PropTag[] excludeTags)
		{
			throw new NotImplementedException();
		}

		void ISourceMailbox.SetMailboxSyncState(string syncStateStr)
		{
			throw new NotImplementedException();
		}

		string ISourceMailbox.GetMailboxSyncState()
		{
			throw new NotImplementedException();
		}

		MailboxChangesManifest ISourceMailbox.EnumerateHierarchyChanges(EnumerateHierarchyChangesFlags flags, int maxChanges)
		{
			throw new NotImplementedException();
		}

		void ISourceMailbox.ExportMessages(List<MessageRec> messages, IFxProxyPool proxyPool, ExportMessagesFlags flags, PropTag[] propsToCopyExplicitly, PropTag[] excludeProps)
		{
			MrsTracer.Provider.Function("PstSourceMailbox.ExportMessages({0} messages)", new object[]
			{
				messages.Count
			});
			this.CopyMessagesOneByOne(messages, proxyPool, propsToCopyExplicitly, excludeProps, null);
		}

		void ISourceMailbox.ExportFolders(List<byte[]> folderIds, IFxProxyPool proxyPool, ExportFoldersDataToCopyFlags exportFoldersDataToCopyFlags, GetFolderRecFlags folderRecFlags, PropTag[] additionalFolderRecProps, CopyPropertiesFlags copyPropertiesFlags, PropTag[] excludeProps, AclFlags extendedAclFlags)
		{
			throw new NotImplementedException();
		}

		List<ReplayActionResult> ISourceMailbox.ReplayActions(List<ReplayAction> actions)
		{
			throw new NotImplementedException();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PstSourceMailbox>(this);
		}

		protected override void CopySingleMessage(MessageRec message, IFolderProxy targetFolderProxy, PropTag[] propsToCopyExplicitly, PropTag[] propTagsToExclude)
		{
			ExecutionContext.Create(new DataContext[]
			{
				new OperationDataContext("PstSourceMailbox.CopySingleMessage", OperationType.None),
				new EntryIDsDataContext(message.EntryId)
			}).Execute(delegate
			{
				try
				{
					uint nodeIdFromEntryId = PstMailbox.GetNodeIdFromEntryId(this.IPst.MessageStore.Guid, message.EntryId);
					IMessage message2 = this.IPst.ReadMessage(nodeIdFromEntryId);
					if (message2 == null)
					{
						throw new UnableToReadPSTMessagePermanentException(this.IPst.FileName, nodeIdFromEntryId);
					}
					PSTMessage pstmessage = new PSTMessage(this, message2);
					using (IMessageProxy messageProxy = targetFolderProxy.OpenMessage(message.EntryId))
					{
						FxCollectorSerializer fxCollectorSerializer = new FxCollectorSerializer(messageProxy);
						fxCollectorSerializer.Config(0, 1);
						using (FastTransferDownloadContext fastTransferDownloadContext = FastTransferDownloadContext.CreateForDownload(FastTransferSendOption.Unicode | FastTransferSendOption.UseCpId | FastTransferSendOption.ForceUnicode, 1U, pstmessage.RawPropertyBag.CachedEncoding, NullResourceTracker.Instance, this.GetPropertyFilterFactory(PstMailbox.MoMTPtaFromPta(propTagsToExclude)), false))
						{
							FastTransferMessageCopyTo fastTransferObject = new FastTransferMessageCopyTo(false, pstmessage, true);
							fastTransferDownloadContext.PushInitial(fastTransferObject);
							FxUtils.TransferFxBuffers(fastTransferDownloadContext, fxCollectorSerializer);
							messageProxy.SaveChanges();
						}
					}
				}
				catch (PSTExceptionBase innerException)
				{
					throw new UnableToReadPSTMessagePermanentException(this.IPst.FileName, PstMailbox.GetNodeIdFromEntryId(this.IPst.MessageStore.Guid, message.EntryId), innerException);
				}
			});
		}

		private static HashSet<PropertyTag> GetPropertyTagsToAlwaysExclude()
		{
			if (PstSourceMailbox.propertyTagsToAlwaysExclude == null)
			{
				HashSet<PropertyTag> hashSet = new HashSet<PropertyTag>(PropertyFilterFactory.ExcludePropertiesForFxMessageCommon);
				hashSet.UnionWith(PropertyTag.OneOffEntryIdPropertyTags);
				hashSet.UnionWith(new PropertyTag[]
				{
					PropertyTag.CreationTime,
					PropertyTag.LastModificationTime
				});
				hashSet.Remove(PropertyTag.SenderEntryId);
				hashSet.Remove(PropertyTag.ReceivedRepresentingEntryId);
				hashSet.Remove(PropertyTag.ReceivedByEntryId);
				PstSourceMailbox.propertyTagsToAlwaysExclude = hashSet.ToArray<PropertyTag>();
			}
			return new HashSet<PropertyTag>(PstSourceMailbox.propertyTagsToAlwaysExclude);
		}

		private PropertyFilterFactory GetPropertyFilterFactory(PropertyTag[] additionalPropertyTags)
		{
			if (this.propertyTagsToExclude != null && (additionalPropertyTags == null || this.propertyTagsToExclude.IsSupersetOf(additionalPropertyTags)))
			{
				return this.propertyFilterFactory;
			}
			this.propertyTagsToExclude = PstSourceMailbox.GetPropertyTagsToAlwaysExclude();
			if (additionalPropertyTags != null)
			{
				this.propertyTagsToExclude.UnionWith(additionalPropertyTags);
			}
			this.propertyFilterFactory = new PropertyFilterFactory(false, false, this.propertyTagsToExclude.ToArray<PropertyTag>());
			return this.propertyFilterFactory;
		}

		private static PropertyTag[] propertyTagsToAlwaysExclude;

		private HashSet<PropertyTag> propertyTagsToExclude;

		private PropertyFilterFactory propertyFilterFactory;
	}
}
