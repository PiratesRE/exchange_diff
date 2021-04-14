using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class MoveCopyItemCommandBase : MoveCopyCommandBase<BaseMoveCopyItemRequest, ItemType>
	{
		public MoveCopyItemCommandBase(CallContext callContext, BaseMoveCopyItemRequest request) : base(callContext, request)
		{
		}

		protected override void PrepareCommandMembers()
		{
			ExTraceGlobals.MoveCopyItemCommandBaseCallTracer.TraceDebug((long)this.GetHashCode(), "MoveCopyItemCommandBase.PrepareCommandMembers called");
			this.objectIds = base.Request.Ids;
			this.destinationFolderId = base.Request.ToFolderId.BaseFolderId;
			ServiceCommandBase.ThrowIfNullOrEmpty<ServiceObjectId>(this.objectIds, "objectIds", "PrepareCommandMembers::Execute");
			ServiceCommandBase.ThrowIfNull(this.destinationFolderId, "destinationFolderId", "PrepareCommandMembers::Execute");
		}

		protected override StoreObject BindObjectFromRequest(StoreSession storeSession, StoreId storeId)
		{
			Item xsoItem = ServiceCommandBase.GetXsoItem(storeSession, storeId, new PropertyDefinition[]
			{
				MessageItemSchema.Flags
			});
			if (ServiceCommandBase.IsAssociated(xsoItem))
			{
				xsoItem.Dispose();
				throw new ServiceInvalidOperationException((CoreResources.IDs)3859804741U);
			}
			return xsoItem;
		}

		protected override ServiceResult<ItemType> PrepareResult(StoreObject storeObject, GroupOperationResult[] groupOperationResults)
		{
			if (groupOperationResults == null || groupOperationResults.Length != 1 || groupOperationResults[0].ResultObjectIds == null || groupOperationResults[0].ResultObjectIds.Count != 1)
			{
				return new ServiceResult<ItemType>(null);
			}
			StoreId storeId = IdConverter.CombineStoreObjectIdWithChangeKey(groupOperationResults[0].ResultObjectIds[0], groupOperationResults[0].ResultChangeKeys[0]);
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(storeId);
			IdAndSession idAndSession = new IdAndSession(storeId, this.destinationFolder.Id, this.destinationFolder.Session);
			ItemType itemType = ItemType.CreateFromStoreObjectType(storeObjectId.ObjectType);
			itemType.ItemId = base.GetServiceItemIdFromStoreId(storeId, idAndSession);
			return new ServiceResult<ItemType>(itemType);
		}

		protected override IdAndSession GetIdAndSession(ServiceObjectId objectId)
		{
			BaseItemId baseItemId = objectId as BaseItemId;
			return base.IdConverter.ConvertItemIdToIdAndSessionReadOnly(baseItemId);
		}

		protected override Folder GetDestinationFolder()
		{
			IdAndSession idAndSession = null;
			try
			{
				idAndSession = base.IdConverter.ConvertTargetFolderIdToIdAndContentSession(this.destinationFolderId, true);
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new ToFolderNotFoundException(innerException);
			}
			return Folder.Bind(idAndSession.Session, idAndSession.Id, null);
		}
	}
}
