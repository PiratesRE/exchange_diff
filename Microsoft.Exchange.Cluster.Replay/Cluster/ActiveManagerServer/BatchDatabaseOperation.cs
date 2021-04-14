using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class BatchDatabaseOperation : DisposeTrackableBase
	{
		internal List<AmDbOperation> GetCompletedOperationList()
		{
			if (this.Phase != BatchDatabaseOperation.BatchPhase.Complete)
			{
				throw new InvalidOperationException("Operations cannot be fetched before completion");
			}
			return this.opList;
		}

		internal BatchDatabaseOperation.BatchPhase Phase { get; private set; }

		internal BatchDatabaseOperation()
		{
			this.Phase = BatchDatabaseOperation.BatchPhase.Initializing;
		}

		internal void AddOperation(AmDbOperation operation)
		{
			lock (this.locker)
			{
				if (this.Phase != BatchDatabaseOperation.BatchPhase.Initializing)
				{
					throw new InvalidOperationException("Operations cannot be added after Dispatch");
				}
				operation.CompletionCallback = (AmReportCompletionDelegate)Delegate.Combine(operation.CompletionCallback, new AmReportCompletionDelegate(this.OnOperationComplete));
				this.opList.Add(operation);
			}
		}

		private void OnOperationComplete(IADDatabase db)
		{
			lock (this.locker)
			{
				if (++this.totalOperationsCompleted == this.opList.Count)
				{
					this.Phase = BatchDatabaseOperation.BatchPhase.Complete;
					if (this.completionEvent != null)
					{
						this.completionEvent.Set();
					}
				}
			}
		}

		internal void DispatchOperations()
		{
			bool flag = false;
			lock (this.locker)
			{
				if (this.Phase != BatchDatabaseOperation.BatchPhase.Initializing)
				{
					throw new InvalidOperationException("Batch can only be dispatched once");
				}
				if (this.opList.Count == 0)
				{
					this.Phase = BatchDatabaseOperation.BatchPhase.Complete;
				}
				else
				{
					this.Phase = BatchDatabaseOperation.BatchPhase.Dispatching;
					this.completionEvent = new ManualOneShotEvent("BatchDatabaseOperation");
					flag = true;
				}
			}
			if (flag)
			{
				AmDatabaseQueueManager databaseQueueManager = AmSystemManager.Instance.DatabaseQueueManager;
				foreach (AmDbOperation opr in this.opList)
				{
					databaseQueueManager.Enqueue(opr);
				}
				lock (this.locker)
				{
					if (this.Phase == BatchDatabaseOperation.BatchPhase.Dispatching)
					{
						this.Phase = BatchDatabaseOperation.BatchPhase.Running;
					}
				}
			}
		}

		internal bool WaitForComplete(TimeSpan timeout)
		{
			bool result = false;
			ManualOneShotEvent manualOneShotEvent = null;
			lock (this.locker)
			{
				if (this.completionEvent != null)
				{
					manualOneShotEvent = this.completionEvent;
				}
				else
				{
					result = (this.Phase == BatchDatabaseOperation.BatchPhase.Complete);
				}
			}
			if (manualOneShotEvent != null && manualOneShotEvent.WaitOne(timeout) == ManualOneShotEvent.Result.Success)
			{
				result = true;
			}
			return result;
		}

		internal void WaitForComplete()
		{
			TimeSpan timeSpan = TimeSpan.FromMinutes(30.0);
			if (!this.WaitForComplete(timeSpan))
			{
				string message = string.Format("Timeout after {0} waiting for BatchDatabaseOperation", timeSpan);
				throw new TimeoutException(message);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			lock (this.locker)
			{
				if (!this.disposeCalled)
				{
					if (disposing && this.completionEvent != null)
					{
						this.completionEvent.Dispose();
						this.completionEvent = null;
					}
					this.disposeCalled = true;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<BatchDatabaseOperation>(this);
		}

		private List<AmDbOperation> opList = new List<AmDbOperation>(20);

		private int totalOperationsCompleted;

		private object locker = new object();

		private bool disposeCalled;

		private ManualOneShotEvent completionEvent;

		internal enum BatchPhase
		{
			Initializing,
			Dispatching,
			Running,
			Complete
		}
	}
}
