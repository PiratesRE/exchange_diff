using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.Win32;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class PerformanceTrackerBase : IPerformanceTracker
	{
		public PerformanceTrackerBase()
		{
			this.stopwatch = new Stopwatch();
			this.internalState = PerformanceTrackerBase.InternalState.Stopped;
		}

		public PerformanceTrackerBase(IMailboxSession mailboxSession) : this()
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			this.SetMailboxSessionToTrack(mailboxSession);
		}

		protected TimeSpan ElapsedTime
		{
			get
			{
				return this.stopwatch.Elapsed;
			}
		}

		private protected TimeSpan CpuTime { protected get; private set; }

		private protected TimeSpan StoreRpcLatency { protected get; private set; }

		private protected int StoreRpcCount { protected get; private set; }

		private protected TimeSpan DirectoryLatency { protected get; private set; }

		private protected int DirectoryCount { protected get; private set; }

		private protected TimeSpan StoreTimeInServer { protected get; private set; }

		private protected TimeSpan StoreTimeInCPU { protected get; private set; }

		private protected int StorePagesRead { protected get; private set; }

		private protected int StorePagesPreread { protected get; private set; }

		private protected int StoreLogRecords { protected get; private set; }

		private protected int StoreLogBytes { protected get; private set; }

		public void SetMailboxSessionToTrack(IMailboxSession session)
		{
			if (session != null)
			{
				if (this.mailboxSession != null && !object.ReferenceEquals(session, this.mailboxSession))
				{
					throw new InvalidOperationException("Only one mailbox session can be tracked at a time");
				}
				this.mailboxSession = session;
				if (this.internalState == PerformanceTrackerBase.InternalState.Started)
				{
					this.startCumulativeRPCPerformanceStatistics = session.GetStoreCumulativeRPCStats();
				}
			}
		}

		public virtual void Start()
		{
			this.EnforceInternalState(PerformanceTrackerBase.InternalState.Stopped, "Start");
			this.stopwatch.Start();
			this.startThreadTimes = ThreadTimes.GetFromCurrentThread();
			this.startStorePerformanceData = RpcDataProvider.Instance.TakeSnapshot(true);
			this.startDirectoryPerformanceData = PerformanceContext.Current.TakeSnapshot(true);
			if (this.mailboxSession != null)
			{
				this.startCumulativeRPCPerformanceStatistics = this.mailboxSession.GetStoreCumulativeRPCStats();
			}
			this.internalState = PerformanceTrackerBase.InternalState.Started;
		}

		public virtual void Stop()
		{
			this.EnforceInternalState(PerformanceTrackerBase.InternalState.Started, "Stop");
			this.stopwatch.Stop();
			ThreadTimes fromCurrentThread = ThreadTimes.GetFromCurrentThread();
			PerformanceData pd = RpcDataProvider.Instance.TakeSnapshot(false);
			PerformanceData pd2 = PerformanceContext.Current.TakeSnapshot(false);
			this.internalState = PerformanceTrackerBase.InternalState.Stopped;
			this.CpuTime += fromCurrentThread.Kernel - this.startThreadTimes.Kernel + (fromCurrentThread.User - this.startThreadTimes.User);
			PerformanceData performanceData = pd - this.startStorePerformanceData;
			this.StoreRpcLatency += performanceData.Latency;
			this.StoreRpcCount += (int)performanceData.Count;
			PerformanceData performanceData2 = pd2 - this.startDirectoryPerformanceData;
			this.DirectoryLatency += performanceData2.Latency;
			this.DirectoryCount += (int)performanceData2.Count;
			this.CalculateStorePerformanceStatistics();
		}

		private void CalculateStorePerformanceStatistics()
		{
			if (this.mailboxSession != null)
			{
				CumulativeRPCPerformanceStatistics storeCumulativeRPCStats = this.mailboxSession.GetStoreCumulativeRPCStats();
				this.StoreTimeInServer += storeCumulativeRPCStats.timeInServer - this.startCumulativeRPCPerformanceStatistics.timeInServer;
				this.StoreTimeInCPU += storeCumulativeRPCStats.timeInCPU - this.startCumulativeRPCPerformanceStatistics.timeInCPU;
				this.StorePagesRead += (int)(storeCumulativeRPCStats.pagesRead - this.startCumulativeRPCPerformanceStatistics.pagesRead);
				this.StorePagesPreread += (int)(storeCumulativeRPCStats.pagesPreread - this.startCumulativeRPCPerformanceStatistics.pagesPreread);
				this.StoreLogRecords += (int)(storeCumulativeRPCStats.logRecords - this.startCumulativeRPCPerformanceStatistics.logRecords);
				this.StoreLogBytes += (int)(storeCumulativeRPCStats.logBytes - this.startCumulativeRPCPerformanceStatistics.logBytes);
			}
		}

		protected void EnforceInternalState(PerformanceTrackerBase.InternalState expectedState, string action)
		{
			if (this.internalState != expectedState)
			{
				throw new InvalidOperationException(string.Format("{0} can only be performed when state is {1}. Present state is {2}", action, expectedState, this.internalState));
			}
		}

		private readonly Stopwatch stopwatch;

		private IMailboxSession mailboxSession;

		private ThreadTimes startThreadTimes;

		private PerformanceData startStorePerformanceData;

		private PerformanceData startDirectoryPerformanceData;

		private CumulativeRPCPerformanceStatistics startCumulativeRPCPerformanceStatistics;

		private PerformanceTrackerBase.InternalState internalState;

		protected enum InternalState
		{
			Stopped,
			Started
		}
	}
}
