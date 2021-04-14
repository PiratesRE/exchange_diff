using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class MoveCopyFolderCommandBase : MoveCopyCommandBase<BaseMoveCopyFolderRequest, BaseFolderType>
	{
		public MoveCopyFolderCommandBase(CallContext callContext, BaseMoveCopyFolderRequest request) : base(callContext, request)
		{
		}

		protected override void PrepareCommandMembers()
		{
			ExTraceGlobals.MoveCopyFolderCommandBaseCallTracer.TraceDebug((long)this.GetHashCode(), "MoveCopyFolderCommandBase.PrepareCommandMembers called");
			this.objectIds = base.Request.Ids;
			this.destinationFolderId = base.Request.ToFolderId.BaseFolderId;
			this.responseShape = ServiceCommandBase.DefaultFolderResponseShape;
			ServiceCommandBase.ThrowIfNullOrEmpty<ServiceObjectId>(this.objectIds, "objectIds", "MoveCopyFolderCommandBase::PrepareCommandMembers");
			ServiceCommandBase.ThrowIfNull(this.destinationFolderId, "destinationFolderId", "MoveCopyFolderCommandBase::PrepareCommandMembers");
			ServiceCommandBase.ThrowIfNull(this.responseShape, "responseShape", "MoveCopyFolderCommandBase::PrepareCommandMembers");
		}

		protected override StoreObject BindObjectFromRequest(StoreSession storeSession, StoreId storeId)
		{
			ToXmlPropertyList propertyList = XsoDataConverter.GetPropertyList(storeId, storeSession, this.responseShape);
			return ServiceCommandBase.GetXsoFolder(storeSession, storeId, propertyList);
		}

		protected override ServiceResult<BaseFolderType> PrepareResult(StoreObject originalStoreObject, GroupOperationResult[] groupOperationResults)
		{
			StoreSession session = this.destinationFolder.Session;
			StoreId resultingObjectId = this.GetResultingObjectId(originalStoreObject);
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(resultingObjectId);
			IdAndSession idAndSession = new IdAndSession(resultingObjectId, session);
			BaseFolderType baseFolderType = BaseFolderType.CreateFromStoreObjectType(storeObjectId.ObjectType);
			baseFolderType.FolderId = base.GetServiceFolderIdFromStoreId(storeObjectId, idAndSession);
			return new ServiceResult<BaseFolderType>(baseFolderType);
		}

		private StoreId GetResultingObjectId(StoreObject originalObject)
		{
			Folder folder = originalObject as Folder;
			ComparisonFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.DisplayName, folder.DisplayName);
			StoreId result;
			using (QueryResult queryResult = this.destinationFolder.FolderQuery(FolderQueryFlags.None, queryFilter, null, new PropertyDefinition[]
			{
				FolderSchema.Id
			}))
			{
				if (queryResult.EstimatedRowCount == 0)
				{
					throw new MoveCopyException();
				}
				object[][] rows = queryResult.GetRows(1);
				result = (VersionedId)rows[0][0];
			}
			return result;
		}

		protected override IdAndSession GetIdAndSession(ServiceObjectId objectId)
		{
			BaseFolderId folderId = objectId as BaseFolderId;
			return base.IdConverter.ConvertFolderIdToIdAndSession(folderId, IdConverter.ConvertOption.IgnoreChangeKey | IdConverter.ConvertOption.AllowKnownExternalUsers | (base.Request.IsHierarchicalOperation ? IdConverter.ConvertOption.IsHierarchicalOperation : IdConverter.ConvertOption.None));
		}

		private ResponseShape responseShape;
	}
}
