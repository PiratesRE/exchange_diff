using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class MoveFolder : MoveCopyFolderCommandBase
	{
		public MoveFolder(CallContext callContext, MoveFolderRequest request) : base(callContext, request)
		{
		}

		protected override BaseInfoResponse CreateResponse()
		{
			return new MoveFolderResponse();
		}

		protected override AggregateOperationResult DoOperation(StoreSession destinationSession, StoreSession sourceSession, StoreId sourceId)
		{
			return sourceSession.Move(destinationSession, this.destinationFolder.Id, new StoreId[]
			{
				sourceId
			});
		}

		protected override ServiceResult<BaseFolderType> MoveCopyObject(IdAndSession idAndSession)
		{
			PublicFolderSession publicFolderSession = idAndSession.Session as PublicFolderSession;
			if (publicFolderSession == null || publicFolderSession.IsPrimaryHierarchySession)
			{
				return base.MoveCopyObject(idAndSession);
			}
			return this.RemoteMovePublicFolder(idAndSession);
		}

		protected override void SubclassValidateOperation(StoreSession storeSession, IdAndSession idAndSession)
		{
			if ((idAndSession.Session is PublicFolderSession && this.destinationFolder.Session is MailboxSession) || (idAndSession.Session is MailboxSession && this.destinationFolder.Session is PublicFolderSession))
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)3206878473U);
			}
			if (idAndSession.Session is MailboxSession)
			{
				MailboxSession mailboxSession = idAndSession.Session as MailboxSession;
				DefaultFolderType defaultFolderType = mailboxSession.IsDefaultFolderType(idAndSession.Id);
				if (defaultFolderType != DefaultFolderType.None)
				{
					throw new MoveDistinguishedFolderException();
				}
			}
		}

		private ServiceResult<BaseFolderType> RemoteMovePublicFolder(IdAndSession idAndSession)
		{
			ServiceResult<BaseFolderType> serviceResult = RemotePublicFolderOperations.MoveFolder(base.CallContext, idAndSession, this.destinationFolder.Id.ObjectId);
			if (serviceResult.Error != null)
			{
				return serviceResult;
			}
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(idAndSession.Id);
			bool flag = false;
			try
			{
				PublicFolderSyncJobRpc.SyncFolder(((PublicFolderSession)idAndSession.Session).MailboxPrincipal, storeObjectId.ProviderLevelItemId);
			}
			catch (PublicFolderSyncTransientException)
			{
				flag = true;
			}
			catch (PublicFolderSyncPermanentException)
			{
				flag = true;
			}
			if (flag)
			{
				return new ServiceResult<BaseFolderType>(new ServiceError((CoreResources.IDs)2636256287U, ResponseCodeType.ErrorInternalServerError, 0, ExchangeVersion.Exchange2012));
			}
			ServiceResult<BaseFolderType> result;
			using (Folder folder = Folder.Bind(idAndSession.Session, storeObjectId))
			{
				BaseFolderType baseFolderType = BaseFolderType.CreateFromStoreObjectType(storeObjectId.ObjectType);
				baseFolderType.FolderId = base.GetServiceFolderIdFromStoreId(StoreId.GetStoreObjectId(folder.Id), idAndSession);
				result = new ServiceResult<BaseFolderType>(baseFolderType);
			}
			return result;
		}
	}
}
