using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class UpdateItemInRecoverableItems : CreateUpdateItemCommandBase<UpdateItemInRecoverableItemsRequest, UpdateItemInRecoverableItemsResponseWrapper>, IDisposeTrackable, IDisposable
	{
		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<UpdateItemInRecoverableItems>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public UpdateItemInRecoverableItems(CallContext callContext, UpdateItemInRecoverableItemsRequest request) : base(callContext, request)
		{
			this.messageDisposition = new MessageDispositionType?(MessageDispositionType.SaveOnly);
			this.savedItemFolderId = new TargetFolderId(new DistinguishedFolderId
			{
				Id = DistinguishedFolderIdName.recoverableitemspurges
			});
			this.responseShape = ServiceCommandBase.DefaultItemResponseShape;
			this.conflictResolutionType = ConflictResolutionType.AlwaysOverwrite;
			this.disposeTracker = this.GetDisposeTracker();
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

		internal override void PreExecuteCommand()
		{
			this.itemId = base.Request.ItemId;
			this.updates = base.Request.PropertyUpdates;
			this.attachments = base.Request.Attachments;
			this.makeItemImmutable = base.Request.MakeItemImmutable;
			ServiceCommandBase.ThrowIfNull(this.itemId, "this.itemId", "UpdateItemInRecoverableItems::PreExecuteCommand");
		}

		internal override int StepCount
		{
			get
			{
				return 1;
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			UpdateItemInRecoverableItemsResponse updateItemInRecoverableItemsResponse = new UpdateItemInRecoverableItemsResponse();
			updateItemInRecoverableItemsResponse.BuildForUpdateItemInRecoverableItemsResults(base.Results);
			return updateItemInRecoverableItemsResponse;
		}

		internal override ServiceResult<UpdateItemInRecoverableItemsResponseWrapper> Execute()
		{
			try
			{
				this.saveToFolderIdAndSession = base.IdConverter.ConvertFolderIdToIdAndSessionReadOnly(this.savedItemFolderId.BaseFolderId);
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new SavedItemFolderNotFoundException(innerException);
			}
			UpdateItemInRecoverableItemsResponseWrapper value = this.DoUpdateItemInRecoverableItems();
			this.objectsChanged++;
			return new ServiceResult<UpdateItemInRecoverableItemsResponseWrapper>(value);
		}

		protected override ConflictResolutionResult ExecuteCalendarOperation(CalendarItemBase calendarItem, ConflictResolutionType resolutionType)
		{
			throw new NotImplementedException("ExecuteCalendarOperation should not be called in UpdateItemInRecoverableItems");
		}

		private UpdateItemInRecoverableItemsResponseWrapper DoUpdateItemInRecoverableItems()
		{
			IdAndSession idAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadWrite(this.itemId);
			ConflictResults conflictResults = new ConflictResults();
			UpdateItemInRecoverableItemsResponseWrapper result;
			using (Item xsoItemForUpdate = ServiceCommandBase.GetXsoItemForUpdate(idAndSession, new PropertyDefinition[]
			{
				MessageItemSchema.Flags
			}))
			{
				this.ValidateItemToUpdateExistInCorrectFolder(xsoItemForUpdate);
				this.ValidateItemToUpdateIsDraft(xsoItemForUpdate);
				if (this.makeItemImmutable)
				{
					xsoItemForUpdate[InternalSchema.IsDraft] = false;
				}
				ItemType itemType = null;
				bool flag = false;
				if (this.updates != null && this.updates.Length > 0)
				{
					this.UpdateProperties(xsoItemForUpdate, this.updates, false);
					ConflictResolutionResult conflictResolutionResult;
					itemType = base.ExecuteOperation(xsoItemForUpdate, this.responseShape, this.conflictResolutionType, out conflictResolutionResult);
					if (conflictResolutionResult != null && conflictResolutionResult.SaveStatus == SaveResult.SuccessWithConflictResolution && conflictResolutionResult.PropertyConflicts != null)
					{
						conflictResults.Count = conflictResolutionResult.PropertyConflicts.Length;
					}
					flag = true;
				}
				AttachmentType[] array = null;
				if (this.attachments != null && this.attachments.Length > 0)
				{
					List<AttachmentType> list = new List<AttachmentType>(this.attachments.Length);
					AttachmentHierarchy attachmentHierarchy = new AttachmentHierarchy(idAndSession, true, false);
					this.attachmentBuilder = new AttachmentBuilder(attachmentHierarchy, this.attachments, base.IdConverter);
					for (int i = 0; i < this.attachments.Length; i++)
					{
						ServiceError serviceError = null;
						using (Attachment attachment = this.attachmentBuilder.CreateAttachment(this.attachments[i], out serviceError))
						{
							list.Add(this.CreateAttachmentResult(attachment, idAndSession));
						}
					}
					attachmentHierarchy.SaveAll();
					attachmentHierarchy.RootItem.Load();
					using (Item xsoItem = ServiceCommandBase.GetXsoItem(idAndSession.Session, idAndSession.Id, new PropertyDefinition[]
					{
						MessageItemSchema.Flags
					}))
					{
						StoreObjectId storeObjectId = StoreId.GetStoreObjectId(idAndSession.Id);
						itemType = ItemType.CreateFromStoreObjectType(storeObjectId.ObjectType);
						base.LoadServiceObject(itemType, xsoItem, idAndSession, this.responseShape);
					}
					for (int j = 0; j < list.Count; j++)
					{
						AttachmentType attachmentType = list[j];
						ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(attachmentHierarchy.RootItem.Id, idAndSession, null);
						attachmentType.AttachmentId.RootItemId = concatenatedId.Id;
						attachmentType.AttachmentId.RootItemChangeKey = concatenatedId.ChangeKey;
						if (j == list.Count - 1)
						{
							itemType.ItemId.Id = concatenatedId.Id;
							itemType.ItemId.ChangeKey = concatenatedId.ChangeKey;
						}
					}
					array = list.ToArray();
					if (this.makeItemImmutable && !flag)
					{
						xsoItemForUpdate.Save(SaveMode.ResolveConflicts);
						flag = true;
					}
				}
				if (itemType == null)
				{
					throw new ServiceArgumentException((CoreResources.IDs)3654096821U);
				}
				result = new UpdateItemInRecoverableItemsResponseWrapper(itemType, array, conflictResults);
			}
			return result;
		}

		private void ValidateItemToUpdateExistInCorrectFolder(Item storeItem)
		{
			StoreObjectId parentIdFromItemId = IdConverter.GetParentIdFromItemId(storeItem.StoreObjectId);
			StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(this.saveToFolderIdAndSession.Id);
			if (!parentIdFromItemId.Equals(asStoreObjectId))
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)2622305962U);
			}
		}

		private void ValidateItemToUpdateIsDraft(Item storeItem)
		{
			MessageItem messageItem;
			if (XsoDataConverter.TryGetStoreObject<MessageItem>(storeItem, out messageItem) && !messageItem.IsDraft)
			{
				throw new ServiceInvalidOperationException(CoreResources.IDs.UpdateNonDraftItemInDumpsterNotAllowed);
			}
		}

		private AttachmentType CreateAttachmentResult(Attachment attachment, IdAndSession itemIdAndSession)
		{
			AttachmentType attachmentType;
			if (attachment is StreamAttachment)
			{
				attachmentType = new FileAttachmentType();
			}
			else
			{
				attachmentType = new ItemAttachmentType();
			}
			IdAndSession idAndSession = itemIdAndSession.Clone();
			attachment.Load();
			idAndSession.AttachmentIds.Add(attachment.Id);
			attachmentType.AttachmentId = new AttachmentIdType(idAndSession.GetConcatenatedId().Id);
			return attachmentType;
		}

		private void Dispose(bool fromDispose)
		{
			if (this.attachmentBuilder != null)
			{
				this.attachmentBuilder.Dispose();
				this.attachmentBuilder = null;
			}
		}

		private readonly DisposeTracker disposeTracker;

		private TargetFolderId savedItemFolderId;

		private ItemResponseShape responseShape;

		private ConflictResolutionType conflictResolutionType;

		private ItemId itemId;

		private PropertyUpdate[] updates;

		private AttachmentType[] attachments;

		private AttachmentBuilder attachmentBuilder;

		private bool makeItemImmutable;
	}
}
