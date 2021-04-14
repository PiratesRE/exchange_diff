using System;
using System.Text;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PerformanceDataProvider : IPerformanceDataProvider
	{
		public PerformanceDataProvider(string name) : this(name, true)
		{
		}

		public PerformanceDataProvider(string name, bool threadLocal)
		{
			this.Name = name;
			this.ThreadLocal = threadLocal;
		}

		public string Name { get; private set; }

		public bool ThreadLocal { get; private set; }

		public uint RequestCount { get; protected set; }

		public TimeSpan Latency { get; protected set; }

		public string Operations
		{
			get
			{
				if (this.operationsBuilder != null)
				{
					return this.operationsBuilder.ToString();
				}
				return string.Empty;
			}
		}

		public bool IsSnapshotInProgress
		{
			get
			{
				return this.snapshotsInProgress > 0U;
			}
		}

		public virtual PerformanceData TakeSnapshot(bool begin)
		{
			this.UpdateSnapshotInProgress(begin);
			return new PerformanceData(this.Latency, this.RequestCount);
		}

		public void ResetOperations()
		{
			this.operationsBuilder = null;
		}

		public void AppendToOperations(string append)
		{
			if (this.IsSnapshotInProgress)
			{
				if (this.operationsBuilder == null)
				{
					this.operationsBuilder = new StringBuilder();
				}
				if (this.operationsBuilder.Length + 1 + append.Length <= 32767)
				{
					if (this.operationsBuilder.Length > 0)
					{
						this.operationsBuilder.Append('/');
					}
					this.operationsBuilder.Append(append);
				}
			}
		}

		public IDisposable StartRequestTimer()
		{
			this.RequestCount += 1U;
			return new PerformanceDataProvider.PerformanceTimer(this);
		}

		public IDisposable StartOperationTimer()
		{
			return new PerformanceDataProvider.PerformanceTimer(this);
		}

		private void UpdateSnapshotInProgress(bool begin)
		{
			if (begin)
			{
				if (PerformanceReportingOptions.Instance.LatencyDetectionEnabled)
				{
					this.snapshotsInProgress += 1U;
					return;
				}
			}
			else if (this.snapshotsInProgress > 0U)
			{
				this.snapshotsInProgress -= 1U;
			}
		}

		public const int MaximumOperationsBufferLength = 32767;

		private StringBuilder operationsBuilder;

		private uint snapshotsInProgress;

		private sealed class PerformanceTimer : IDisposable
		{
			public PerformanceTimer(PerformanceDataProvider perfDataProvider)
			{
				this.perfDataProvider = perfDataProvider;
			}

			public void Dispose()
			{
				this.perfDataProvider.Latency += this.stopwatch.Elapsed;
			}

			private MyStopwatch stopwatch = MyStopwatch.StartNew();

			private PerformanceDataProvider perfDataProvider;
		}
	}
}
