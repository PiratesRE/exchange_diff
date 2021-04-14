using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class MoveCopyCommandBase<RequestType, ResponseType> : MultiStepServiceCommand<RequestType, ResponseType>, IDisposeTrackable, IDisposable where RequestType : BaseRequest
	{
		public MoveCopyCommandBase(CallContext callContext, RequestType request) : base(callContext, request)
		{
			this.PrepareCommandMembers();
			this.disposeTracker = this.GetDisposeTracker();
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MoveCopyCommandBase<RequestType, ResponseType>>(this);
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

		internal override IExchangeWebMethodResponse GetResponse()
		{
			BaseInfoResponse baseInfoResponse = this.CreateResponse();
			baseInfoResponse.BuildForResults<ResponseType>(base.Results);
			return baseInfoResponse;
		}

		internal override void PreExecuteCommand()
		{
			this.destinationFolder = this.GetDestinationFolder();
		}

		protected virtual Folder GetDestinationFolder()
		{
			IdAndSession idAndSession = null;
			try
			{
				idAndSession = base.IdConverter.ConvertTargetFolderIdToIdAndHierarchySession(this.destinationFolderId, true);
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new ToFolderNotFoundException(innerException);
			}
			return Folder.Bind(idAndSession.Session, idAndSession.Id, null);
		}

		protected abstract IdAndSession GetIdAndSession(ServiceObjectId objectId);

		internal override ServiceResult<ResponseType> Execute()
		{
			IdAndSession idAndSession = this.GetIdAndSession(this.objectIds[base.CurrentStep]);
			if (idAndSession.GetAsStoreObjectId().ObjectType == StoreObjectType.CalendarItemOccurrence)
			{
				throw new CalendarExceptionCannotMoveOrCopyOccurrence();
			}
			this.ValidateOperation(this.destinationFolder.Session, idAndSession);
			return this.MoveCopyObject(idAndSession);
		}

		protected virtual ServiceResult<ResponseType> MoveCopyObject(IdAndSession idAndSession)
		{
			ServiceResult<ResponseType> result = null;
			using (StoreObject storeObject = this.BindObjectFromRequest(idAndSession.Session, idAndSession.Id))
			{
				AggregateOperationResult aggregateOperationResult = this.DoOperation(this.destinationFolder.Session, idAndSession.Session, idAndSession.Id);
				if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
				{
					throw new MoveCopyException();
				}
				result = this.PrepareResult(storeObject, aggregateOperationResult.GroupOperationResults);
			}
			this.objectsChanged++;
			return result;
		}

		private void ValidateOperation(StoreSession storeSession, IdAndSession idAndSession)
		{
			this.SubclassValidateOperation(storeSession, idAndSession);
		}

		protected virtual void SubclassValidateOperation(StoreSession storeSession, IdAndSession idAndSession)
		{
		}

		internal override int StepCount
		{
			get
			{
				return this.objectIds.Count;
			}
		}

		private void Dispose(bool fromDispose)
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			if (!this.disposed)
			{
				if (this.destinationFolder != null)
				{
					this.destinationFolder.Dispose();
					this.destinationFolder = null;
				}
				this.disposed = true;
			}
		}

		protected abstract BaseInfoResponse CreateResponse();

		protected abstract void PrepareCommandMembers();

		protected abstract AggregateOperationResult DoOperation(StoreSession destinationSession, StoreSession sourceSession, StoreId sourceId);

		protected abstract StoreObject BindObjectFromRequest(StoreSession storeSession, StoreId storeId);

		protected abstract ServiceResult<ResponseType> PrepareResult(StoreObject storeObject, GroupOperationResult[] groupOperationResults);

		private readonly DisposeTracker disposeTracker;

		protected IList<ServiceObjectId> objectIds;

		protected BaseFolderId destinationFolderId;

		protected Folder destinationFolder;

		private bool disposed;
	}
}
