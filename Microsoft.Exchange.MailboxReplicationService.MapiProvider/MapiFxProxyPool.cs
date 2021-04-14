using System;
using System.Collections.Generic;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MapiFxProxyPool : FxProxyPool<MapiFxProxyPool.FolderEntry, MapiFxProxyPool.MessageEntry>
	{
		public MapiFxProxyPool(MapiDestinationMailbox destMailbox, ICollection<byte[]> folderIds) : base(folderIds)
		{
			this.destMailbox = destMailbox;
		}

		protected override MapiFxProxyPool.FolderEntry CreateFolder(FolderRec folderRec)
		{
			throw new NotImplementedException();
		}

		protected override MapiFxProxyPool.FolderEntry OpenFolder(byte[] folderID)
		{
			MapiFolder folder = (MapiFolder)this.destMailbox.OpenMapiEntry(folderID, folderID, OpenEntryFlags.Modify | OpenEntryFlags.DontThrowIfEntryIsMissing);
			return MapiFxProxyPool.FolderEntry.Wrap(folder);
		}

		protected override void MailboxSetItemProperties(ItemPropertiesBase props)
		{
			throw new NotImplementedException(string.Format("MapiFxProxyPool.SetItemProperties({0})", props.GetType().Name));
		}

		protected override byte[] FolderGetObjectData(MapiFxProxyPool.FolderEntry folder)
		{
			return folder.Proxy.GetObjectData();
		}

		protected override void FolderProcessRequest(MapiFxProxyPool.FolderEntry entry, FxOpcodes opcode, byte[] request)
		{
			entry.Proxy.ProcessRequest(opcode, request);
		}

		protected override void FolderSetProps(MapiFxProxyPool.FolderEntry folder, PropValueData[] pvda)
		{
			PropValue[] native = DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(pvda);
			folder.WrappedObject.SetProps(native);
		}

		protected override void FolderSetItemProperties(MapiFxProxyPool.FolderEntry folder, ItemPropertiesBase props)
		{
			throw new NotImplementedException(string.Format("MapiFxProxyPool.FolderSetItemProperties({0})", props.GetType().Name));
		}

		protected override MapiFxProxyPool.MessageEntry FolderOpenMessage(MapiFxProxyPool.FolderEntry folder, byte[] entryID)
		{
			MapiMessage message = (MapiMessage)folder.WrappedObject.OpenEntry(entryID, OpenEntryFlags.Modify);
			return MapiFxProxyPool.MessageEntry.Wrap(message);
		}

		protected override MapiFxProxyPool.MessageEntry FolderCreateMessage(MapiFxProxyPool.FolderEntry folder, bool isAssociated)
		{
			CreateMessageFlags createMessageFlags = CreateMessageFlags.None;
			if (isAssociated)
			{
				createMessageFlags |= CreateMessageFlags.Associated;
			}
			MapiMessage message = folder.WrappedObject.CreateMessage(createMessageFlags);
			return MapiFxProxyPool.MessageEntry.Wrap(message);
		}

		protected override void FolderDeleteMessage(MapiFxProxyPool.FolderEntry folder, byte[] entryID)
		{
			folder.WrappedObject.DeleteMessages(DeleteMessagesFlags.ForceHardDelete, new byte[][]
			{
				entryID
			});
		}

		protected override byte[] MessageGetObjectData(MapiFxProxyPool.MessageEntry message)
		{
			if (message != null)
			{
				return message.Proxy.GetObjectData();
			}
			if (this.destMailbox.InTransitStatus != InTransitStatus.NotInTransit)
			{
				return null;
			}
			byte[] inboxFolderEntryId = this.destMailbox.MapiStore.GetInboxFolderEntryId();
			byte[] result;
			using (MapiFxProxyPool.FolderEntry folderEntry = this.OpenFolder(inboxFolderEntryId))
			{
				using (MapiFxProxyPool.MessageEntry messageEntry = this.FolderCreateMessage(folderEntry, false))
				{
					result = this.MessageGetObjectData(messageEntry);
				}
			}
			return result;
		}

		protected override void MessageProcessRequest(MapiFxProxyPool.MessageEntry message, FxOpcodes opcode, byte[] request)
		{
			message.Proxy.ProcessRequest(opcode, request);
		}

		protected override void MessageSetProps(MapiFxProxyPool.MessageEntry entry, PropValueData[] pvda)
		{
			PropValue[] native = DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(pvda);
			entry.WrappedObject.SetProps(native);
		}

		protected override void MessageSetItemProperties(MapiFxProxyPool.MessageEntry message, ItemPropertiesBase props)
		{
			throw new NotImplementedException(string.Format("MapiFxProxyPool.MessageSetItemProperties({0})", props.GetType().Name));
		}

		protected override byte[] MessageSaveChanges(MapiFxProxyPool.MessageEntry entry)
		{
			entry.WrappedObject.SaveChanges();
			return entry.WrappedObject.GetProp(PropTag.EntryId).GetBytes();
		}

		protected override void MessageWriteToMime(MapiFxProxyPool.MessageEntry entry, byte[] buffer)
		{
			throw new NotImplementedException();
		}

		private MapiDestinationMailbox destMailbox;

		internal abstract class MapiEntry<T> : DisposableWrapper<T> where T : MapiProp
		{
			protected MapiEntry(T entry) : base(entry, true)
			{
				T wrappedObject = base.WrappedObject;
				this.proxy = wrappedObject.GetFxProxyCollector();
			}

			public IMapiFxProxy Proxy
			{
				get
				{
					return this.proxy;
				}
			}

			protected override void InternalDispose(bool disposing)
			{
				if (disposing && this.proxy != null)
				{
					this.proxy.Dispose();
					this.proxy = null;
				}
				base.InternalDispose(disposing);
			}

			private IMapiFxProxy proxy;
		}

		internal class FolderEntry : MapiFxProxyPool.MapiEntry<MapiFolder>
		{
			protected FolderEntry(MapiFolder folder) : base(folder)
			{
			}

			public static MapiFxProxyPool.FolderEntry Wrap(MapiFolder folder)
			{
				if (folder != null)
				{
					return new MapiFxProxyPool.FolderEntry(folder);
				}
				return null;
			}
		}

		internal class MessageEntry : MapiFxProxyPool.MapiEntry<MapiMessage>
		{
			protected MessageEntry(MapiMessage message) : base(message)
			{
			}

			public static MapiFxProxyPool.MessageEntry Wrap(MapiMessage message)
			{
				if (message != null)
				{
					return new MapiFxProxyPool.MessageEntry(message);
				}
				return null;
			}
		}
	}
}
