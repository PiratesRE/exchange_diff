using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class ReferralQueue
	{
		internal ReferralQueue(DirectoryContext directoryContext)
		{
			this.directoryContext = directoryContext;
		}

		internal Dictionary<string, int> UnitTestHelper_UniqueAuthoritiesInQueue
		{
			get
			{
				return this.uniqueAuthoritiesInQueue;
			}
		}

		internal int UnitTestHelper_UniqueAuthoritiesInQueueNotPending
		{
			get
			{
				return this.uniqueAuthoritiesInQueueNotPending;
			}
		}

		internal HashSet<string> UnitTestHelper_PendingAuthorities
		{
			get
			{
				return this.pendingAuthorities;
			}
		}

		internal bool UnitTestHelper_ShouldCreateWorkerThread
		{
			get
			{
				return this.ShouldCreateWorkerThread();
			}
		}

		private bool IsWorkerAvailable
		{
			get
			{
				return this.currentWorkerCount < 5;
			}
		}

		private bool IsDone
		{
			get
			{
				return this.referralQueue.Count == 0 && this.pendingAuthorities.Count == 0;
			}
		}

		private bool IsMoreReferralsReadyToProcess
		{
			get
			{
				return this.uniqueAuthoritiesInQueueNotPending != 0;
			}
		}

		private bool IsMoreReferralsReadyToProcessOrDone
		{
			get
			{
				return this.IsMoreReferralsReadyToProcess || this.IsDone;
			}
		}

		internal void BeginWorker(ReferralQueue.State stateParam)
		{
			this.WaitForEvent(this.workerAvailableEvent);
			if (!this.ShouldCreateWorkerThread())
			{
				try
				{
					stateParam.WorkerMethod(stateParam.WorkerState);
					return;
				}
				finally
				{
					this.pendingAuthorities.Remove(stateParam.AuthorityKey);
					this.UpdateOnQueueChange();
				}
			}
			this.currentWorkerCount++;
			ReferralQueue.UpdateEvent(this.workerAvailableEvent, this.IsWorkerAvailable);
			bool flag = true;
			try
			{
				TrackingEventBudget.AcquireThread();
				flag = false;
			}
			finally
			{
				if (flag)
				{
					TrackingEventBudget.ReleaseThread();
				}
			}
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.WorkerMethodWrapper), stateParam);
		}

		internal void Enqueue(ReferralQueue.ReferralData referralData)
		{
			this.referralQueue.Enqueue(referralData);
			string key = referralData.Authority.ToString();
			int num;
			if (this.uniqueAuthoritiesInQueue.TryGetValue(key, out num))
			{
				num++;
			}
			else
			{
				num = 1;
			}
			this.uniqueAuthoritiesInQueue[key] = num;
			this.UpdateOnQueueChange();
		}

		internal bool DeQueue(out ReferralQueue.ReferralData referralData)
		{
			this.WaitForEvent(this.moreReferralsOrDoneEvent);
			if (!this.IsMoreReferralsReadyToProcess)
			{
				referralData = default(ReferralQueue.ReferralData);
				return false;
			}
			referralData = this.GetNextReferralThatIsNotPending().Value;
			string item = referralData.Authority.ToString();
			this.pendingAuthorities.Add(item);
			this.UpdateDequeue(referralData);
			this.UpdateOnQueueChange();
			return true;
		}

		internal void Clear()
		{
			this.referralQueue.Clear();
			this.uniqueAuthoritiesInQueue.Clear();
			this.UpdateOnQueueChange();
		}

		private static void UpdateEvent(ManualResetEvent eventObject, bool conditionValue)
		{
			if (conditionValue)
			{
				eventObject.Set();
				return;
			}
			eventObject.Reset();
		}

		private void WorkerMethodWrapper(object stateObject)
		{
			ReferralQueue.State state = (ReferralQueue.State)stateObject;
			bool flag = false;
			bool flag2 = false;
			TrackingError trackingError = null;
			try
			{
				this.directoryContext.Acquire();
				flag = true;
				if (this.directoryContext.DiagnosticsContext.DiagnosticsLevel == DiagnosticsLevel.Etw)
				{
					CommonDiagnosticsLogTracer traceWriter = new CommonDiagnosticsLogTracer();
					TraceWrapper.SearchLibraryTracer.Register(traceWriter);
					flag2 = true;
				}
				state.WorkerMethod(state.WorkerState);
			}
			catch (TrackingTransientException ex)
			{
				if (!ex.IsAlreadyLogged)
				{
					trackingError = ex.TrackingError;
				}
			}
			catch (TrackingFatalException ex2)
			{
				if (ex2.IsAlreadyLogged)
				{
					trackingError = ex2.TrackingError;
				}
			}
			catch (TransientException ex3)
			{
				trackingError = new TrackingError(ErrorCode.UnexpectedErrorTransient, string.Empty, ex3.Message, ex3.ToString());
			}
			catch (DataSourceOperationException ex4)
			{
				trackingError = new TrackingError(ErrorCode.UnexpectedErrorPermanent, string.Empty, "Error from Active Directory provider", ex4.ToString());
			}
			catch (DataValidationException ex5)
			{
				trackingError = new TrackingError(ErrorCode.UnexpectedErrorPermanent, string.Empty, "Validation Error from Active Directory provider", ex5.ToString());
			}
			finally
			{
				if (flag)
				{
					if (trackingError != null)
					{
						TraceWrapper.SearchLibraryTracer.TraceError<TrackingError>(this.GetHashCode(), "Error in woker thread while processing referral, {0}", trackingError);
						this.directoryContext.Errors.Errors.Add(trackingError);
					}
					this.pendingAuthorities.Remove(state.AuthorityKey);
					this.currentWorkerCount--;
					ReferralQueue.UpdateEvent(this.workerAvailableEvent, this.IsWorkerAvailable);
					this.UpdateOnQueueChange();
					if (flag2)
					{
						TraceWrapper.SearchLibraryTracer.Unregister();
					}
					this.directoryContext.Yield();
					TrackingEventBudget.ReleaseThread();
				}
			}
		}

		private bool ShouldCreateWorkerThread()
		{
			int num = this.uniqueAuthoritiesInQueueNotPending + this.pendingAuthorities.Count;
			return num > 1;
		}

		private ReferralQueue.ReferralData? GetNextReferralThatIsNotPending()
		{
			for (int i = 0; i < this.referralQueue.Count; i++)
			{
				ReferralQueue.ReferralData referralData = this.referralQueue.Dequeue();
				string item = referralData.Authority.ToString();
				if (!this.pendingAuthorities.Contains(item))
				{
					return new ReferralQueue.ReferralData?(referralData);
				}
				this.referralQueue.Enqueue(referralData);
			}
			return null;
		}

		private int GetUniqueAuthoritiesInQueueThatAreNotPending()
		{
			int num = this.uniqueAuthoritiesInQueue.Count;
			foreach (string item in this.uniqueAuthoritiesInQueue.Keys)
			{
				if (this.pendingAuthorities.Contains(item))
				{
					num--;
				}
			}
			return num;
		}

		private void UpdateDequeue(ReferralQueue.ReferralData item)
		{
			string key = item.Authority.ToString();
			int num = this.uniqueAuthoritiesInQueue[key];
			if (num == 1)
			{
				this.uniqueAuthoritiesInQueue.Remove(key);
				return;
			}
			this.uniqueAuthoritiesInQueue[key] = num - 1;
		}

		private void UpdateOnQueueChange()
		{
			this.uniqueAuthoritiesInQueueNotPending = this.GetUniqueAuthoritiesInQueueThatAreNotPending();
			ReferralQueue.UpdateEvent(this.moreReferralsOrDoneEvent, this.IsMoreReferralsReadyToProcessOrDone);
		}

		private void WaitForEvent(ManualResetEvent eventObject)
		{
			bool flag = false;
			try
			{
				this.directoryContext.Yield();
				flag = true;
				eventObject.WaitOne();
			}
			finally
			{
				if (flag)
				{
					this.directoryContext.Acquire();
				}
			}
		}

		private const int MaxWorkers = 5;

		private DirectoryContext directoryContext;

		private int currentWorkerCount;

		private Queue<ReferralQueue.ReferralData> referralQueue = new Queue<ReferralQueue.ReferralData>();

		private Dictionary<string, int> uniqueAuthoritiesInQueue = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

		private HashSet<string> pendingAuthorities = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

		private int uniqueAuthoritiesInQueueNotPending;

		private ManualResetEvent workerAvailableEvent = new ManualResetEvent(true);

		private ManualResetEvent moreReferralsOrDoneEvent = new ManualResetEvent(true);

		internal struct ReferralData
		{
			internal Node Node { get; set; }

			internal TrackingAuthority Authority { get; set; }
		}

		internal class State
		{
			internal object WorkerState { get; set; }

			internal string AuthorityKey { get; set; }

			internal WaitCallback WorkerMethod { get; set; }
		}
	}
}
