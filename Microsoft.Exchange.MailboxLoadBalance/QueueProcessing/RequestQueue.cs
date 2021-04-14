using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Logging;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RequestQueue : IRequestQueue
	{
		public RequestQueue(Guid queueId, IQueueCounters counters)
		{
			this.activeThreads = 0L;
			this.queueId = queueId;
			this.queue = new ConcurrentQueue<IRequest>();
			this.counters = counters;
			this.queuedItemSignal = new AutoResetEvent(false);
		}

		public Guid Id
		{
			get
			{
				return this.queueId;
			}
		}

		public void EnqueueRequest(IRequest request)
		{
			this.counters.QueueLengthCounter.Increment();
			this.counters.IncomingRequestRateCounter.IncrementBy(1L);
			request.AssignQueue(this);
			this.queue.Enqueue(request);
			this.queuedItemSignal.Set();
			this.EnsureThreadIsActiveIfNeeded();
		}

		public void EnsureThreadIsActiveIfNeeded()
		{
			if (!this.queue.IsEmpty && Interlocked.CompareExchange(ref this.activeThreads, 1L, 0L) == 0L)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcessQueuedItems));
			}
		}

		public QueueDiagnosticData GetDiagnosticData(bool includeRequestDetails, bool includeRequestVerboseDiagnostics)
		{
			QueueDiagnosticData queueDiagnosticData = new QueueDiagnosticData
			{
				QueueGuid = this.queueId,
				QueueLength = this.queue.Count,
				IsActive = (this.activeThreads > 0L)
			};
			queueDiagnosticData.CurrentRequest = ((this.currentRequest != null) ? this.currentRequest.GetDiagnosticData(includeRequestVerboseDiagnostics) : null);
			if (includeRequestDetails)
			{
				queueDiagnosticData.Requests = (from request in this.queue
				select request.GetDiagnosticData(includeRequestVerboseDiagnostics)).ToList<RequestDiagnosticData>();
			}
			return queueDiagnosticData;
		}

		public void Clean()
		{
			IRequest request;
			while (this.queue.TryDequeue(out request))
			{
			}
		}

		public override string ToString()
		{
			return string.Format("RequestQueue(Id={0},Active={1},QueuedItems={2})", this.queueId, this.activeThreads > 0L, this.queue.Count);
		}

		private void ProcessQueuedItems(object state)
		{
			try
			{
				for (;;)
				{
					IRequest request;
					if (this.queue.IsEmpty)
					{
						if (!this.queuedItemSignal.WaitOne(TimeSpan.FromSeconds(10.0)))
						{
							break;
						}
					}
					else if (this.queue.TryDequeue(out request))
					{
						this.counters.QueueLengthCounter.Decrement();
						this.currentRequest = request;
						request.Process();
						this.counters.ExecutionRateCounter.IncrementBy(1L);
						DatabaseRequestLog.Write(request);
						this.currentRequest = null;
					}
				}
			}
			finally
			{
				Interlocked.Exchange(ref this.activeThreads, 0L);
			}
		}

		private readonly IQueueCounters counters;

		private readonly ConcurrentQueue<IRequest> queue;

		private readonly Guid queueId;

		private readonly AutoResetEvent queuedItemSignal;

		private long activeThreads;

		private IRequest currentRequest;
	}
}
