using System;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class AsyncAuditReceiver : DisposeTrackableBase
	{
		public AsyncAuditReceiver()
		{
			this.workerThread = new Thread(new ThreadStart(this.DoWork));
			this.workerThread.Name = "AsyncAuditReceiver";
		}

		public void Start()
		{
			if (this.cancellationTokenSource == null)
			{
				this.cancellationTokenSource = new CancellationTokenSource();
				this.workerThread.Start();
				return;
			}
			throw new InvalidOperationException("Async Auditing receiver already started");
		}

		public void Stop()
		{
			if (this.cancellationTokenSource != null)
			{
				this.cancellationTokenSource.Cancel();
				this.workerThread.Join(1000);
				this.cancellationTokenSource.Dispose();
				this.cancellationTokenSource = null;
			}
		}

		private void DoWork()
		{
			while (!this.cancellationTokenSource.Token.IsCancellationRequested)
			{
				this.CheckQueues(DateTime.UtcNow);
			}
		}

		private void CheckQueues(DateTime currentTime)
		{
			this.CheckMainQueue(currentTime);
		}

		internal void CheckMainQueue(DateTime currentTime)
		{
			IQueue<AuditData> queue = QueueFactory.GetQueue<AuditData>(Queues.AdminAuditingMainQueue);
			IQueueMessage<AuditData> next = queue.GetNext(1000, this.cancellationTokenSource.Token);
			if (next != null)
			{
				AuditData data = next.Data;
				try
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						data.AuditLogger.WriteAuditRecord(data.AuditRecord);
					});
				}
				catch (GrayException)
				{
					IQueue<AuditData> queue2 = QueueFactory.GetQueue<AuditData>(Queues.AdminAuditingRetryQueue);
					queue2.Send(data);
				}
				finally
				{
					queue.Commit(next);
				}
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this.Stop();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AsyncAuditReceiver>(this);
		}

		private const int TimeoutInMilliseconds = 1000;

		private Thread workerThread;

		private CancellationTokenSource cancellationTokenSource;
	}
}
