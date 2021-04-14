using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class DeleteCommandBase<RequestType, ResponseType> : MultiStepServiceCommand<RequestType, ResponseType> where RequestType : BaseRequest
	{
		public DeleteCommandBase(CallContext callContext, RequestType request) : base(callContext, request)
		{
		}

		protected abstract StoreObject GetStoreObject(IdAndSession idAndSession);

		protected abstract IdAndSession GetIdAndSession(ServiceObjectId objectId);

		internal override ServiceResult<ResponseType> Execute()
		{
			DeleteItemRequest deleteItemRequest = base.Request as DeleteItemRequest;
			if (deleteItemRequest != null && deleteItemRequest.Ids != null && deleteItemRequest.Ids.Length > base.CurrentStep && base.LogItemId())
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "PreDeleteId: ", string.Format("{0}:{1}", deleteItemRequest.Ids[base.CurrentStep].GetId(), deleteItemRequest.Ids[base.CurrentStep].GetChangeKey()));
			}
			IdAndSession idAndSession = null;
			try
			{
				idAndSession = this.GetIdAndSession(this.objectIds[base.CurrentStep]);
			}
			catch (ObjectNotFoundException)
			{
				if (!this.IgnoreObjectNotFoundError)
				{
					ExTraceGlobals.DeleteItemCallTracer.TraceDebug((long)this.GetHashCode(), "DeleteCommandBase.Execute: ObjectNotFoundException caught. Will be rethrown.");
					throw;
				}
				ExTraceGlobals.DeleteItemCallTracer.TraceDebug((long)this.GetHashCode(), "DeleteCommandBase.Execute: ObjectNotFoundException caught and ignored.");
				return this.CreateEmptyResponseType();
			}
			bool returnNewItemIds = false;
			if (deleteItemRequest != null)
			{
				returnNewItemIds = deleteItemRequest.ReturnMovedItemIds;
			}
			ServiceResult<ResponseType> result;
			using (DelegateSessionHandleWrapper delegateSessionHandleWrapper = base.GetDelegateSessionHandleWrapper(idAndSession))
			{
				if (delegateSessionHandleWrapper != null)
				{
					idAndSession = new IdAndSession(idAndSession.Id, delegateSessionHandleWrapper.Handle.MailboxSession);
				}
				result = this.DeleteObject(idAndSession, returnNewItemIds);
			}
			this.objectsChanged++;
			return result;
		}

		internal override int StepCount
		{
			get
			{
				return this.objectIds.Count;
			}
		}

		protected virtual ServiceResult<ResponseType> DeleteObject(IdAndSession idAndSession, bool returnNewItemIds)
		{
			ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug<StoreId, DisposalType>(0L, "DeleteObject called for storeObjectId '{0}' using disposalType '{1}'", idAndSession.Id, this.disposalType);
			DeleteItemFlags deleteItemFlags = (DeleteItemFlags)this.disposalType;
			LocalizedException ex = null;
			ServiceResult<ResponseType> serviceResult = null;
			try
			{
				using (StoreObject storeObject = this.GetStoreObject(idAndSession))
				{
					this.ValidateOperation(storeObject);
					serviceResult = this.TryExecuteDelete(storeObject, deleteItemFlags, idAndSession, out ex, returnNewItemIds);
					if (serviceResult == null)
					{
						if (ex is CannotMoveDefaultFolderException)
						{
							throw new DeleteDistinguishedFolderException(ex);
						}
						throw new DeleteItemsException(ex);
					}
				}
			}
			catch (ObjectNotFoundException)
			{
				if (!this.IgnoreObjectNotFoundError)
				{
					ExTraceGlobals.DeleteItemCallTracer.TraceDebug((long)this.GetHashCode(), "DeleteCommandBase.DeleteObject: ObjectNotFoundException caught. Will be rethrown.");
					throw;
				}
				ExTraceGlobals.DeleteItemCallTracer.TraceDebug((long)this.GetHashCode(), "DeleteCommandBase.DeleteObject: ObjectNotFoundException caught and ignored.");
			}
			return serviceResult;
		}

		protected abstract ServiceResult<ResponseType> TryExecuteDelete(StoreObject storeObject, DeleteItemFlags deleteItemFlags, IdAndSession idAndSession, out LocalizedException localizedException, bool returnNewItemIds);

		protected abstract ServiceResult<ResponseType> CreateEmptyResponseType();

		protected AggregateOperationResult ExecuteDelete(StoreObject storeObject, DeleteItemFlags deleteItemFlags, IdAndSession idAndSession, bool returnNewItemIds)
		{
			return idAndSession.DeleteRootXsoItem(storeObject.ParentId, deleteItemFlags, returnNewItemIds);
		}

		private void ValidateOperation(StoreObject storeObject)
		{
			this.ExecuteCommandValidations(storeObject);
			if (storeObject.Session is PublicFolderSession && this.disposalType == DisposalType.MoveToDeletedItems)
			{
				throw new ServiceInvalidOperationException((storeObject is Folder) ? CoreResources.IDs.ErrorCannotMovePublicFolderOnDelete : CoreResources.IDs.ErrorCannotMovePublicFolderItemOnDelete);
			}
		}

		protected abstract void ExecuteCommandValidations(StoreObject storeObject);

		internal virtual bool IgnoreObjectNotFoundError
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		protected IList<ServiceObjectId> objectIds;

		protected DisposalType disposalType;

		protected CalendarItemOperationType.CreateOrDelete? sendMeetingCancellations = null;

		protected AffectedTaskOccurrencesType? affectedTaskOccurrences = null;
	}
}
