using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PST;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PSTFxProxyPool : FxProxyPool<PSTFxProxyPool.FolderEntry, PSTFxProxyPool.MessageEntry>
	{
		public PSTFxProxyPool(PstDestinationMailbox destPst, ICollection<byte[]> folderIds) : base(folderIds)
		{
			this.destPst = destPst;
		}

		protected override PSTFxProxyPool.FolderEntry CreateFolder(FolderRec folderRec)
		{
			throw new NotImplementedException();
		}

		protected override PSTFxProxyPool.FolderEntry OpenFolder(byte[] folderID)
		{
			uint nodeIdFromEntryId = PstMailbox.GetNodeIdFromEntryId(this.destPst.IPst.MessageStore.Guid, folderID);
			IFolder iPstFolder = this.destPst.IPst.ReadFolder(nodeIdFromEntryId);
			return PSTFxProxyPool.FolderEntry.Wrap(this.destPst, iPstFolder);
		}

		protected override void MailboxSetItemProperties(ItemPropertiesBase props)
		{
			throw new NotImplementedException(string.Format("PSTFxProxyPool.SetItemProperties({0})", props.GetType().Name));
		}

		protected override byte[] FolderGetObjectData(PSTFxProxyPool.FolderEntry folder)
		{
			return folder.Proxy.GetObjectData();
		}

		protected override void FolderProcessRequest(PSTFxProxyPool.FolderEntry entry, FxOpcodes opcode, byte[] request)
		{
			throw new NotImplementedException();
		}

		protected override void FolderSetProps(PSTFxProxyPool.FolderEntry folder, PropValueData[] pvda)
		{
			throw new NotImplementedException();
		}

		protected override void FolderSetItemProperties(PSTFxProxyPool.FolderEntry folder, ItemPropertiesBase props)
		{
			throw new NotImplementedException(string.Format("PSTFxProxyPool.FolderSetItemProperties({0})", props.GetType().Name));
		}

		protected override PSTFxProxyPool.MessageEntry FolderOpenMessage(PSTFxProxyPool.FolderEntry folder, byte[] entryID)
		{
			throw new NotImplementedException();
		}

		protected override PSTFxProxyPool.MessageEntry FolderCreateMessage(PSTFxProxyPool.FolderEntry folder, bool isAssociated)
		{
			return folder.CreateMessage(isAssociated);
		}

		protected override void FolderDeleteMessage(PSTFxProxyPool.FolderEntry folder, byte[] entryID)
		{
			folder.DeleteMessage(entryID);
		}

		protected override byte[] MessageGetObjectData(PSTFxProxyPool.MessageEntry message)
		{
			if (message == null)
			{
				return MapiUtils.CreateObjectData(InterfaceIds.IMessageGuid);
			}
			return message.Proxy.GetObjectData();
		}

		protected override void MessageProcessRequest(PSTFxProxyPool.MessageEntry message, FxOpcodes opcode, byte[] request)
		{
			message.ProcessRequest(opcode, request);
		}

		protected override void MessageSetProps(PSTFxProxyPool.MessageEntry message, PropValueData[] pvda)
		{
			message.SetProps(pvda);
		}

		protected override void MessageSetItemProperties(PSTFxProxyPool.MessageEntry message, ItemPropertiesBase props)
		{
			throw new NotImplementedException(string.Format("PSTFxProxyPool.MessageSetItemProperties({0})", props.GetType().Name));
		}

		protected override byte[] MessageSaveChanges(PSTFxProxyPool.MessageEntry message)
		{
			return message.SaveChanges();
		}

		protected override void MessageWriteToMime(PSTFxProxyPool.MessageEntry entry, byte[] buffer)
		{
			throw new NotImplementedException();
		}

		private PstDestinationMailbox destPst;

		internal abstract class PSTEntry : DisposeTrackableBase
		{
			protected PSTEntry(object entry)
			{
				this.proxy = new PSTFxProxy(entry);
			}

			public IMapiFxProxy Proxy
			{
				get
				{
					return this.proxy;
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<PSTFxProxyPool.PSTEntry>(this);
			}

			protected override void InternalDispose(bool disposing)
			{
				if (disposing && this.proxy != null)
				{
					this.proxy.Dispose();
					this.proxy = null;
				}
			}

			private PSTFxProxy proxy;
		}

		internal class FolderEntry : PSTFxProxyPool.PSTEntry
		{
			private FolderEntry(PstFxFolder folder) : base(folder)
			{
				this.folder = folder;
			}

			public static PSTFxProxyPool.FolderEntry Wrap(PstMailbox pstMailbox, IFolder iPstFolder)
			{
				if (iPstFolder == null)
				{
					return null;
				}
				return new PSTFxProxyPool.FolderEntry(new PstFxFolder(pstMailbox, iPstFolder));
			}

			public PSTFxProxyPool.MessageEntry CreateMessage(bool isAssociated)
			{
				IMessage iPstMessage;
				try
				{
					iPstMessage = (isAssociated ? this.folder.IPstFolder.AddAssociatedMessage() : this.folder.IPstFolder.AddMessage());
				}
				catch (PSTExceptionBase innerException)
				{
					throw new UnableToCreatePSTMessagePermanentException(this.folder.PstMailbox.IPst.FileName, innerException);
				}
				return PSTFxProxyPool.MessageEntry.Wrap(this.folder.PstMailbox, iPstMessage);
			}

			public void DeleteMessage(byte[] entryID)
			{
				uint nodeIdFromEntryId = PstMailbox.GetNodeIdFromEntryId(this.folder.PstMailbox.IPst.MessageStore.Guid, entryID);
				try
				{
					IMessage message = this.folder.PstMailbox.IPst.ReadMessage(nodeIdFromEntryId);
					if (message != null)
					{
						this.folder.PstMailbox.IPst.DeleteMessage(nodeIdFromEntryId);
					}
				}
				catch (InternalItemErrorException)
				{
				}
				catch (PSTExceptionBase innerException)
				{
					throw new UnableToReadPSTMessagePermanentException(this.folder.PstMailbox.IPst.FileName, nodeIdFromEntryId, innerException);
				}
			}

			private PstFxFolder folder;
		}

		internal class MessageEntry : PSTFxProxyPool.PSTEntry
		{
			private MessageEntry(PSTMessage message) : base(message)
			{
				this.message = message;
				this.uploadContext = null;
				this.messageProcessor = null;
				this.message.PropertyBag.SetProperty(new PropertyValue(PSTFxProxyPool.MessageEntry.MsgStatusPropertyTag, 0));
			}

			public static PSTFxProxyPool.MessageEntry Wrap(PstMailbox pstMailbox, IMessage iPstMessage)
			{
				if (iPstMessage == null)
				{
					return null;
				}
				return new PSTFxProxyPool.MessageEntry(new PSTMessage(pstMailbox, iPstMessage));
			}

			public byte[] SaveChanges()
			{
				this.uploadContext.Flush();
				try
				{
					this.message.Save();
				}
				catch (PSTExceptionBase innerException)
				{
					throw new UnableToCreatePSTMessagePermanentException(this.message.PstMailbox.IPst.FileName, innerException);
				}
				return PstMailbox.CreateEntryIdFromNodeId(this.message.PstMailbox.IPst.MessageStore.Guid, this.message.IPstMessage.Id);
			}

			public void ProcessRequest(FxOpcodes opCode, byte[] request)
			{
				try
				{
					switch (opCode)
					{
					case FxOpcodes.Config:
						this.messageProcessor = new FastTransferMessageCopyTo(false, this.message, true);
						this.uploadContext = new FastTransferUploadContext(Encoding.ASCII, NullResourceTracker.Instance, PropertyFilterFactory.IncludeAllFactory, false);
						this.uploadContext.PushInitial(this.messageProcessor);
						break;
					case FxOpcodes.TransferBuffer:
						this.uploadContext.PutNextBuffer(new ArraySegment<byte>(request));
						break;
					case FxOpcodes.IsInterfaceOk:
					case FxOpcodes.TellPartnerVersion:
						break;
					default:
						throw new NotSupportedException();
					}
				}
				catch (PSTExceptionBase innerException)
				{
					throw new UnableToCreatePSTMessagePermanentException(this.message.PstMailbox.IPst.FileName, innerException);
				}
			}

			public void SetProps(PropValueData[] pvda)
			{
				try
				{
					foreach (PropValueData data in pvda)
					{
						this.message.PropertyBag.SetProperty(PstMailbox.MoMTPvFromPv(DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(data)));
					}
				}
				catch (PSTExceptionBase innerException)
				{
					throw new UnableToCreatePSTMessagePermanentException(this.message.PstMailbox.IPst.FileName, innerException);
				}
			}

			protected override void InternalDispose(bool disposing)
			{
				base.InternalDispose(disposing);
				if (disposing && this.uploadContext != null)
				{
					this.messageProcessor.Dispose();
					this.uploadContext.Dispose();
					this.messageProcessor = null;
					this.uploadContext = null;
				}
			}

			private const int MsgStatusPropertyValue = 0;

			private static readonly PropertyTag MsgStatusPropertyTag = new PropertyTag(236388355U);

			private PSTMessage message;

			private FastTransferUploadContext uploadContext;

			private IFastTransferProcessor<FastTransferUploadContext> messageProcessor;
		}
	}
}
