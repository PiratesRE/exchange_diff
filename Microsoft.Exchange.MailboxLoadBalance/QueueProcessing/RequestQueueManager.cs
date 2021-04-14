using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RequestQueueManager : IRequestQueueManager
	{
		public RequestQueueManager()
		{
			this.injectionQueues = new ConcurrentDictionary<Guid, RequestQueue>();
			this.processingQueues = new ConcurrentDictionary<Guid, RequestQueue>();
			this.queueMonitor = new Timer(new TimerCallback(this.VerifyQueues), null, TimeSpan.Zero, TimeSpan.FromMinutes(1.0));
		}

		public IRequestQueue MainProcessingQueue
		{
			get
			{
				return this.GetProcessingQueue(Guid.Empty, "Main");
			}
		}

		public IRequestQueue GetInjectionQueue(DirectoryDatabase database)
		{
			RequestQueue requestQueue;
			if (!this.injectionQueues.TryGetValue(database.Guid, out requestQueue))
			{
				InjectionQueueCounters counters = new InjectionQueueCounters(database.Name);
				RequestQueue value = new RequestQueue(database.Guid, counters);
				this.injectionQueues.TryAdd(database.Guid, value);
			}
			return this.injectionQueues[database.Guid];
		}

		public IRequestQueue GetProcessingQueue(LoadEntity loadObject)
		{
			return this.GetProcessingQueue(loadObject.Guid, loadObject.Name);
		}

		public IRequestQueue GetProcessingQueue(DirectoryObject directoryObject)
		{
			return this.GetProcessingQueue(directoryObject.Guid, directoryObject.Name);
		}

		public QueueManagerDiagnosticData GetDiagnosticData(bool includeRequestDetails, bool includeRequestVerboseData)
		{
			return new QueueManagerDiagnosticData
			{
				InjectionQueues = (from iq in this.injectionQueues
				select iq.Value.GetDiagnosticData(includeRequestDetails, includeRequestVerboseData)).ToList<QueueDiagnosticData>(),
				ProcessingQueues = (from pq in this.processingQueues
				select pq.Value.GetDiagnosticData(includeRequestDetails, includeRequestVerboseData)).ToList<QueueDiagnosticData>()
			};
		}

		public void Clean()
		{
			foreach (IRequestQueue requestQueue in this.injectionQueues.Values)
			{
				requestQueue.Clean();
			}
			foreach (IRequestQueue requestQueue2 in this.processingQueues.Values)
			{
				requestQueue2.Clean();
			}
		}

		private IRequestQueue GetProcessingQueue(Guid queueId, string queueName)
		{
			RequestQueue requestQueue;
			if (!this.processingQueues.TryGetValue(queueId, out requestQueue))
			{
				RequestQueue value = new RequestQueue(queueId, new ProcessingQueueCounters(queueName));
				this.processingQueues.TryAdd(queueId, value);
			}
			return this.processingQueues[queueId];
		}

		private void VerifyQueues(object state)
		{
			foreach (RequestQueue requestQueue in this.injectionQueues.Values)
			{
				requestQueue.EnsureThreadIsActiveIfNeeded();
			}
			foreach (RequestQueue requestQueue2 in this.processingQueues.Values)
			{
				requestQueue2.EnsureThreadIsActiveIfNeeded();
			}
		}

		private readonly Timer queueMonitor;

		private readonly ConcurrentDictionary<Guid, RequestQueue> injectionQueues;

		private readonly ConcurrentDictionary<Guid, RequestQueue> processingQueues;
	}
}
