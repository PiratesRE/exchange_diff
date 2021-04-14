using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class MoveCopyItemBatchCommandBase<RequestType, MoveCopyCommand> : ItemBatchCommandBase<RequestType, ItemType>, IDisposeTrackable, IDisposable where RequestType : BaseMoveCopyItemRequest where MoveCopyCommand : MoveCopyItemCommandBase
	{
		public MoveCopyItemBatchCommandBase(CallContext callContext, RequestType request) : base(callContext, request)
		{
			this.ServiceResults = null;
			this.disposeTracker = this.GetDisposeTracker();
		}

		internal override int StepCount
		{
			get
			{
				RequestType request = base.Request;
				return request.Ids.Length;
			}
		}

		internal override void PreExecuteCommand()
		{
			RequestType request = base.Request;
			this.ItemIds = request.ItemStoreIds;
			try
			{
				IdConverter idConverter = base.IdConverter;
				RequestType request2 = base.Request;
				this.DestFolderIdAndSession = idConverter.ConvertFolderIdToIdAndSessionReadOnly(request2.ToFolderId.BaseFolderId);
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new ToFolderNotFoundException(innerException);
			}
			try
			{
				RequestType request3 = base.Request;
				BaseItemId baseItemId = request3.Ids[0];
				this.SourceFolderIdAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadOnly(baseItemId);
			}
			catch (ObjectNotFoundException)
			{
				throw new MoveCopyException();
			}
			RequestType request4 = base.Request;
			this.ReturnNewItemIds = request4.ReturnNewItemIds;
		}

		protected StoreId FindMovedOrCopiedItemId(Folder destFolder, byte[] searchKey)
		{
			ComparisonFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.SearchKey, searchKey);
			StoreId result;
			using (QueryResult queryResult = destFolder.ItemQuery(ItemQueryType.None, queryFilter, null, new PropertyDefinition[]
			{
				ItemSchema.Id
			}))
			{
				object[][] rows = queryResult.GetRows(1);
				if (rows.Length > 0)
				{
					result = (StoreId)rows[0][0];
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		protected bool TryCopyItemBatch(out int itemsCopied)
		{
			Func<AggregateOperationResult> batchCall = () => this.SourceFolderIdAndSession.Session.Copy(this.DestFolderIdAndSession.Session, this.DestFolderIdAndSession.Id, true, this.ItemIds.ToArray());
			return this.TryExecuteBatchCall(batchCall, out itemsCopied);
		}

		protected bool TryMoveItemBatch(out int itemsMoved)
		{
			Func<AggregateOperationResult> batchCall = () => this.SourceFolderIdAndSession.Session.Move(this.DestFolderIdAndSession.Session, this.DestFolderIdAndSession.Id, true, this.ItemIds.ToArray());
			return this.TryExecuteBatchCall(batchCall, out itemsMoved);
		}

		private bool TryExecuteBatchCall(Func<AggregateOperationResult> batchCall, out int itemsChanged)
		{
			bool flag = false;
			itemsChanged = 0;
			if (!base.VerifyItemsCanBeBatched(this.ItemIds, this.SourceFolderIdAndSession, this.DestFolderIdAndSession.Session, ref flag))
			{
				this.ServiceResults = null;
				return false;
			}
			StoreSession session = this.SourceFolderIdAndSession.Session;
			Dictionary<StoreId, byte[]> dictionary = new Dictionary<StoreId, byte[]>();
			if (this.ReturnNewItemIds && !this.BuildItemIdToSearchKeyDictionary(dictionary, session, this.ItemIds))
			{
				this.ServiceResults = null;
				return false;
			}
			AggregateOperationResult aggregateOperationResult = batchCall();
			switch (aggregateOperationResult.OperationResult)
			{
			case OperationResult.Succeeded:
				itemsChanged = this.ItemIds.Count;
				return this.PrepareOperationSucceededResults(aggregateOperationResult.GroupOperationResults, dictionary);
			case OperationResult.Failed:
				return this.PrepareMoveCopyFailedServiceResults();
			case OperationResult.PartiallySucceeded:
				return this.PreparePartiallySucceededResults(dictionary, ref itemsChanged);
			default:
				this.ServiceResults = null;
				return false;
			}
		}

		private bool PrepareOperationSucceededResults(GroupOperationResult[] groupOperationResults, Dictionary<StoreId, byte[]> searchKeyDictionary)
		{
			if (groupOperationResults == null || groupOperationResults.Count<GroupOperationResult>() == 0 || groupOperationResults[0].ResultObjectIds == null || groupOperationResults[0].ResultObjectIds.Count<StoreObjectId>() < this.ItemIds.Count)
			{
				return this.PrepareEmptySuccessfulServiceResults();
			}
			List<StoreId> list = new List<StoreId>();
			for (int i = 0; i < groupOperationResults[0].ResultObjectIds.Count; i++)
			{
				StoreId item = IdConverter.CombineStoreObjectIdWithChangeKey(groupOperationResults[0].ResultObjectIds[i], groupOperationResults[0].ResultChangeKeys[i]);
				list.Add(item);
			}
			return this.PrepareServiceResultForItemIds(list);
		}

		private bool PreparePartiallySucceededResults(Dictionary<StoreId, byte[]> itemIdToSearchKeyDictionary, ref int itemsChanged)
		{
			if (!this.ReturnNewItemIds)
			{
				return this.PrepareMoveCopyFailedServiceResults();
			}
			using (Folder folder = Folder.Bind(this.DestFolderIdAndSession.Session, this.DestFolderIdAndSession.Id, null))
			{
				this.ServiceResults = new ServiceResult<ItemType>[this.ItemIds.Count];
				for (int i = 0; i < this.ItemIds.Count; i++)
				{
					StoreId key = this.ItemIds[i];
					byte[] searchKey = itemIdToSearchKeyDictionary[key];
					StoreId storeId = this.FindMovedOrCopiedItemId(folder, searchKey);
					if (storeId != null)
					{
						itemsChanged++;
						this.ServiceResults[i] = this.PrepareServiceResultForItemId(storeId);
					}
					else
					{
						this.ServiceResults[i] = new ServiceResult<ItemType>(this.CreateMoveCopyFailedServiceError());
					}
				}
			}
			return true;
		}

		private bool PrepareEmptySuccessfulServiceResults()
		{
			this.ServiceResults = this.PrepareBulkServiceResults(this.ItemIds.Count, () => new ServiceResult<ItemType>(null));
			return true;
		}

		private bool PrepareMoveCopyFailedServiceResults()
		{
			ServiceError serviceError = this.CreateMoveCopyFailedServiceError();
			this.ServiceResults = this.PrepareBulkServiceResults(this.ItemIds.Count, () => new ServiceResult<ItemType>(serviceError));
			return true;
		}

		private ServiceResult<ItemType>[] PrepareBulkServiceResults(int count, Func<ServiceResult<ItemType>> createServiceResult)
		{
			ServiceResult<ItemType>[] array = new ServiceResult<ItemType>[count];
			for (int i = 0; i < this.ItemIds.Count; i++)
			{
				array[i] = createServiceResult();
			}
			return array;
		}

		private ServiceError CreateMoveCopyFailedServiceError()
		{
			return new ServiceError((CoreResources.IDs)2524108663U, ResponseCodeType.ErrorMoveCopyFailed, 0, ExchangeVersion.Exchange2007SP1);
		}

		private bool PrepareServiceResultForItemIds(IList<StoreId> itemIdList)
		{
			this.ServiceResults = new ServiceResult<ItemType>[itemIdList.Count<StoreId>()];
			for (int i = 0; i < itemIdList.Count<StoreId>(); i++)
			{
				this.ServiceResults[i] = this.PrepareServiceResultForItemId(itemIdList[i]);
			}
			return true;
		}

		private ServiceResult<ItemType> PrepareServiceResultForItemId(StoreId itemId)
		{
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(itemId);
			ItemType itemType = ItemType.CreateFromStoreObjectType(storeObjectId.ObjectType);
			itemType.ItemId = base.GetServiceItemIdFromStoreId(itemId, this.DestFolderIdAndSession);
			return new ServiceResult<ItemType>(itemType);
		}

		private bool BuildItemIdToSearchKeyDictionary(Dictionary<StoreId, byte[]> searchKeyDictionary, StoreSession session, List<StoreId> itemIds)
		{
			foreach (StoreId storeId in itemIds)
			{
				byte[] itemSearchKey = this.GetItemSearchKey(session, storeId);
				if (itemSearchKey == null || searchKeyDictionary.Keys.Contains(storeId))
				{
					return false;
				}
				searchKeyDictionary.Add(storeId, itemSearchKey);
			}
			return true;
		}

		private byte[] GetItemSearchKey(StoreSession session, StoreId itemId)
		{
			byte[] result;
			try
			{
				using (Item item = Item.Bind(session, itemId, new PropertyDefinition[]
				{
					StoreObjectSchema.SearchKey
				}))
				{
					result = (byte[])item.TryGetProperty(StoreObjectSchema.SearchKey);
				}
			}
			catch (ObjectNotFoundException)
			{
				result = null;
			}
			return result;
		}

		internal override void PostExecuteCommand()
		{
			if (this.FallbackCommand != null)
			{
				MoveCopyCommand fallbackCommand = this.FallbackCommand;
				fallbackCommand.PostExecuteCommand();
			}
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MoveCopyItemBatchCommandBase<RequestType, MoveCopyCommand>>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool fromDispose)
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			if (!this.disposed)
			{
				if (this.FallbackCommand != null)
				{
					MoveCopyCommand fallbackCommand = this.FallbackCommand;
					fallbackCommand.Dispose();
					this.FallbackCommand = default(MoveCopyCommand);
				}
				this.disposed = true;
			}
		}

		protected MoveCopyCommand FallbackCommand { get; set; }

		protected List<StoreId> ItemIds { get; set; }

		protected IdAndSession SourceFolderIdAndSession { get; set; }

		protected IdAndSession DestFolderIdAndSession { get; set; }

		protected bool ReturnNewItemIds { get; set; }

		protected ServiceResult<ItemType>[] ServiceResults { get; set; }

		private readonly DisposeTracker disposeTracker;

		private bool disposed;
	}
}
