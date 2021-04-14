using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class UpdateFolder : MultiStepServiceCommand<UpdateFolderRequest, BaseFolderType>
	{
		public UpdateFolder(CallContext callContext, UpdateFolderRequest request) : base(callContext, request)
		{
		}

		internal override void PreExecuteCommand()
		{
			this.folderChanges = base.Request.FolderChanges;
			this.responseShape = ServiceCommandBase.DefaultFolderResponseShape;
			ServiceCommandBase.ThrowIfNullOrEmpty<FolderChange>(this.folderChanges, "this.folderChanges", "UpdateFolder::Execute");
			ServiceCommandBase.ThrowIfNull(this.responseShape, "this.responseShape", "UpdateFolder::Execute");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			UpdateFolderResponse updateFolderResponse = new UpdateFolderResponse();
			updateFolderResponse.BuildForResults<BaseFolderType>(base.Results);
			return updateFolderResponse;
		}

		internal override int StepCount
		{
			get
			{
				return this.folderChanges.Length;
			}
		}

		internal override ServiceResult<BaseFolderType> Execute()
		{
			if (this.folderChanges[base.CurrentStep].FolderId == null)
			{
				throw new InvalidFolderIdException(CoreResources.IDs.ErrorInvalidFolderId);
			}
			IdAndSession idAndSession = base.IdConverter.ConvertFolderIdToIdAndSession(this.folderChanges[base.CurrentStep].FolderId, base.Request.IsHierarchicalOperation ? IdConverter.ConvertOption.IsHierarchicalOperation : IdConverter.ConvertOption.None);
			PublicFolderSession publicFolderSession = idAndSession.Session as PublicFolderSession;
			ServiceResult<BaseFolderType> result;
			if (publicFolderSession == null || publicFolderSession.IsPrimaryHierarchySession)
			{
				result = new ServiceResult<BaseFolderType>(this.UpdateFolderFromServiceObject(idAndSession, this.folderChanges[base.CurrentStep]));
			}
			else
			{
				result = this.RemoteUpdatePublicFolder(idAndSession, this.folderChanges[base.CurrentStep]);
			}
			this.objectsChanged++;
			return result;
		}

		private BaseFolderType UpdateFolderFromServiceObject(IdAndSession idAndSession, FolderChange folderChange)
		{
			BaseFolderType baseFolderType = null;
			ToServiceObjectPropertyList toServiceObjectPropertyList = XsoDataConverter.GetToServiceObjectPropertyList(idAndSession.Id, idAndSession.Session, this.responseShape, base.ParticipantResolver);
			using (Folder xsoFolder = ServiceCommandBase.GetXsoFolder(idAndSession.Session, idAndSession.Id, ref toServiceObjectPropertyList))
			{
				this.UpdateProperties(xsoFolder, folderChange.PropertyUpdates, false);
				this.SaveXsoFolder(xsoFolder);
				StoreObjectId storeObjectId = StoreId.GetStoreObjectId(idAndSession.Id);
				baseFolderType = BaseFolderType.CreateFromStoreObjectType(storeObjectId.ObjectType);
				ServiceCommandBase.LoadServiceObject(baseFolderType, xsoFolder, idAndSession, this.responseShape, toServiceObjectPropertyList);
			}
			return baseFolderType;
		}

		private ServiceResult<BaseFolderType> RemoteUpdatePublicFolder(IdAndSession idAndSession, FolderChange folderChange)
		{
			ServiceResult<BaseFolderType> serviceResult = RemotePublicFolderOperations.UpdateFolder(base.CallContext, idAndSession, folderChange);
			if (serviceResult.Error != null)
			{
				return serviceResult;
			}
			StoreObjectId storeObjectId = StoreId.EwsIdToFolderStoreObjectId(serviceResult.Value.FolderId.Id);
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
			ToServiceObjectPropertyList toServiceObjectPropertyList = XsoDataConverter.GetToServiceObjectPropertyList(idAndSession.Id, idAndSession.Session, this.responseShape, base.ParticipantResolver);
			StoreObjectId storeObjectId2 = StoreId.GetStoreObjectId(idAndSession.Id);
			ServiceResult<BaseFolderType> result;
			using (Folder xsoFolder = ServiceCommandBase.GetXsoFolder(idAndSession.Session, storeObjectId2, ref toServiceObjectPropertyList))
			{
				BaseFolderType baseFolderType = BaseFolderType.CreateFromStoreObjectType(storeObjectId2.ObjectType);
				ServiceCommandBase.LoadServiceObject(baseFolderType, xsoFolder, idAndSession, this.responseShape, toServiceObjectPropertyList);
				result = new ServiceResult<BaseFolderType>(baseFolderType);
			}
			return result;
		}

		private FolderChange[] folderChanges;

		private FolderResponseShape responseShape;
	}
}
