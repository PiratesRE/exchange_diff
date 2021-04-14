using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class FxProxyPool<TFolderEntry, TMessageEntry> : DisposeTrackableBase, IFxProxyPool, IDisposable where TFolderEntry : class, IDisposable where TMessageEntry : class, IDisposable
	{
		public FxProxyPool(ICollection<byte[]> folderIds)
		{
			this.folderIds = folderIds;
			this.uploadedMessageIDs = null;
		}

		IFolderProxy IFxProxyPool.CreateFolder(FolderRec folderRec)
		{
			TFolderEntry tfolderEntry = this.OpenFolder(folderRec.EntryId);
			if (tfolderEntry == null)
			{
				tfolderEntry = this.CreateFolder(folderRec);
			}
			return new FxProxyPool<TFolderEntry, TMessageEntry>.FolderProxy(tfolderEntry, this);
		}

		IFolderProxy IFxProxyPool.GetFolderProxy(byte[] folderId)
		{
			TFolderEntry tfolderEntry = this.OpenFolder(folderId);
			if (tfolderEntry != null)
			{
				return new FxProxyPool<TFolderEntry, TMessageEntry>.FolderProxy(tfolderEntry, this);
			}
			return null;
		}

		EntryIdMap<byte[]> IFxProxyPool.GetFolderData()
		{
			EntryIdMap<byte[]> entryIdMap = new EntryIdMap<byte[]>();
			if (this.folderIds != null)
			{
				foreach (byte[] array in this.folderIds)
				{
					using (TFolderEntry tfolderEntry = this.OpenFolder(array))
					{
						entryIdMap[array] = ((tfolderEntry != null) ? this.FolderGetObjectData(tfolderEntry) : null);
					}
				}
			}
			byte[] array2 = this.MessageGetObjectData(default(TMessageEntry));
			if (array2 != null)
			{
				entryIdMap[CommonUtils.MessageData] = array2;
			}
			return entryIdMap;
		}

		List<byte[]> IFxProxyPool.GetUploadedMessageIDs()
		{
			return this.uploadedMessageIDs;
		}

		void IFxProxyPool.Flush()
		{
		}

		void IFxProxyPool.SetItemProperties(ItemPropertiesBase props)
		{
			this.MailboxSetItemProperties(props);
		}

		protected abstract TFolderEntry CreateFolder(FolderRec folderRec);

		protected abstract TFolderEntry OpenFolder(byte[] folderID);

		protected abstract void MailboxSetItemProperties(ItemPropertiesBase props);

		protected abstract byte[] FolderGetObjectData(TFolderEntry folder);

		protected abstract void FolderProcessRequest(TFolderEntry folder, FxOpcodes opcode, byte[] request);

		protected abstract void FolderSetProps(TFolderEntry folder, PropValueData[] pvda);

		protected abstract void FolderSetItemProperties(TFolderEntry folder, ItemPropertiesBase props);

		protected abstract TMessageEntry FolderOpenMessage(TFolderEntry folder, byte[] entryID);

		protected abstract TMessageEntry FolderCreateMessage(TFolderEntry folder, bool isAssociated);

		protected abstract void FolderDeleteMessage(TFolderEntry folder, byte[] entryID);

		protected abstract byte[] MessageGetObjectData(TMessageEntry message);

		protected abstract void MessageProcessRequest(TMessageEntry message, FxOpcodes opcode, byte[] request);

		protected abstract void MessageSetProps(TMessageEntry message, PropValueData[] pvda);

		protected abstract void MessageSetItemProperties(TMessageEntry message, ItemPropertiesBase props);

		protected abstract byte[] MessageSaveChanges(TMessageEntry message);

		protected abstract void MessageWriteToMime(TMessageEntry message, byte[] buffer);

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FxProxyPool<TFolderEntry, TMessageEntry>>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		private ICollection<byte[]> folderIds;

		private List<byte[]> uploadedMessageIDs;

		private class FolderProxy : DisposableWrapper<TFolderEntry>, IFolderProxy, IMapiFxProxyEx, IMapiFxProxy, IDisposeTrackable, IDisposable
		{
			public FolderProxy(TFolderEntry folder, FxProxyPool<TFolderEntry, TMessageEntry> owner) : base(folder, true)
			{
				this.owner = owner;
			}

			byte[] IMapiFxProxy.GetObjectData()
			{
				return this.owner.FolderGetObjectData(base.WrappedObject);
			}

			void IMapiFxProxy.ProcessRequest(FxOpcodes opCode, byte[] request)
			{
				this.owner.FolderProcessRequest(base.WrappedObject, opCode, request);
			}

			void IMapiFxProxyEx.SetProps(PropValueData[] pvda)
			{
				this.owner.FolderSetProps(base.WrappedObject, pvda);
			}

			void IMapiFxProxyEx.SetItemProperties(ItemPropertiesBase props)
			{
				this.owner.FolderSetItemProperties(base.WrappedObject, props);
			}

			IMessageProxy IFolderProxy.OpenMessage(byte[] entryId)
			{
				TMessageEntry message = this.owner.FolderOpenMessage(base.WrappedObject, entryId);
				return new FxProxyPool<TFolderEntry, TMessageEntry>.MessageProxy(message, this.owner);
			}

			IMessageProxy IFolderProxy.CreateMessage(bool isAssociated)
			{
				TMessageEntry message = this.owner.FolderCreateMessage(base.WrappedObject, isAssociated);
				return new FxProxyPool<TFolderEntry, TMessageEntry>.MessageProxy(message, this.owner);
			}

			void IFolderProxy.DeleteMessage(byte[] entryId)
			{
				this.owner.FolderDeleteMessage(base.WrappedObject, entryId);
			}

			private FxProxyPool<TFolderEntry, TMessageEntry> owner;
		}

		private class MessageProxy : DisposableWrapper<TMessageEntry>, IMessageProxy, IMapiFxProxyEx, IMapiFxProxy, IDisposeTrackable, IDisposable
		{
			public MessageProxy(TMessageEntry message, FxProxyPool<TFolderEntry, TMessageEntry> owner) : base(message, true)
			{
				this.owner = owner;
			}

			byte[] IMapiFxProxy.GetObjectData()
			{
				return this.owner.MessageGetObjectData(base.WrappedObject);
			}

			void IMapiFxProxy.ProcessRequest(FxOpcodes opCode, byte[] request)
			{
				this.owner.MessageProcessRequest(base.WrappedObject, opCode, request);
			}

			void IMapiFxProxyEx.SetProps(PropValueData[] pvda)
			{
				this.owner.MessageSetProps(base.WrappedObject, pvda);
			}

			void IMapiFxProxyEx.SetItemProperties(ItemPropertiesBase props)
			{
				this.owner.MessageSetItemProperties(base.WrappedObject, props);
			}

			void IMessageProxy.SaveChanges()
			{
				byte[] item = this.owner.MessageSaveChanges(base.WrappedObject);
				if (this.owner.uploadedMessageIDs == null)
				{
					this.owner.uploadedMessageIDs = new List<byte[]>();
				}
				this.owner.uploadedMessageIDs.Add(item);
			}

			void IMessageProxy.WriteToMime(byte[] buffer)
			{
				this.owner.MessageWriteToMime(base.WrappedObject, buffer);
			}

			private FxProxyPool<TFolderEntry, TMessageEntry> owner;
		}
	}
}
