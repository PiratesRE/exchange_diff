using System;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class DeleteFolder : DeleteCommandBase<DeleteFolderRequest, ServiceResultNone>
	{
		public DeleteFolder(CallContext callContext, DeleteFolderRequest request) : base(callContext, request)
		{
		}

		internal override void PreExecuteCommand()
		{
			this.objectIds = base.Request.Ids.OfType<ServiceObjectId>().ToList<ServiceObjectId>();
			this.disposalType = base.Request.DeleteType;
			ServiceCommandBase.ThrowIfNullOrEmpty<ServiceObjectId>(this.objectIds, "objectIds", "DeleteFolder::PreExecuteCommand");
		}

		protected override ServiceResult<ServiceResultNone> DeleteObject(IdAndSession idAndSession, bool returnNewItemIds)
		{
			PublicFolderSession publicFolderSession = idAndSession.Session as PublicFolderSession;
			if (publicFolderSession == null || publicFolderSession.IsPrimaryHierarchySession)
			{
				return base.DeleteObject(idAndSession, returnNewItemIds);
			}
			return this.RemoteDeletePublicFolder(idAndSession);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			DeleteFolderResponse deleteFolderResponse = new DeleteFolderResponse();
			deleteFolderResponse.BuildForNoReturnValue(base.Results);
			return deleteFolderResponse;
		}

		protected override IdAndSession GetIdAndSession(ServiceObjectId objectId)
		{
			BaseFolderId folderId = objectId as BaseFolderId;
			return base.IdConverter.ConvertFolderIdToIdAndSession(folderId, IdConverter.ConvertOption.IgnoreChangeKey | IdConverter.ConvertOption.AllowKnownExternalUsers | (base.Request.IsHierarchicalOperation ? IdConverter.ConvertOption.IsHierarchicalOperation : IdConverter.ConvertOption.None));
		}

		protected override StoreObject GetStoreObject(IdAndSession idAndSession)
		{
			return Folder.Bind(idAndSession.Session, idAndSession.Id, null);
		}

		protected override ServiceResult<ServiceResultNone> TryExecuteDelete(StoreObject storeObject, DeleteItemFlags deleteItemFlags, IdAndSession idAndSession, out LocalizedException localizedException, bool returnNewId)
		{
			AggregateOperationResult aggregateOperationResult;
			if (storeObject is OutlookSearchFolder)
			{
				MailboxSession session = (MailboxSession)storeObject.Session;
				aggregateOperationResult = OutlookSearchFolder.DeleteOutlookSearchFolder(deleteItemFlags, session, storeObject.Id);
			}
			else
			{
				aggregateOperationResult = base.ExecuteDelete(storeObject, deleteItemFlags, idAndSession, returnNewId);
			}
			bool flag = aggregateOperationResult.OperationResult == OperationResult.Succeeded;
			localizedException = (flag ? null : aggregateOperationResult.GroupOperationResults[0].Exception);
			ExTraceGlobals.DeleteFolderCallTracer.TraceDebug<OperationResult>(0L, "Delete item operation result '{0}'", aggregateOperationResult.OperationResult);
			if (!flag)
			{
				return null;
			}
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		protected override ServiceResult<ServiceResultNone> CreateEmptyResponseType()
		{
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		protected override void ExecuteCommandValidations(StoreObject storeObject)
		{
			PublicFolderSession publicFolderSession = storeObject.Session as PublicFolderSession;
			if (publicFolderSession != null)
			{
				StoreId ipmSubtreeFolderId = publicFolderSession.GetIpmSubtreeFolderId();
				if (storeObject.Id.Equals(ipmSubtreeFolderId))
				{
					throw new DeleteItemsException((CoreResources.IDs)2671356913U);
				}
			}
		}

		private ServiceResult<ServiceResultNone> RemoteDeletePublicFolder(IdAndSession idAndSession)
		{
			ServiceResult<ServiceResultNone> serviceResult = RemotePublicFolderOperations.DeleteFolder(base.CallContext, idAndSession, this.disposalType);
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
				return new ServiceResult<ServiceResultNone>(new ServiceError((CoreResources.IDs)2636256287U, ResponseCodeType.ErrorInternalServerError, 0, ExchangeVersion.Exchange2012));
			}
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}
	}
}
