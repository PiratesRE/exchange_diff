using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolReceiver : DisposableWrapper<IFxProxyPool>, IDataImport, IDisposable
	{
		public FxProxyPoolReceiver(IFxProxyPool destination, bool ownsDestination) : base(destination, ownsDestination)
		{
			this.currentFolder = null;
			this.currentMessage = null;
		}

		private IMapiFxProxyEx CurrentEntry
		{
			get
			{
				if (this.currentMessage != null)
				{
					return this.currentMessage;
				}
				if (this.currentFolder != null)
				{
					return this.currentFolder;
				}
				throw new InvalidProxyOperationOrderPermanentException();
			}
		}

		private IMessageProxy CurrentMessage
		{
			get
			{
				return this.currentMessage;
			}
		}

		private IFolderProxy CurrentFolder
		{
			get
			{
				if (this.currentMessage != null)
				{
					this.currentMessage.Dispose();
					this.currentMessage = null;
				}
				return this.currentFolder;
			}
		}

		IDataMessage IDataImport.SendMessageAndWaitForReply(IDataMessage message)
		{
			if (message is FlushMessage)
			{
				base.WrappedObject.Flush();
				return null;
			}
			if (message is FxProxyPoolGetFolderDataRequestMessage)
			{
				return new FxProxyPoolGetFolderDataResponseMessage(base.WrappedObject.GetFolderData());
			}
			if (message is FxProxyPoolGetUploadedIDsRequestMessage)
			{
				return new FxProxyPoolGetUploadedIDsResponseMessage(base.WrappedObject.GetUploadedMessageIDs());
			}
			throw new UnexpectedErrorPermanentException(-2147024809);
		}

		void IDataImport.SendMessage(IDataMessage message)
		{
			FxProxyPoolOpenFolderMessage fxProxyPoolOpenFolderMessage = message as FxProxyPoolOpenFolderMessage;
			if (fxProxyPoolOpenFolderMessage != null)
			{
				this.ClearCurrentObjectsReferences();
				this.currentFolder = base.WrappedObject.GetFolderProxy(fxProxyPoolOpenFolderMessage.Buffer);
				return;
			}
			FxProxyPoolCreateFolderMessage fxProxyPoolCreateFolderMessage = message as FxProxyPoolCreateFolderMessage;
			if (fxProxyPoolCreateFolderMessage != null)
			{
				this.ClearCurrentObjectsReferences();
				this.currentFolder = base.WrappedObject.CreateFolder(fxProxyPoolCreateFolderMessage.Data);
				return;
			}
			FxProxyPoolSetItemPropertiesMessage fxProxyPoolSetItemPropertiesMessage = message as FxProxyPoolSetItemPropertiesMessage;
			if (fxProxyPoolSetItemPropertiesMessage != null)
			{
				if (fxProxyPoolSetItemPropertiesMessage.Props != null)
				{
					if (this.currentMessage != null)
					{
						this.currentMessage.SetItemProperties(fxProxyPoolSetItemPropertiesMessage.Props);
						return;
					}
					if (this.currentFolder != null)
					{
						this.currentFolder.SetItemProperties(fxProxyPoolSetItemPropertiesMessage.Props);
						return;
					}
					base.WrappedObject.SetItemProperties(fxProxyPoolSetItemPropertiesMessage.Props);
				}
				return;
			}
			if (this.currentFolder == null)
			{
				throw new FolderIsMissingTransientException();
			}
			FxProxyPoolOpenItemMessage fxProxyPoolOpenItemMessage = message as FxProxyPoolOpenItemMessage;
			if (fxProxyPoolOpenItemMessage != null)
			{
				this.currentMessage = this.CurrentFolder.OpenMessage(fxProxyPoolOpenItemMessage.Buffer);
				return;
			}
			FxProxyPoolCreateItemMessage fxProxyPoolCreateItemMessage = message as FxProxyPoolCreateItemMessage;
			if (fxProxyPoolCreateItemMessage != null)
			{
				this.currentMessage = this.CurrentFolder.CreateMessage(fxProxyPoolCreateItemMessage.CreateFAI);
				return;
			}
			FxProxyPoolDeleteItemMessage fxProxyPoolDeleteItemMessage = message as FxProxyPoolDeleteItemMessage;
			if (fxProxyPoolDeleteItemMessage != null)
			{
				this.CurrentFolder.DeleteMessage(fxProxyPoolDeleteItemMessage.Buffer);
				return;
			}
			FxProxyPoolCloseEntryMessage fxProxyPoolCloseEntryMessage = message as FxProxyPoolCloseEntryMessage;
			if (fxProxyPoolCloseEntryMessage != null)
			{
				if (this.currentMessage != null)
				{
					this.currentMessage.Dispose();
					this.currentMessage = null;
					return;
				}
				if (this.currentFolder != null)
				{
					this.currentFolder.Dispose();
					this.currentFolder = null;
				}
				return;
			}
			else
			{
				FxProxyPoolSetPropsMessage fxProxyPoolSetPropsMessage = message as FxProxyPoolSetPropsMessage;
				if (fxProxyPoolSetPropsMessage != null)
				{
					this.CurrentEntry.SetProps(fxProxyPoolSetPropsMessage.PropValues);
					return;
				}
				FxProxyPoolSetExtendedAclMessage fxProxyPoolSetExtendedAclMessage = message as FxProxyPoolSetExtendedAclMessage;
				if (fxProxyPoolSetExtendedAclMessage != null)
				{
					this.CurrentFolder.SetItemProperties(new FolderAcl(fxProxyPoolSetExtendedAclMessage.AclFlags, fxProxyPoolSetExtendedAclMessage.AclData));
					return;
				}
				FxProxyPoolSaveChangesMessage fxProxyPoolSaveChangesMessage = message as FxProxyPoolSaveChangesMessage;
				if (fxProxyPoolSaveChangesMessage != null)
				{
					this.CurrentMessage.SaveChanges();
					return;
				}
				FxProxyPoolWriteToMimeMessage fxProxyPoolWriteToMimeMessage = message as FxProxyPoolWriteToMimeMessage;
				if (fxProxyPoolWriteToMimeMessage != null)
				{
					this.CurrentMessage.WriteToMime(fxProxyPoolWriteToMimeMessage.Buffer);
					return;
				}
				FxProxyImportBufferMessage fxProxyImportBufferMessage = message as FxProxyImportBufferMessage;
				if (fxProxyImportBufferMessage != null)
				{
					this.CurrentEntry.ProcessRequest(fxProxyImportBufferMessage.Opcode, fxProxyImportBufferMessage.Buffer);
					return;
				}
				throw new UnexpectedErrorPermanentException(-2147024809);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.ClearCurrentObjectsReferences();
			}
			base.InternalDispose(disposing);
		}

		private void ClearCurrentObjectsReferences()
		{
			if (this.currentMessage != null)
			{
				this.currentMessage.Dispose();
				this.currentMessage = null;
			}
			if (this.currentFolder != null)
			{
				this.currentFolder.Dispose();
				this.currentFolder = null;
			}
		}

		private IFolderProxy currentFolder;

		private IMessageProxy currentMessage;
	}
}
