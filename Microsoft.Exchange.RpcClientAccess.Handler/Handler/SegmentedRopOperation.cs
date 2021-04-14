using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class SegmentedRopOperation : BaseObject
	{
		protected SegmentedRopOperation(RopId ropId)
		{
			this.ropId = ropId;
			this.referencedActivityScope = ReferencedActivityScope.Current;
			if (this.referencedActivityScope != null)
			{
				this.referencedActivityScope.AddRef();
			}
		}

		internal static bool SafeSegmentExecution(SegmentedRopOperation segmentedRopOperation, Action execution)
		{
			Exception ex2;
			try
			{
				execution();
				return true;
			}
			catch (StoragePermanentException ex)
			{
				ex2 = ex;
			}
			catch (StorageTransientException ex3)
			{
				ex2 = ex3;
			}
			ExTraceGlobals.FailedRopTracer.TraceDebug<object, MethodInfo, Exception>((long)segmentedRopOperation.GetHashCode(), "Received exception in {0}{1}. XsoException = {2}", execution.Target, execution.Method, ex2);
			Exception ex4;
			SegmentedRopOperation.TranslateSegmentedOperationException(ex2, out ex4, out segmentedRopOperation.errorCode);
			if (ex4 != null)
			{
				segmentedRopOperation.exception = ex4;
			}
			return false;
		}

		public int CompletedWork
		{
			get
			{
				return this.completedWork;
			}
		}

		public int TotalWork
		{
			get
			{
				return this.totalWork;
			}
			protected set
			{
				this.totalWork = value;
			}
		}

		internal bool SafeSegmentExecution(Action execution)
		{
			return SegmentedRopOperation.SafeSegmentExecution(this, execution);
		}

		internal bool DoNextBatchOperation()
		{
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			SegmentOperationResult segmentOperationResult;
			Exception ex;
			ErrorCode error;
			bool flag;
			try
			{
				if (this.referencedActivityScope != null)
				{
					ActivityContext.SetThreadScope(this.referencedActivityScope.ActivityScope);
				}
				flag = ExceptionTranslator.TryExecuteCatchAndTranslateExceptions<SegmentOperationResult>(new Func<SegmentOperationResult>(this.InternalDoNextBatchOperation), (SegmentOperationResult unused) => ErrorCode.None, true, out segmentOperationResult, out ex, out error);
			}
			finally
			{
				ActivityContext.SetThreadScope(currentActivityScope);
			}
			if (!flag)
			{
				this.CalculateAggregateResult(OperationResult.Failed, error);
				RopHandlerHelper.TraceRopResult(this.ropId, ex, error);
				this.errorCode = error;
				return false;
			}
			if (segmentOperationResult.OperationResult != SegmentOperationResult.NeutralOperationResult)
			{
				this.CalculateAggregateResult(segmentOperationResult);
			}
			this.completedWork += segmentOperationResult.CompletedWork;
			return !segmentOperationResult.IsCompleted;
		}

		internal OperationResult AggregatedResult
		{
			get
			{
				OperationResult? operationResult = this.aggregatedResult;
				if (operationResult == null)
				{
					return OperationResult.Succeeded;
				}
				return operationResult.GetValueOrDefault();
			}
		}

		internal ErrorCode ErrorCode
		{
			get
			{
				return this.errorCode;
			}
			set
			{
				this.errorCode = value;
			}
		}

		internal Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		internal abstract RopResult CreateCompleteResult(object progressToken, IProgressResultFactory resultFactory);

		internal abstract RopResult CreateCompleteResultForProgress(object progressToken, ProgressResultFactory resultFactory);

		protected static StoreObjectId[] ConvertMessageIds(IdConverter converter, StoreObjectId parentFolderId, IList<StoreObjectId> objectIds)
		{
			StoreId storeId = new StoreId(converter.GetFidFromId(parentFolderId));
			StoreObjectId[] array = new StoreObjectId[objectIds.Count];
			int num = 0;
			foreach (StoreObjectId messageStoreObjectId in objectIds)
			{
				array[num++] = converter.CreateMessageId(storeId, converter.GetMidFromMessageId(messageStoreObjectId));
			}
			return array;
		}

		protected static int EstimateWork(CoreFolder rootFolder, bool includeAssociatedMessage, List<StoreObjectId> subfolderIds)
		{
			int num = 0;
			using (QueryResult queryResult = rootFolder.QueryExecutor.FolderQuery(FolderQueryFlags.DeepTraversal | FolderQueryFlags.SuppressNotificationsOnMyActions, null, null, SegmentedRopOperation.properties))
			{
				object[][] rows;
				do
				{
					rows = queryResult.GetRows(int.MaxValue);
					foreach (object[] array2 in rows)
					{
						StoreId storeId = array2[0] as StoreId;
						if (storeId != null)
						{
							int num2 = (array2[1] is int) ? ((int)array2[1]) : 0;
							num += num2;
							int num3 = (array2[2] is int) ? ((int)array2[2]) : 0;
							num += num3;
							if (includeAssociatedMessage)
							{
								int num4 = (array2[3] is int) ? ((int)array2[3]) : 0;
								num += num4;
							}
							if (subfolderIds != null)
							{
								subfolderIds.Add(StoreId.GetStoreObjectId(storeId));
							}
						}
					}
				}
				while (rows.Length != 0);
			}
			num += (int)rootFolder.PropertyBag[FolderSchema.ChildCount];
			num += (int)rootFolder.PropertyBag[FolderSchema.ItemCount];
			if (includeAssociatedMessage)
			{
				num += (int)rootFolder.PropertyBag[FolderSchema.AssociatedItemCount];
			}
			return num;
		}

		protected abstract SegmentOperationResult InternalDoNextBatchOperation();

		protected void CalculateAggregateResult(SegmentOperationResult segmentOperationResult)
		{
			ErrorCode errorCode = ErrorCode.None;
			if (segmentOperationResult.Exception != null)
			{
				Exception ex;
				SegmentedRopOperation.TranslateSegmentedOperationException(segmentOperationResult.Exception, out ex, out errorCode);
				if (errorCode == ErrorCode.PartialCompletion)
				{
					errorCode = ErrorCode.None;
				}
				if (ex != null)
				{
					this.exception = ex;
				}
			}
			this.CalculateAggregateResult(segmentOperationResult.OperationResult, errorCode);
		}

		protected void CalculateAggregateResult(OperationResult operationResult, ErrorCode error)
		{
			if (this.aggregatedResult == null)
			{
				this.aggregatedResult = new OperationResult?(operationResult);
				return;
			}
			OperationResult operationResult2 = this.aggregatedResult.Value;
			switch (operationResult)
			{
			case OperationResult.Succeeded:
				if (operationResult2 == OperationResult.Failed)
				{
					operationResult2 = OperationResult.PartiallySucceeded;
				}
				break;
			case OperationResult.Failed:
				if (operationResult2 == OperationResult.Succeeded)
				{
					operationResult2 = OperationResult.PartiallySucceeded;
				}
				break;
			case OperationResult.PartiallySucceeded:
				operationResult2 = OperationResult.PartiallySucceeded;
				break;
			}
			this.aggregatedResult = new OperationResult?(operationResult2);
			if (error != ErrorCode.None)
			{
				this.errorCode = error;
			}
		}

		protected void DetectCopyMoveLoop(ReferenceCount<CoreFolder> destinationFolderReferenceCount, List<StoreObjectId> subFolderIds)
		{
			VersionedId id = destinationFolderReferenceCount.ReferencedObject.Id;
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(id);
			bool flag = subFolderIds.Contains(storeObjectId);
			if (flag)
			{
				throw new RopExecutionException("Copy/move folder cycle detected", (ErrorCode)2147747339U);
			}
		}

		protected RopId RopId
		{
			get
			{
				return this.ropId;
			}
		}

		protected bool IsPartiallyCompleted
		{
			get
			{
				return this.aggregatedResult == OperationResult.PartiallySucceeded;
			}
		}

		protected override void InternalDispose()
		{
			if (this.referencedActivityScope != null)
			{
				this.referencedActivityScope.Release();
			}
			base.InternalDispose();
		}

		private static void TranslateSegmentedOperationException(Exception xsoException, out Exception momtException, out ErrorCode errorCode)
		{
			momtException = null;
			errorCode = ErrorCode.None;
			if (xsoException == null)
			{
				return;
			}
			try
			{
				errorCode = ExceptionTranslator.ErrorFromXsoException(xsoException);
			}
			catch (SessionDeadException ex)
			{
				momtException = ex;
			}
			catch (ClientBackoffException ex2)
			{
				momtException = ex2;
			}
		}

		public override string ToString()
		{
			return string.Format("RopId:{0}; Work: {1}/{2}; Error:{3}", new object[]
			{
				this.ropId,
				this.CompletedWork,
				this.TotalWork,
				this.errorCode
			});
		}

		private static readonly StorePropertyDefinition[] properties = new StorePropertyDefinition[]
		{
			FolderSchema.Id,
			FolderSchema.ChildCount,
			FolderSchema.ItemCount,
			FolderSchema.AssociatedItemCount
		};

		private readonly RopId ropId;

		private readonly ReferencedActivityScope referencedActivityScope;

		private OperationResult? aggregatedResult = null;

		private ErrorCode errorCode;

		private Exception exception;

		private int completedWork;

		private int totalWork;

		protected static readonly SegmentOperationResult FinalResult = new SegmentOperationResult
		{
			CompletedWork = 0,
			IsCompleted = true,
			OperationResult = SegmentOperationResult.NeutralOperationResult,
			Exception = null
		};

		protected static readonly SegmentOperationResult FailedSegmentResult = new SegmentOperationResult
		{
			CompletedWork = 0,
			IsCompleted = false,
			OperationResult = OperationResult.Failed,
			Exception = null
		};
	}
}
