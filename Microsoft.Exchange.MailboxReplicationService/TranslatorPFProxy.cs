using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class TranslatorPFProxy : DisposableWrapper<IFxProxyPool>, IFxProxyPool, IDisposable
	{
		public TranslatorPFProxy(ISourceMailbox sourceMailbox, IDestinationMailbox destinationMailbox, IFxProxyPool destinationProxyPool) : base(destinationProxyPool, true)
		{
			this.sourceMailbox = sourceMailbox;
			this.destinationMailbox = destinationMailbox;
		}

		EntryIdMap<byte[]> IFxProxyPool.GetFolderData()
		{
			EntryIdMap<byte[]> entryIdMap = new EntryIdMap<byte[]>();
			foreach (KeyValuePair<byte[], byte[]> keyValuePair in base.WrappedObject.GetFolderData())
			{
				if (keyValuePair.Key.Length == 46)
				{
					entryIdMap.Add(this.sourceMailbox.GetSessionSpecificEntryId(keyValuePair.Key), keyValuePair.Value);
				}
			}
			return entryIdMap;
		}

		void IFxProxyPool.Flush()
		{
			base.WrappedObject.Flush();
		}

		void IFxProxyPool.SetItemProperties(ItemPropertiesBase props)
		{
			base.WrappedObject.SetItemProperties(props);
		}

		IFolderProxy IFxProxyPool.CreateFolder(FolderRec folder)
		{
			return base.WrappedObject.CreateFolder(folder);
		}

		IFolderProxy IFxProxyPool.GetFolderProxy(byte[] sourceFolderId)
		{
			return base.WrappedObject.GetFolderProxy(this.destinationMailbox.GetSessionSpecificEntryId(sourceFolderId));
		}

		List<byte[]> IFxProxyPool.GetUploadedMessageIDs()
		{
			return base.WrappedObject.GetUploadedMessageIDs();
		}

		private const int SizeOfFolderId = 46;

		private ISourceMailbox sourceMailbox;

		private IDestinationMailbox destinationMailbox;
	}
}
