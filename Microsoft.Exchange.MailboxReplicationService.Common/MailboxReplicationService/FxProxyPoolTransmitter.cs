using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolTransmitter : DisposableWrapper<IDataImport>, IFxProxyPool, IDisposable
	{
		public FxProxyPoolTransmitter(IDataImport destination, bool ownsDestination, VersionInformation destinationCapabilities) : base(destination, ownsDestination)
		{
			this.currentEntries = new Stack<FxProxyPoolTransmitter.EntryWrapper>();
			this.pendingOperations = new Queue<IDataMessage>();
			this.destinationCapabilities = destinationCapabilities;
			this.folderDataMap = null;
		}

		private FxProxyPoolTransmitter.EntryWrapper CurrentEntry
		{
			get
			{
				if (this.currentEntries.Count <= 0)
				{
					return null;
				}
				return this.currentEntries.Peek();
			}
		}

		IFolderProxy IFxProxyPool.CreateFolder(FolderRec folder)
		{
			this.EnsureFolderDataCached();
			this.FlushBufferedOperations();
			if (this.folderDataMap == null)
			{
				this.folderDataMap = new EntryIdMap<byte[]>();
			}
			if (!this.folderDataMap.ContainsKey(folder.EntryId))
			{
				this.folderDataMap.Add(folder.EntryId, MapiUtils.MapiFolderObjectData);
			}
			return new FxProxyPoolTransmitter.FolderWrapper(folder, this);
		}

		IFolderProxy IFxProxyPool.GetFolderProxy(byte[] folderId)
		{
			this.EnsureFolderDataCached();
			this.FlushBufferedOperations();
			if (this.folderDataMap.ContainsKey(folderId))
			{
				return new FxProxyPoolTransmitter.FolderWrapper(folderId, this);
			}
			return null;
		}

		EntryIdMap<byte[]> IFxProxyPool.GetFolderData()
		{
			this.EnsureFolderDataCached();
			this.FlushBufferedOperations();
			return this.folderDataMap;
		}

		void IFxProxyPool.Flush()
		{
			this.EnsureFolderDataCached();
			this.FlushBufferedOperations();
			base.WrappedObject.SendMessageAndWaitForReply(FlushMessage.Instance);
		}

		List<byte[]> IFxProxyPool.GetUploadedMessageIDs()
		{
			this.EnsureFolderDataCached();
			this.FlushBufferedOperations();
			IDataMessage dataMessage = base.WrappedObject.SendMessageAndWaitForReply(FxProxyPoolGetUploadedIDsRequestMessage.Instance);
			FxProxyPoolGetUploadedIDsResponseMessage fxProxyPoolGetUploadedIDsResponseMessage = dataMessage as FxProxyPoolGetUploadedIDsResponseMessage;
			if (fxProxyPoolGetUploadedIDsResponseMessage == null)
			{
				throw new UnexpectedErrorPermanentException(-2147024809);
			}
			return fxProxyPoolGetUploadedIDsResponseMessage.EntryIDs;
		}

		void IFxProxyPool.SetItemProperties(ItemPropertiesBase props)
		{
			this.EnsureFolderDataCached();
			this.FlushBufferedOperations();
			this.BufferOrSendMessage(new FxProxyPoolSetItemPropertiesMessage(props));
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				while (this.currentEntries.Count > 0)
				{
					this.currentEntries.Pop().Dispose();
				}
			}
			base.InternalDispose(disposing);
		}

		private void EnsureFolderDataCached()
		{
			if (this.folderDataMap != null)
			{
				return;
			}
			IDataMessage dataMessage = base.WrappedObject.SendMessageAndWaitForReply(FxProxyPoolGetFolderDataRequestMessage.Instance);
			FxProxyPoolGetFolderDataResponseMessage fxProxyPoolGetFolderDataResponseMessage = dataMessage as FxProxyPoolGetFolderDataResponseMessage;
			if (fxProxyPoolGetFolderDataResponseMessage == null)
			{
				throw new UnexpectedErrorPermanentException(-2147024809);
			}
			this.folderDataMap = fxProxyPoolGetFolderDataResponseMessage.FolderData;
		}

		private void FlushBufferedOperations()
		{
			while (this.pendingOperations.Count > 0)
			{
				IDataMessage message = this.pendingOperations.Dequeue();
				base.WrappedObject.SendMessage(message);
			}
		}

		private void BufferOrSendMessage(IDataMessage msg)
		{
			if (msg is FxProxyPoolCloseEntryMessage || msg is FxProxyPoolOpenFolderMessage || msg is FxProxyPoolCreateItemMessage || msg is FxProxyPoolOpenItemMessage)
			{
				this.pendingOperations.Enqueue(msg);
				return;
			}
			this.FlushBufferedOperations();
			base.WrappedObject.SendMessage(msg);
		}

		private EntryIdMap<byte[]> folderDataMap;

		private Stack<FxProxyPoolTransmitter.EntryWrapper> currentEntries;

		private Queue<IDataMessage> pendingOperations;

		private VersionInformation destinationCapabilities;

		private abstract class EntryWrapper : DisposeTrackableBase, IMapiFxProxyEx, IMapiFxProxy, IDisposeTrackable, IDisposable
		{
			public EntryWrapper(FxProxyPoolTransmitter pool)
			{
				this.pool = pool;
				this.Pool.currentEntries.Push(this);
			}

			protected abstract byte[] ObjectData { get; }

			protected FxProxyPoolTransmitter Pool
			{
				get
				{
					return this.pool;
				}
			}

			byte[] IMapiFxProxy.GetObjectData()
			{
				return this.ObjectData;
			}

			void IMapiFxProxy.ProcessRequest(FxOpcodes opCode, byte[] request)
			{
				this.Pool.BufferOrSendMessage(new FxProxyImportBufferMessage(opCode, request));
			}

			void IMapiFxProxyEx.SetProps(PropValueData[] pvda)
			{
				this.Pool.BufferOrSendMessage(new FxProxyPoolSetPropsMessage(pvda));
			}

			void IMapiFxProxyEx.SetItemProperties(ItemPropertiesBase props)
			{
				if (this.Pool.destinationCapabilities[56])
				{
					this.Pool.BufferOrSendMessage(new FxProxyPoolSetItemPropertiesMessage(props));
					return;
				}
				FolderAcl folderAcl = props as FolderAcl;
				if (folderAcl != null)
				{
					this.Pool.BufferOrSendMessage(new FxProxyPoolSetExtendedAclMessage(folderAcl.Flags, folderAcl.Value));
					return;
				}
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(this.Pool.destinationCapabilities.ComputerName, this.Pool.destinationCapabilities.ToString(), "IFxProxyPool.SetItemProperties");
			}

			protected override void InternalDispose(bool disposing)
			{
				this.Pool.BufferOrSendMessage(FxProxyPoolCloseEntryMessage.Instance);
				this.Pool.currentEntries.Pop();
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<FxProxyPoolTransmitter.EntryWrapper>(this);
			}

			private FxProxyPoolTransmitter pool;
		}

		private class FolderWrapper : FxProxyPoolTransmitter.EntryWrapper, IFolderProxy, IMapiFxProxyEx, IMapiFxProxy, IDisposeTrackable, IDisposable
		{
			public FolderWrapper(FolderRec folderRec, FxProxyPoolTransmitter pool) : base(pool)
			{
				this.entryId = folderRec.EntryId;
				base.Pool.BufferOrSendMessage(new FxProxyPoolCreateFolderMessage(folderRec));
			}

			public FolderWrapper(byte[] entryId, FxProxyPoolTransmitter pool) : base(pool)
			{
				this.entryId = entryId;
				base.Pool.BufferOrSendMessage(new FxProxyPoolOpenFolderMessage(entryId));
			}

			protected override byte[] ObjectData
			{
				get
				{
					return base.Pool.folderDataMap[this.entryId];
				}
			}

			IMessageProxy IFolderProxy.OpenMessage(byte[] entryId)
			{
				return new FxProxyPoolTransmitter.MessageWrapper(new FxProxyPoolOpenItemMessage(entryId), base.Pool);
			}

			IMessageProxy IFolderProxy.CreateMessage(bool isAssociated)
			{
				if (isAssociated)
				{
					return new FxProxyPoolTransmitter.MessageWrapper(FxProxyPoolCreateItemMessage.FAI, base.Pool);
				}
				return new FxProxyPoolTransmitter.MessageWrapper(FxProxyPoolCreateItemMessage.Regular, base.Pool);
			}

			void IFolderProxy.DeleteMessage(byte[] entryId)
			{
				base.Pool.BufferOrSendMessage(new FxProxyPoolDeleteItemMessage(entryId));
			}

			private byte[] entryId;
		}

		private class MessageWrapper : FxProxyPoolTransmitter.EntryWrapper, IMessageProxy, IMapiFxProxyEx, IMapiFxProxy, IDisposeTrackable, IDisposable
		{
			public MessageWrapper(IDataMessage openMessage, FxProxyPoolTransmitter pool) : base(pool)
			{
				base.Pool.BufferOrSendMessage(openMessage);
			}

			protected override byte[] ObjectData
			{
				get
				{
					return base.Pool.folderDataMap[CommonUtils.MessageData];
				}
			}

			void IMessageProxy.SaveChanges()
			{
				base.Pool.BufferOrSendMessage(FxProxyPoolSaveChangesMessage.Instance);
			}

			void IMessageProxy.WriteToMime(byte[] buffer)
			{
				base.Pool.BufferOrSendMessage(new FxProxyPoolWriteToMimeMessage(buffer));
			}
		}
	}
}
