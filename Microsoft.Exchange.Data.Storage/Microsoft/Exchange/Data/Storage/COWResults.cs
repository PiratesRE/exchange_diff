using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class COWResults
	{
		internal COWResults(StoreSession session, ICollection<StoreObjectId> itemIds)
		{
			this.session = session;
			if (itemIds != null)
			{
				this.itemIds = new StoreObjectId[itemIds.Count];
				itemIds.CopyTo(this.itemIds, 0);
				this.topOperationResults = new List<GroupOperationResult>(itemIds.Count);
			}
			else
			{
				this.topOperationResults = new List<GroupOperationResult>(0);
			}
			this.ResetPartialResults();
		}

		internal COWResults(StoreSession session, ICollection<StoreObjectId> itemIds, ConflictResolutionResult conflictResolutionResult) : this(session, itemIds)
		{
			this.conflictResolutionResult = conflictResolutionResult;
		}

		internal COWResults(StoreSession session, StoreObjectId itemId, bool itemOperationSuccess) : this(session, new StoreObjectId[]
		{
			itemId
		})
		{
			this.itemOperationSuccess = itemOperationSuccess;
		}

		internal void ResetPartialResults()
		{
			this.partialOperationResults = new List<GroupOperationResult>(1);
		}

		internal List<GroupOperationResult> GetPartialResults()
		{
			return this.partialOperationResults;
		}

		internal ConflictResolutionResult ConflictResolutionResult
		{
			get
			{
				return this.conflictResolutionResult;
			}
		}

		internal bool ItemOperationSuccess
		{
			get
			{
				return this.itemOperationSuccess;
			}
		}

		internal StoreObjectId CopyOnWriteGeneratedId
		{
			get
			{
				return this.copyOnWriteGeneratedId;
			}
			set
			{
				this.copyOnWriteGeneratedId = value;
			}
		}

		internal StoreObjectId CalendarLogGeneratedId
		{
			get
			{
				return this.calendarLogGeneratedId;
			}
			set
			{
				this.calendarLogGeneratedId = value;
			}
		}

		internal void AddPartialResult(GroupOperationResult result)
		{
			this.partialOperationResults.Add(result);
			if (ExTraceGlobals.SessionTracer.IsTraceEnabled(TraceType.WarningTrace))
			{
				if (result.OperationResult == OperationResult.PartiallySucceeded)
				{
					ExTraceGlobals.SessionTracer.TraceWarning<string, int>((long)this.session.GetHashCode(), "Dumpster internal operation partial success: exception {0},  item id count {1}.", (result.Exception != null) ? result.Exception.ToString() : "null", (result.ObjectIds != null) ? result.ObjectIds.Count : 0);
					return;
				}
				if (result.OperationResult == OperationResult.Failed)
				{
					ExTraceGlobals.SessionTracer.TraceWarning<string, int>((long)this.session.GetHashCode(), "Dumpster internal operation failure: exception {0},  item id count {1}.", (result.Exception != null) ? result.Exception.ToString() : "null", (result.ObjectIds != null) ? result.ObjectIds.Count : 0);
				}
			}
		}

		internal bool AnyPartialResultFailure()
		{
			foreach (GroupOperationResult groupOperationResult in this.partialOperationResults)
			{
				if (groupOperationResult.OperationResult == OperationResult.Failed)
				{
					return true;
				}
			}
			return false;
		}

		internal bool AnyResultNotSucceeded()
		{
			foreach (GroupOperationResult groupOperationResult in this.topOperationResults)
			{
				if (groupOperationResult.OperationResult == OperationResult.PartiallySucceeded || groupOperationResult.OperationResult == OperationResult.Failed)
				{
					return true;
				}
			}
			return false;
		}

		internal void AddResult(GroupOperationResult result)
		{
			this.topOperationResults.Add(result);
			if (ExTraceGlobals.SessionTracer.IsTraceEnabled(TraceType.WarningTrace))
			{
				if (result.OperationResult == OperationResult.PartiallySucceeded)
				{
					ExTraceGlobals.SessionTracer.TraceWarning<string, int>((long)this.session.GetHashCode(), "Dumpster top item operation partial success: exception {0},  item id count {1}.", (result.Exception != null) ? result.Exception.ToString() : "null", (result.ObjectIds != null) ? result.ObjectIds.Count : 0);
					return;
				}
				if (result.OperationResult == OperationResult.Failed)
				{
					ExTraceGlobals.SessionTracer.TraceWarning<string, int>((long)this.session.GetHashCode(), "Dumpster top item operation failure: exception {0},  item id count {1}.", (result.Exception != null) ? result.Exception.ToString() : "null", (result.ObjectIds != null) ? result.ObjectIds.Count : 0);
				}
			}
		}

		internal void AddResult(IEnumerable<GroupOperationResult> results)
		{
			if (results != null)
			{
				foreach (GroupOperationResult result in results)
				{
					this.AddResult(result);
				}
			}
		}

		internal GroupOperationResult GetResults()
		{
			OperationResult operationResult = OperationResult.Succeeded;
			LocalizedException storageException = null;
			foreach (GroupOperationResult groupOperationResult in this.topOperationResults)
			{
				if (groupOperationResult.OperationResult == OperationResult.Failed)
				{
					operationResult = OperationResult.Failed;
					storageException = groupOperationResult.Exception;
					break;
				}
				if (operationResult != OperationResult.PartiallySucceeded && groupOperationResult.OperationResult == OperationResult.PartiallySucceeded)
				{
					operationResult = OperationResult.PartiallySucceeded;
					storageException = groupOperationResult.Exception;
				}
			}
			return new GroupOperationResult(operationResult, this.itemIds, storageException);
		}

		private readonly StoreSession session;

		private readonly StoreObjectId[] itemIds;

		private ConflictResolutionResult conflictResolutionResult;

		private bool itemOperationSuccess;

		private List<GroupOperationResult> partialOperationResults;

		private List<GroupOperationResult> topOperationResults;

		private StoreObjectId copyOnWriteGeneratedId;

		private StoreObjectId calendarLogGeneratedId;
	}
}
