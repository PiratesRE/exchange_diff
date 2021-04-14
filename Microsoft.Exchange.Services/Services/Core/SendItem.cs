using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SendItem : MultiStepServiceCommand<SendItemRequest, ServiceResultNone>
	{
		public SendItem(CallContext callContext, SendItemRequest request) : base(callContext, request)
		{
			this.itemIds = base.Request.Ids;
			this.targetFolderToSaveIn = base.Request.SavedItemFolderId;
			this.saveInFolder = base.Request.SaveItemToFolder;
			ServiceCommandBase.ThrowIfNullOrEmpty<BaseItemId>(this.itemIds, "itemIds", "SendItem::ctor");
		}

		public event Action<MessageItem> BeforeMessageDisposition
		{
			add
			{
				this.beforeMessageDispositionEventHandler = (Action<MessageItem>)Delegate.Combine(this.beforeMessageDispositionEventHandler, value);
			}
			remove
			{
				this.beforeMessageDispositionEventHandler = (Action<MessageItem>)Delegate.Remove(this.beforeMessageDispositionEventHandler, value);
			}
		}

		internal override void PreExecuteCommand()
		{
			this.saveFolderIdAndSession = this.GetSaveToFolderIdFromRequest();
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			SendItemResponse sendItemResponse = new SendItemResponse();
			sendItemResponse.BuildForNoReturnValue(base.Results);
			return sendItemResponse;
		}

		internal override int StepCount
		{
			get
			{
				return this.itemIds.Length;
			}
		}

		private void OnBeforeMessageDisposition(MessageItem messageItem)
		{
			if (this.beforeMessageDispositionEventHandler != null)
			{
				this.beforeMessageDispositionEventHandler(messageItem);
			}
		}

		private IdAndSession GetSaveToFolderIdFromRequest()
		{
			IdAndSession idAndSession = null;
			if (this.targetFolderToSaveIn != null)
			{
				if (!this.saveInFolder)
				{
					throw new InvalidSendItemSaveSettingsException();
				}
				try
				{
					idAndSession = base.IdConverter.ConvertTargetFolderIdToIdAndContentSession(this.targetFolderToSaveIn.BaseFolderId, true);
				}
				catch (ObjectNotFoundException innerException)
				{
					throw new SavedItemFolderNotFoundException(innerException);
				}
				if (idAndSession.Session is PublicFolderSession)
				{
					throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorCannotSaveSentItemInPublicFolder);
				}
				if (idAndSession.Session is MailboxSession && ((MailboxSession)idAndSession.Session).MailboxOwner.MailboxInfo.IsArchive)
				{
					throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorCannotSaveSentItemInArchiveFolder);
				}
			}
			return idAndSession;
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			IdAndSession idAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadWrite(this.itemIds[base.CurrentStep]);
			if (idAndSession.Session is PublicFolderSession)
			{
				throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorCannotSendMessageFromPublicFolder);
			}
			IdAndSession sentItemsFolderId = this.GetSentItemsFolderId(idAndSession);
			return this.SendItemById(idAndSession, sentItemsFolderId);
		}

		private IdAndSession GetSentItemsFolderId(IdAndSession itemIdToSend)
		{
			IdAndSession result = null;
			if (this.saveInFolder)
			{
				if (this.saveFolderIdAndSession != null)
				{
					result = this.saveFolderIdAndSession;
				}
				else
				{
					try
					{
						result = base.IdConverter.ConvertDefaultFolderType(DefaultFolderType.SentItems, ((MailboxSession)itemIdToSend.Session).MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
					}
					catch (ObjectNotFoundException innerException)
					{
						throw new SavedItemFolderNotFoundException(innerException);
					}
				}
			}
			return result;
		}

		private ServiceResult<ServiceResultNone> SendItemById(IdAndSession sendIdAndSession, IdAndSession folderIdAndSessionToSaveIn)
		{
			using (Item xsoItemForUpdate = ServiceCommandBase.GetXsoItemForUpdate(sendIdAndSession, new PropertyDefinition[]
			{
				MessageItemSchema.Flags
			}))
			{
				MessageItem messageItem = xsoItemForUpdate as MessageItem;
				this.ValidateMessageToSend(messageItem);
				this.OnBeforeMessageDisposition(messageItem);
				try
				{
					ServiceCommandBase.RequireUpToDateItem(sendIdAndSession.Id, xsoItemForUpdate);
					if (folderIdAndSessionToSaveIn == null)
					{
						messageItem.SendWithoutSavingMessage();
					}
					else if (folderIdAndSessionToSaveIn.Id.Equals(messageItem.ParentId))
					{
						messageItem.SendWithoutMovingMessage(folderIdAndSessionToSaveIn.GetAsStoreObjectId(), SaveMode.ResolveConflicts);
					}
					else
					{
						messageItem.Send(folderIdAndSessionToSaveIn.GetAsStoreObjectId(), SaveMode.ResolveConflicts);
					}
				}
				catch (InvalidRecipientsException ex)
				{
					if (messageItem.Recipients.Count == 0)
					{
						throw new MissingRecipientsException(ex.InnerException);
					}
					throw;
				}
			}
			this.objectsChanged++;
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		private void ValidateMessageToSend(MessageItem messageToSend)
		{
			if (messageToSend == null || (!(messageToSend is MeetingResponse) && Shape.IsGenericMessageOnly(messageToSend) && !ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1)))
			{
				throw new InvalidItemForOperationException("SendItem");
			}
			if (ServiceCommandBase.IsAssociated(messageToSend))
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)3859804741U);
			}
		}

		private BaseItemId[] itemIds;

		private TargetFolderId targetFolderToSaveIn;

		private bool saveInFolder;

		private IdAndSession saveFolderIdAndSession;

		private Action<MessageItem> beforeMessageDispositionEventHandler;
	}
}
